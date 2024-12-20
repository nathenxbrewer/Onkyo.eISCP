using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

namespace Onkyo.eISCP
{
    public class ISCPConnection : IDisposable
    {
        private TcpClient _client;
        private NetworkStream _networkStream;
        private Thread _listenerThread;
        private ManualResetEventSlim _messageProcessingResetEvent;
        private CancellationTokenSource _listenerCancellationToken;
        
        private string _ipAddress;
        private int _port;

        public bool Connected => _client != null && _client.Connected;

        public bool KeepAlive { get; set; }

        public void Connect(IPAddress address, int port = 60128)
        {
            Connect(new IPEndPoint(address, port));
        }

        public void Connect(string ipaddress, int port = 60128)
        {
            Connect(new IPEndPoint(IPAddress.Parse(ipaddress), port));
        }

        public void Connect(IPEndPoint endPoint)
        {
            //store values to reconnect later
            _ipAddress = endPoint.Address.ToString();
            _port = endPoint.Port;
            
            _client = new TcpClient();
            _client.Connect(endPoint);
            _networkStream = _client.GetStream();
            _listenerCancellationToken = new CancellationTokenSource();

            _listenerThread = new Thread(MessageReceiveListener);
            _listenerThread.IsBackground = true;
            _listenerThread.Start();

            _messageProcessingResetEvent = new ManualResetEventSlim(true);

            SetKeepAlive(_client);
            OnConnected();
        }
        
        static void SetKeepAlive(TcpClient tcpClient)
        {
            // Access the underlying socket
            Socket socket = tcpClient.Client;

            // Enable TCP Keep-Alive
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
        }

        public async Task ConnectAsync(string ipaddress, int port = 60128)
        {
            await Task.Run(() => Connect(ipaddress, port));
        }

        public async Task ConnectAsync(IPAddress address, int port = 60128)
        {
            await Task.Run(() => Connect(address, port));
        }

        public async Task ConnectAsync(IPEndPoint endPoint)
        {
            await Task.Run(() => Connect(endPoint));
        }
 
        public async Task ConnectAsync(ReceiverInfo receiverInfo)
        {
            await ConnectAsync(receiverInfo.IPAddress, receiverInfo.Port);
        }
        
        public async Task<ReceiverInfo> FindAndConnectAsync(string ip, int millisecondsDiscoverTimeout = 5000)
        {
            var receivers = await DiscoverAsync(millisecondsDiscoverTimeout);
            var receiver = receivers.FirstOrDefault(x=>x.IPAddress.ToString() == ip);

            if (receiver == null)
            {
                throw new InvalidOperationException("No Receivier found.");
            }

            await ConnectAsync(receiver);
            return receiver;
        }

        public void Connect(ReceiverInfo receiverInfo)
        {
            Connect(receiverInfo.IPAddress, receiverInfo.Port);
        }

        public event EventHandler<ISCPMessageEventArgs> MessageReceived;

        private void MessageReceiveListener(object obj)
        {
            var readBuf = new byte[2048];
            var messageBuf = new List<byte>();

            while (!_listenerCancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (KeepAlive)
                    {
                        if (!IsClientConnected())
                        {
                            _listenerCancellationToken.Cancel();
                            // Attempt to reconnect
                            Task.Run(async () => await ReconnectAsync());
                            return;
                        }
                    }

                    
                    _messageProcessingResetEvent.Set();
                    var readTask = _networkStream.ReadAsync(readBuf, 0, readBuf.Length, _listenerCancellationToken.Token);
                    int read = readTask.Result;

                    _messageProcessingResetEvent.Reset();

                    if (read > 0)
                    {
                        messageBuf.AddRange(readBuf.Take(read));

                        // Check if we have a complete message
                        while (messageBuf.Count >= 16)
                        {
                            // Check for ISCP magic
                            if (messageBuf.Take(4).SequenceEqual(ISCPMessage.Magic))
                            {
                                var messageSize = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(messageBuf.Skip(8).Take(4).ToArray(), 0));
                                var totalSize = 16 + messageSize;

                                if (messageBuf.Count >= totalSize)
                                {
                                    var msgString = Encoding.ASCII.GetString(messageBuf.Skip(16).Take(messageSize).ToArray());
                                    var msg = ISCPMessage.Parse(msgString);

                                    if (msg != null)
                                    {
                                        System.Diagnostics.Trace.WriteLine($"{DateTime.Now:O} Received {msg}");
                                        MessageReceived?.Invoke(this, new ISCPMessageEventArgs() { Message = msg });
                                    }

                                    messageBuf.RemoveRange(0, totalSize);
                                }
                                else
                                {
                                    break; // Wait for more data
                                }
                            }
                            else
                            {
                                messageBuf.RemoveAt(0); // Remove invalid data
                            }
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Handle cancellation
                    //System.Diagnostics.Trace.WriteLine("Message receive operation was canceled.");
                }
                catch (Exception exp)
                {
                    //System.Diagnostics.Trace.Fail($"Error parsing message: {exp}");
                }
            }
        }

