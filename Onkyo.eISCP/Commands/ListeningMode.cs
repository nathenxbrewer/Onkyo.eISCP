using System.Globalization;
using System.Threading.Tasks;

namespace Onkyo.eISCP.Commands
{
    public enum ListeningModeValue
    {
        Stereo = 0x00,
        Direct = 0x01,
        Surround = 0x02,
        Film = 0x03,
        Thx = 0x04,
        Action = 0x05,
        Musical = 0x06,
        MonoMovie = 0x07,
        Orchestra = 0x08,
        Unplugged = 0x09,
        StudioMix = 0x0A,
        TvLogic = 0x0B,
        AllChStereo = 0x0C,
        TheaterDimensional = 0x0D,
        Enhanced7 = 0x0E,
        Mono = 0x0F,
        PureAudio = 0x11,
        Multiplex = 0x12,
        FullMono = 0x13,
        DolbyVirtual = 0x14,
        DtsSurroundSensation = 0x15,
        AudysseyDsx = 0x16,
        DtsVirtualX = 0x17,
        WholeHouseMode = 0x1F,
        Stage = 0x23,
        ActionJapan = 0x25,
        MusicJapan = 0x26,
        SportsJapan = 0x2E,
        Surround5_1 = 0x40,
        StraightDecode = 0x40,
        DolbyExDtsEs = 0x41,
        DolbyEx = 0x41,
        ThxCinema = 0x42,
        ThxSurroundEx = 0x43,
        ThxMusic = 0x44,
        ThxGames = 0x45,
        ThxCinema2 = 0x50,
        ThxMusicMode = 0x51,
        ThxGamesMode = 0x52,
        PlIiPlIixMovie = 0x80,
        PlIiPlIixMusic = 0x81,
        Neo6Cinema = 0x82,
        Neo6Music = 0x83,
        PlIiPlIixThxCinema = 0x84,
        Neo6ThxCinema = 0x85,
        PlIiPlIixGame = 0x86,
        NeuralSurr = 0x87,
        NeuralThx = 0x88,
        PlIiPlIixThxGames = 0x89,
        Neo6ThxGames = 0x8A,
        PlIiPlIixThxMusic = 0x8B,
        Neo6ThxMusic = 0x8C,
        NeuralThxCinema = 0x8D,
        NeuralThxMusic = 0x8E,
        NeuralThxGames = 0x8F,
        PlIizHeight = 0x90,
        Neo6CinemaDtsSurroundSensation = 0x91,
        Neo6MusicDtsSurroundSensation = 0x92,
        NeuralDigitalMusic = 0x93,
        PlIizHeightThxCinema = 0x94,
        PlIizHeightThxMusic = 0x95,
        PlIizHeightThxGames = 0x96,
        PlIizHeightThxU2S2Cinema = 0x97,
        PlIizHeightThxU2S2Music = 0x98,
        PlIizHeightThxU2S2Games = 0x99,
        NeoXGame = 0x9A,
        PlIixPlIiMovieAudysseyDsx = 0xA0,
        PlIixPlIiMusicAudysseyDsx = 0xA1,
        PlIixPlIiGameAudysseyDsx = 0xA2,
        Neo6CinemaAudysseyDsx = 0xA3,
        Neo6MusicAudysseyDsx = 0xA4,
        NeuralSurroundAudysseyDsx = 0xA5,
        NeuralDigitalMusicAudysseyDsx = 0xA6,
        DolbyExAudysseyDsx = 0xA7,
        Auro3D = 0xB0,
        AuroSurround = 0xB1,
        AutoSurround = 0xFF
    }

    public class ListeningMode : ISCPMessage
    {
        private ListeningModeValue _selectedValue;

        public ListeningMode() : base("LMD")
        {
        }

        public ListeningModeValue SelectedValue
        {
            get => _selectedValue;
            set
            {
                if (_selectedValue != value)
                {
                    _selectedValue = value;
                    OnPropertyChanged();
                }
            }
        }

        protected override string BuildMessage()
        {
            if (string.IsNullOrEmpty(RawData))
                return ((int)SelectedValue).ToString("X2");
            return RawData;
        }

        public override void ParseFrom(ISCPMessage source)
        {
            base.ParseFrom(source);
            SelectedValue = (ListeningModeValue)int.Parse(RawData, NumberStyles.HexNumber);
        }
    }

    public static class ListeningModeWrapAroundCommandExtensions
    {
        public static async Task<ListeningMode> SetListeningModeWrapAroundAsync(this ISCPConnection connection, string value)
        {
            return await connection.SendCommandAsync<ListeningMode>(new ListeningMode { RawData = value});
        }
        
        public static async Task<ListeningMode> SetListeningModeAsync(this ISCPConnection connection, ListeningModeValue value)
        {
            return await connection.SendCommandAsync<ListeningMode>(new ListeningMode { SelectedValue = value });
        }

        public static async Task<ListeningMode> GetListeningModeAsync(this ISCPConnection connection)
        {
            return await connection.SendCommandAsync<ListeningMode>(new ListeningMode { RawData = "QSTN" });
        }

        public static async Task<ListeningMode> SetListeningModeWrapAroundUpAsync(this ISCPConnection connection)
        {
            return await connection.SendCommandAsync<ListeningMode>(new ListeningMode { RawData = "UP" });
        }

        public static async Task<ListeningMode> SetListeningModeWrapAroundDownAsync(this ISCPConnection connection)
        {
            return await connection.SendCommandAsync<ListeningMode>(new ListeningMode { RawData = "DOWN" });
        }
    }
}