using System.Text;

namespace LibEDF_DotNet
{
    internal class EDFReader : BinaryReader
    {
        public EDFReader(FileStream fs) : base(fs) { }
        public EDFReader(byte[] edfBytes) : base(new MemoryStream(edfBytes)) { }

        public EDFHeader ReadHeader()
        {
            var h = new EDFHeader();

            this.BaseStream.Seek(0, SeekOrigin.Begin);

            //------ Fixed length header part --------
            h.Version.Value                 = ReadAscii(HeaderItems.Version);
            h.PatientID.Value               = ReadAscii(HeaderItems.PatientID);
            h.RecordID.Value                = ReadAscii(HeaderItems.RecordID);
            h.StartDate.Value               = ReadAscii(HeaderItems.StartDate);
            h.StartTime.Value               = ReadAscii(HeaderItems.StartTime);
            h.NumberOfBytesInHeader.Value   = ReadInt16(HeaderItems.NumberOfBytesInHeader);
            h.Reserved.Value                = ReadAscii(HeaderItems.Reserved);
            h.NumberOfDataRecords.Value     = ReadInt16(HeaderItems.NumberOfDataRecords);
            h.DurationOfDataRecord.Value    = ReadInt16(HeaderItems.DurationOfDataRecord);
            h.NumberOfSignals.Value         = ReadInt16(HeaderItems.NumberOfSignals);

            //------ Variable length header part --------
            int ns = h.NumberOfSignals.Value;
            h.Labels.Value                      = ReadMultipleAscii(HeaderItems.Label, ns);
            h.TransducerType.Value              = ReadMultipleAscii(HeaderItems.TransducerType, ns);
            h.PhysicalDimension.Value           = ReadMultipleAscii(HeaderItems.PhysicalDimension, ns);
            h.PhysicalMinimum.Value             = ReadMultipleDouble(HeaderItems.PhysicalMinimum, ns);
            h.PhysicalMaximum.Value             = ReadMultipleDouble(HeaderItems.PhysicalMaximum, ns);
            h.DigitalMinimum.Value              = ReadMultipleInt(HeaderItems.DigitalMinimum, ns);
            h.DigitalMaximum.Value              = ReadMultipleInt(HeaderItems.DigitalMaximum, ns);
            h.Prefiltering.Value                = ReadMultipleAscii(HeaderItems.Prefiltering, ns);
            h.NumberOfSamplesInDataRecord.Value = ReadMultipleInt(HeaderItems.NumberOfSamplesInDataRecord, ns);
            h.SignalsReserved.Value             = ReadMultipleAscii(HeaderItems.SignalsReserved, ns);

            return h;
        }

        public EDFSignal[] ReadSignals()
        {
            EDFHeader header = ReadHeader();
            var signals = new EDFSignal[header.NumberOfSignals.Value];

            for (var i = 0; i < signals.Length; i++) {
                signals[i] = new EDFSignal();
                signals[i].Label.Value = header.Labels.Value[i];
                signals[i].NumberOfSamples.Value = header.NumberOfSamplesInDataRecord.Value[i];
            }

            //Read the signal sample values
            int readPosition = header.NumberOfBytesInHeader.Value;

            foreach (EDFSignal edfSignal in signals)
            {
                edfSignal.Samples = ReadSignalSamples(readPosition, edfSignal.NumberOfSamples.Value);
                readPosition += edfSignal.Samples.Length * 2; //2 bytes per integer.
            }

            return signals;
        }

        private short[] ReadSignalSamples(int startPosition, int numberOfSamples)
        {
            var samples = new List<short>();
            var countBytesRead = 0;

            this.BaseStream.Seek(startPosition, SeekOrigin.Begin);

            while (countBytesRead < numberOfSamples * 2) //2 bytes per integer
            {
                byte[] intBytes = this.ReadBytes(2);
                var intVal = BitConverter.ToInt16(intBytes, 0);
                samples.Add(intVal);
                countBytesRead += intBytes.Length;
            }

            return samples.ToArray();
        }

        private Int16 ReadInt16(EDFField itemInfo)
        {
            string strInt = ReadAscii(itemInfo).Trim();
            Int16 intResult = -1;
            try { intResult = Convert.ToInt16(strInt); }
            catch (Exception ex) { Console.WriteLine("Error, could not convert string to integer. " + ex.Message); }
            return intResult;
        }

        private string ReadAscii(EDFField itemInfo)
        {
            byte[] bytes = this.ReadBytes(itemInfo.AsciiLength);
            return AsciiString(bytes);
        }

        private string[] ReadMultipleAscii(EDFField itemInfo, int numberOfParts)
        {
            var parts = new List<string>();

            for (var i = 0; i < numberOfParts; i++) {
                byte[] bytes = this.ReadBytes(itemInfo.AsciiLength);
                parts.Add(AsciiString(bytes));
            }
            
            return parts.ToArray();
        }

        private int[] ReadMultipleInt(EDFField itemInfo, int numberOfParts)
        {
            var parts = new List<int>();

            for (var i = 0; i < numberOfParts; i++)
            {
                byte[] bytes = this.ReadBytes(itemInfo.AsciiLength);
                string ascii = AsciiString(bytes);
                parts.Add(Convert.ToInt32(ascii));
            }

            return parts.ToArray();
        }

        private double[] ReadMultipleDouble(EDFField itemInfo, int numberOfParts)
        {
            var parts = new List<double>();

            for (var i = 0; i < numberOfParts; i++)
            {
                byte[] bytes = this.ReadBytes(itemInfo.AsciiLength);
                string ascii = AsciiString(bytes);
                parts.Add(Convert.ToDouble(ascii));
            }

            return parts.ToArray();
        }

        private static string AsciiString(byte[] bytes)
        {
            return Encoding.ASCII.GetString(bytes);
        }
    }
}
