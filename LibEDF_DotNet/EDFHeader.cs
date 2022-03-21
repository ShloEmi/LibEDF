using System.Globalization;

namespace LibEDF_DotNet
{
    public class EDFField
    {
        public string Name { get; set; }
        public int AsciiLength { get; set; }

        public EDFField() { }

        public EDFField(string name, int asciiLength) {
            Name = name;
            AsciiLength = asciiLength;
        }
    }

    public class HeaderItems
    {
        //Fixed length header items

        public static EDFField Version { get; } = new("Version", 8);
        public static EDFField PatientID { get; } = new("PatientID", 80);
        public static EDFField RecordID { get; } = new("RecordID", 80);
        public static EDFField StartDate { get; } = new("StartDate", 8);
        public static EDFField StartTime { get; } = new("StartTime", 8);
        public static EDFField NumberOfBytesInHeader { get; } = new("NumberOfBytesInHeader", 8);
        public static EDFField Reserved { get; }  = new("Reserved", 44);
        public static EDFField NumberOfDataRecords { get; } = new("NumberOfDataRecords", 8);
        public static EDFField DurationOfDataRecord { get; } = new("DurationOfDataRecord", 8);
        public static EDFField NumberOfSignals { get; } = new("NumberOfSignals", 4);

        //Variable length header items

        public static EDFField Label { get; } = new("Labels", 16);
        public static EDFField TransducerType { get; } = new("TransducerType", 80);
        public static EDFField PhysicalDimension { get; } = new("PhysicalDimension", 8);
        public static EDFField PhysicalMinimum { get; } = new("PhysicalMinimum", 8);
        public static EDFField PhysicalMaximum { get; } = new("PhysicalMaximum", 8);
        public static EDFField DigitalMinimum { get; } = new("DigitalMinimum", 8);
        public static EDFField DigitalMaximum { get; } = new("DigitalMaximum", 8);
        public static EDFField Prefiltering { get; } = new("Prefiltering", 80);
        public static EDFField NumberOfSamplesInDataRecord { get; } = new("NumberOfSamplesInDataRecord", 8);
        public static EDFField SignalsReserved { get; } = new("SignalsReserved", 32);
    }

    public abstract class HeaderItem
    {
        protected HeaderItem(EDFField info) {
            Name = info.Name;
            AsciiLength = info.AsciiLength;
        }

        public string Name { get; set; }
        public int AsciiLength { get; set; }
        public abstract string ToAscii();
    }

    public class FixedLengthString : HeaderItem
    {
        public string Value { get; set; }
        public FixedLengthString(EDFField info) : base(info) { }

        public override string ToAscii() {
            var asciiString = "";
            if (Value != null)
                asciiString = Value.PadRight(AsciiLength, ' ');
            else
                asciiString = asciiString.PadRight(AsciiLength, ' ');

            return asciiString;
        }
    }

    public class FixedLengthInt : HeaderItem
    {
        public int Value { get; set; }
        public FixedLengthInt(EDFField info) : base(info) { }

        public override string ToAscii()
        {
            var asciiString = "";
            if (Value != null)
                asciiString = Value.ToString().PadRight(AsciiLength, ' ');
            else
                asciiString = asciiString.PadRight(AsciiLength, ' ');

            return asciiString;
        }
    }

    public class FixedLengthDouble : HeaderItem
    {
        public double Value { get; set; }
        public FixedLengthDouble(EDFField info) : base(info) { }

        public override string ToAscii()
        {
            var asciiString = "";
            if (Value != null)
            {
                asciiString = Value.ToString(CultureInfo.InvariantCulture);
                if (asciiString.Length >= AsciiLength)
                    asciiString = asciiString[..AsciiLength];
                else
                    asciiString = Value.ToString(CultureInfo.InvariantCulture).PadRight(AsciiLength, ' ');
            }
            else
            {
                asciiString = asciiString.PadRight(AsciiLength, ' ');
            }

            return asciiString;
        }
    }

    public class VariableLengthString : HeaderItem
    {
        public string[] Value { get; set; }
        public VariableLengthString(EDFField info) : base(info) { }

        public override string ToAscii() {
            var ascii = "";
            foreach (string strVal in Value)
            {
                var temp = strVal;
                if (strVal.Length > AsciiLength)
                    temp = temp[..AsciiLength];
                ascii += temp;
            }
            return ascii;
        }
    }

    public class VariableLengthInt : HeaderItem
    {
        public int[] Value { get; set; }
        public VariableLengthInt(EDFField info) : base(info) { }

