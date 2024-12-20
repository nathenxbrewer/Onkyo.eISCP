using System.Globalization;
using System.Threading.Tasks;

namespace Onkyo.eISCP.Commands
{
    public class AudioInformationCommand : ISCPMessage
    {
        public AudioInformationCommand() : base("IFA")
        {
        }

        public AudioInformation AudioInfo { get; private set; }

        protected override string BuildMessage()
        {
            return RawData;
        }

        public override void ParseFrom(ISCPMessage source)
        {
            base.ParseFrom(source);
            AudioInfo = ParseAudioInformation(RawData);
        }

        private AudioInformation ParseAudioInformation(string rawData)
        {
            var parts = rawData.Split(',');
            var audioInfo = new AudioInformation
            {
                AudioInputPort = parts.Length > 0 ? parts[0] : null,
                InputSignalFormat = parts.Length > 1 ? parts[1] : null,
                SamplingFrequency = parts.Length > 2 ? parts[2] : null,
                InputSignalChannel = parts.Length > 3 ? parts[3] : null,
                ListeningMode = parts.Length > 4 ? parts[4] : null,
                OutputSignalChannel = parts.Length > 5 ? parts[5] : null,
                OutputSamplingFrequency = parts.Length > 6 ? parts[6] : null,
                PQLS = parts.Length > 7 ? parts[7] : null,
                AutoPhaseControlCurrentDelay = parts.Length > 8 ? parts[8] : null,
                AutoPhaseControlPhase = parts.Length > 9 ? parts[9] : null,
                UpmixMode = parts.Length > 10 ? parts[10] : null
            };
            return audioInfo;
        }
    }

    public static class AudioInformationExtensions
    {
        public static async Task<AudioInformationCommand> GetAudioInformationAsync(this ISCPConnection connection)
        {
            return await connection.SendCommandAsync<AudioInformationCommand>(new AudioInformationCommand() { RawData = "QSTN" });
        }
    }
    
    public class AudioInformation
    {
        public string AudioInputPort { get; set; }
        public string InputSignalFormat { get; set; }
        public string SamplingFrequency { get; set; }
        public string InputSignalChannel { get; set; }
        public string ListeningMode { get; set; }
        public string OutputSignalChannel { get; set; }
        public string OutputSamplingFrequency { get; set; }
        public string PQLS { get; set; }
        public string AutoPhaseControlCurrentDelay { get; set; }
        public string AutoPhaseControlPhase { get; set; }
        public string UpmixMode { get; set; }
    }
}