    private bool IsClientConnected()
    {
        try
        {
            if (_client != null && _client.Client != null && _client.Client.Connected)
            {
                // Detect if client is connected
                if (_client.Client.Poll(0, SelectMode.SelectRead))
                {
                    byte[] buffer = new byte[1];
                    if (_client.Client.Receive(buffer, SocketFlags.Peek) == 0)
                    {
                        return false; // Client disconnected
                    }
                }
                return true; // Client connected
            }
        }
        catch
        {
            return false;
        }
        return false;
    }

        private async Task ReconnectAsync()
        {
            try
            {
                Disconnect();
                await ConnectAsync(_ipAddress, _port);
            }
            catch (Exception ex)
            {
                // Handle reconnection exception
                Console.WriteLine($"Reconnection failed: {ex.Message}");
            }
        }

        public WaitHandle MessageProcessingWaitHandle => _messageProcessingResetEvent.WaitHandle;

        public async Task<ISCPMessage> SendCommandAsync(string message)
        {
            var m = ISCPMessage.Parse(message);
            return await SendCommandAsync(m);
        }
        public async Task<ISCPMessage> SendCommandAsync(string command,string message)
        {
            var m = ISCPMessage.Parse(command+message);
            return await SendCommandAsync(m);
        }

        public async Task<ISCPMessage> SendCommandAsync(ISCPMessage message, int millisecondsTimeout = 2000, Func<ISCPMessage, ISCPMessage, bool> acceptedResponseFunc = null)
        {
            var wait = new ManualResetEventSlim(false);
            ISCPMessage response = null;

            // handle to check if command matches. this methods seems more efective then observeable events with Rx.
            var messsageHandler = new EventHandler<ISCPMessageEventArgs>((sender, args) =>
            {
                // check if response is accepted. default behavior ist just verifing if command is the same 
                if ((acceptedResponseFunc != null && acceptedResponseFunc(message, args.Message)) || DefaultResponseAcceptFunc(message, args.Message))
                {
                    response = args.Message;
                    wait.Set();
                }
            });

            try
            {
                MessageReceived += messsageHandler;
                System.Diagnostics.Trace.WriteLine($"{DateTime.Now:O} Send:{message.ToString()}");

                var bytes = message.GetBytes();
                await _networkStream.WriteAsync(bytes, 0, bytes.Length);

                var timeout = !wait.Wait(millisecondsTimeout);
            }
            catch (Exception exp)
            {
                System.Diagnostics.Trace.WriteLine($"{DateTime.Now:O} Error ({message.ToString()}):{exp.ToString()}");
                throw exp;
            }
            finally
            {
                MessageReceived -= messsageHandler; // detach!
            }

            if (response != null)
                System.Diagnostics.Trace.WriteLine($"{DateTime.Now:O} Response({message.Command}):{response.ToString()}");
            else
            {
                System.Diagnostics.Trace.WriteLine($"{DateTime.Now:O} Response timeout ({message.Command}, {millisecondsTimeout} ms).");
                throw new TimeoutException($"Response timeout ({message.Command}, {millisecondsTimeout} ms).");
            }

            return response;
        }

        public async Task<ISCPMessage> SendCommandWithRetryAsync(ISCPMessage message, int nbrOfRetries = 3, int millisecondsTimeout = 2000, Func<ISCPMessage, ISCPMessage, bool> acceptedResponseFunc = null)
        {
            int i = 0;
            ISCPMessage response = null;
            while (i < nbrOfRetries)
            {
                try
                {
                    response = await SendCommandAsync(message, millisecondsTimeout, acceptedResponseFunc);
                    break;
                }
                catch(TimeoutException)
                {
                    i++;
                }
            }
            return response;
        }


        protected bool DefaultResponseAcceptFunc(ISCPMessage originalMessage, ISCPMessage receivedMessage)
        {
            if (originalMessage.Command == receivedMessage.Command)
                return true;
            return false;
        }