        public override string ToAscii() {
            var ascii = "";
            foreach (int intVal in Value)
            {
                var temp = intVal.ToString();
                if (temp.Length > AsciiLength)
                    temp = temp[..AsciiLength];
                ascii += temp;
            }
            return ascii;
        }
    }

    public class VariableLengthDouble : HeaderItem
    {
        public double[] Value { get; set; }
        public VariableLengthDouble(EDFField info) : base(info) { }

        public override string ToAscii() {
            var ascii = "";
            foreach (double doubleVal in Value)
            {
                var temp = doubleVal.ToString(CultureInfo.InvariantCulture);
                if (temp.Length > AsciiLength)
                    temp = temp[..AsciiLength];
                ascii += temp;
            }
            return ascii;
        }
    }

    public class EDFHeader
    {
        public FixedLengthString Version { get; } = new(HeaderItems.Version);
        public FixedLengthString PatientID { get; } = new(HeaderItems.PatientID);
        public FixedLengthString RecordID { get; } = new(HeaderItems.RecordID);
        public FixedLengthString StartDate { get; } = new(HeaderItems.StartDate);
        public FixedLengthString StartTime { get; } = new(HeaderItems.StartTime);
        public FixedLengthInt NumberOfBytesInHeader { get; } = new(HeaderItems.NumberOfBytesInHeader);
        public FixedLengthString Reserved { get; } = new(HeaderItems.Reserved);
        public FixedLengthInt NumberOfDataRecords { get; } = new(HeaderItems.NumberOfDataRecords);
        public FixedLengthInt DurationOfDataRecord { get; } = new(HeaderItems.DurationOfDataRecord);
        public FixedLengthInt NumberOfSignals { get; } = new(HeaderItems.NumberOfSignals);

        public VariableLengthString Labels { get; } = new(HeaderItems.Label);
        public VariableLengthString TransducerType { get; } = new(HeaderItems.TransducerType);
        public VariableLengthString PhysicalDimension { get; } = new(HeaderItems.PhysicalDimension);
        public VariableLengthDouble PhysicalMinimum { get; } = new(HeaderItems.PhysicalMinimum);
        public VariableLengthDouble PhysicalMaximum { get; } = new(HeaderItems.PhysicalMaximum);
        public VariableLengthInt DigitalMinimum { get; } = new(HeaderItems.DigitalMinimum);
        public VariableLengthInt DigitalMaximum { get; } = new(HeaderItems.DigitalMaximum);
        public VariableLengthString Prefiltering { get; } = new(HeaderItems.Prefiltering);
        public VariableLengthInt NumberOfSamplesInDataRecord { get; } = new(HeaderItems.NumberOfSamplesInDataRecord);
        public VariableLengthString SignalsReserved { get; } = new(HeaderItems.SignalsReserved);

        public override string ToString()
        {
            string NL = Environment.NewLine;

            var strOutput = ""; /*TODO: Use StringBuilder!*/
            strOutput += $"8b\tVersion [{Version.Value}]{NL}";
            strOutput += $"80b\tPatient ID [{PatientID.Value}]\n";
            strOutput += $"80b\tRecording ID [{RecordID.Value}]\n";
            strOutput += $"8b\tStart Date [{StartDate.Value}]\n";
            strOutput += $"8b\tStart Time [{StartTime.Value}\n]";
            strOutput += $"8b\tNumber of bytes in header [{NumberOfBytesInHeader.Value}]\n";
            strOutput += $"44b\tReserved [{Reserved.Value}]\n";
            strOutput += $"8b\tNumber of data records [{NumberOfDataRecords.Value}]\n";
            strOutput += $"8b\tDuration of data record [{DurationOfDataRecord.Value}]\n";
            strOutput += $"4b\tNumber of signals [{NumberOfSignals.Value}]\n";

            strOutput += $"\tLabels [{Labels.Value}]\n";
            strOutput += $"\tTransducer type [{TransducerType.Value}]\n";
            strOutput += $"\tPhysical dimension [{PhysicalDimension.Value}]\n";
            strOutput += $"\tPhysical minimum [{PhysicalMinimum.Value}]\n";
            strOutput += $"\tPhysical maximum [{PhysicalMaximum.Value}]\n";
            strOutput += $"\tDigital minimum [{DigitalMinimum.Value}]\n";
            strOutput += $"\tDigital maximum [{DigitalMaximum.Value}]\n";
            strOutput += $"\tPrefiltering [{Prefiltering.Value}]\n";
            strOutput += $"\tNumber of samples in data record [{NumberOfSamplesInDataRecord.Value}]\n";
            strOutput += $"\tSignals reserved [{SignalsReserved.Value}]\n";

            Console.WriteLine($"\n---------- EDF File Header ---------\n{strOutput}");

            return strOutput;
        }
    }
}
