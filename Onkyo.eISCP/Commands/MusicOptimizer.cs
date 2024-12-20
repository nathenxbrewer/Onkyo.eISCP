using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static Onkyo.eISCP.Enums;

namespace Onkyo.eISCP.Commands
{
    public class MusicOptimizer : ISCPMessage
    {
        private bool _enabled;

        public MusicOptimizer() : this(Zone.Main)
        { }

        public MusicOptimizer(Zone zone) : base("MOT")
        { }

        public bool Enabled
        {
            get => _enabled; set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    OnPropertyChanged();
                }
            }
        }

        protected override string BuildMessage()
        {
            return Enabled ? "01" : "00";
        }

        public override void ParseFrom(ISCPMessage source)
        {
            base.ParseFrom(source);
            Enabled = RawData == "01" ? true : false;
        }
    }

    public class MusicOptimizerStatus : MusicOptimizer
    {
        protected override string BuildMessage()
        {
            return "QSTN";
        }
    }

    public static class MusicOptimizerExtensions
    {
        public static async Task EnableMusicOptimizerAsync(this ISCPConnection connection)
        {
            await connection.SendCommandAsync(new MusicOptimizer() { Enabled = true });
        }

        public static async Task DisableMusicOptimizerAsync(this ISCPConnection connection)
        {
            await connection.SendCommandAsync(new MusicOptimizer() { Enabled = false });
        }
        
        public static async Task<MusicOptimizer> SetMusicOptimizerAsync(this ISCPConnection connection, bool value)
        {
            return await connection.SendCommandAsync<MusicOptimizer>(new MusicOptimizer() { Enabled = value });
        }

        public static async Task<bool> GetMusicOptimizerStatusAsync(this ISCPConnection connection)
        {
            var result = await connection.SendCommandAsync<MusicOptimizer>(new MusicOptimizerStatus());
            return result.RawData == "01";
        }
        public static async Task<MusicOptimizer> ToggleMusicOptimizerAsync(this ISCPConnection connection)
        {
            return await connection.SendCommandAsync<MusicOptimizer>(new MusicOptimizerStatus(){RawData = "UP"});
        }
    }
}