        public async Task<TResponse> SendCommandAsync<TResponse>(ISCPMessage message, int millisecondsTimeout = 2000, Func<ISCPMessage, ISCPMessage, bool> acceptedResponseFunc = null) where TResponse : ISCPMessage, new()
        {
            var response = await SendCommandAsync(message, millisecondsTimeout, acceptedResponseFunc);

            if (response != null)
            {
                var newReponse = new TResponse();
                newReponse.ParseFrom(response);
                return newReponse;
            }

            return null;
        }

        public void Disconnect()
        {
            _listenerCancellationToken.Cancel();
            _client.Close();
            OnDisconnect();
        }

        protected virtual void OnConnected()
        {
        }

        protected virtual void OnDisconnect()
        {
        }

        /// <summary>
        /// Try to find ISCP devices on network.
        /// </summary>
        public async Task<List<ReceiverInfo>> DiscoverAsync(int millisecondsTimeout = 5000)
        {
                int onkyoPort = 60128;
                byte[] onkyoMagic = new ISCPMessage("ECNQSTN").GetBytes("!x");
                var foundReceivers = new List<ReceiverInfo>();

                IEnumerable<string> ips = GetInterfaceAddresses();

                // Broadcast magic
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                using (socket)
                {
                    socket.Blocking = false; // So we can use Poll
                    socket.EnableBroadcast = true;
                    socket.Bind(new IPEndPoint(IPAddress.Any, 0));

                    foreach (var i in ips)
                    {
                        try
                        {
                            var ip = IPAddress.Parse(i);
                            socket.SendTo(onkyoMagic, new IPEndPoint(ip, onkyoPort));
                        }
                        catch (SocketException)
                        { }
                    }

                    EndPoint addr = new IPEndPoint(IPAddress.Broadcast, onkyoPort);
                    byte[] data = new byte[1024];
                    int read = 0;
                    while (true)
                    {
                        int microsecondTimeout = (int)(millisecondsTimeout * 1000);
                        if (!socket.Poll(microsecondTimeout, SelectMode.SelectRead))
                            break;

                        read = socket.ReceiveFrom(data, ref addr);
                        var msg = ISCPMessage.Parse(data);

                        // Return string looks something like this:
                        // !1ECNTX-NR609/60128/DX
                        GroupCollection info = Regex.Match(msg.ToString(),
                            //@"!" +
                            //@"(?<device_category>\d)" +
                            @"ECN" +
                            @"(?<model_name>[^/]*)/" +
                            @"(?<iscp_port>\d{5})/" +
                            @"(?<area_code>\w{2})/" +
                            @"(?<identifier>.*)"
                        ).Groups;

                        IPAddress adr = (addr as IPEndPoint).Address;

                        if (!foundReceivers.Any(p => p.IPAddress == adr))
                        {
                            var ri = new ReceiverInfo
                            {
                                Port = Int32.Parse(info["iscp_port"].Value),
                                Model = info["model_name"].Value,
                                IPAddress = adr, 
                            };
                            foundReceivers.Add(ri);
                        }
                    }
                }
                return foundReceivers;
        }

        private static IEnumerable<string> GetInterfaceAddresses()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();

            return (from nic in nics
                    select nic.GetIPProperties()
                      into ipProps
                    from addr in ipProps.UnicastAddresses.Where(addr => addr.Address.AddressFamily == AddressFamily.InterNetwork)
                    select GetBroadcastAddress(addr.Address, addr.IPv4Mask)
                        into network
                    where network != null
                    select network.ToString()).ToList();
        }

        private static IPAddress GetBroadcastAddress(IPAddress address, IPAddress subnetMask)
        {
            if (subnetMask == null) return null;
            byte[] ipAdressBytes = address.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

            if (ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            var broadcastAddress = new byte[ipAdressBytes.Length];
            for (int i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (byte)(ipAdressBytes[i] | (subnetMaskBytes[i] ^ 255));
            }
            return new IPAddress(broadcastAddress);
        }

        public async Task<ReceiverInfo> DiscoverAndConnectAsync(int millisecondsDiscoverTimeout = 5000)
        {
            var receivers = await DiscoverAsync(millisecondsDiscoverTimeout);
            var receiver = receivers.FirstOrDefault();

            if (receiver == null)
            {
                throw new InvalidOperationException("No Receivier found.");
            }

            await ConnectAsync(receiver);
            return receiver;
        }

        public void Dispose()
        {
            if(_client != null)
                Disconnect();
        }
    }

    public class ReceiverInfo
    {
        public IPAddress IPAddress { get; set; }
        public int Port { get; set; }
        public string Model { get; set; }
    }
}
