using System;

namespace Onkyo.eISCP.Commands
{
    public class FrontTone : ISCPMessage
    {
        private short _toneLevel;
        private Enums.Zone _zone;
        private readonly ToneType _toneType;

        public FrontTone(ToneType toneType, Enums.Zone zone = Enums.Zone.Main) : base(GetCommandForZone(zone, toneType))
        {
            _toneType = toneType;
            _zone = zone;
        }

        public short ToneLevel
        {
            get => _toneLevel;
            set
            {
                if (_toneLevel != value)
                {
                    _toneLevel = value;
                    OnPropertyChanged();
                }
            }
        }

        public override void ParseFrom(ISCPMessage source)
        {
            base.ParseFrom(source);

            if (RawData == "N/A")
                ToneLevel = -1;
            else
                ToneLevel = short.Parse(source.RawData, System.Globalization.NumberStyles.HexNumber);
        }

        protected override string BuildMessage()
        {
            string toneLevelString;
            switch (ToneLevel)
            {
                case -10:
                    toneLevelString = "-A";
                    break;
                case 10:
                    toneLevelString = "+A";
                    break;
                default:
                    toneLevelString = (ToneLevel > 0 ? "+" : "") + ToneLevel;
                    break;
            }

            return toneLevelString;
        }

        private static string GetCommandForZone(Enums.Zone zone, ToneType toneType)
        {
            string baseCommand;
            switch (zone)
            {
                case Enums.Zone.Main:
                    baseCommand = "TFR";
                    break;
                case Enums.Zone.Zone2:
                    baseCommand = "ZTN";
                    break;
                case Enums.Zone.Zone3:
                    baseCommand = "TN3";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(zone), zone, null);
            }

            switch (toneType)
            {
                case ToneType.Treble:
                    return $"{baseCommand}T";
                case ToneType.Bass:
                    return $"{baseCommand}B";
                default:
                    throw new ArgumentOutOfRangeException(nameof(toneType), toneType, null);
            }
        }
    }

    public enum ToneType
    {
        Treble,
        Bass
    }
}