using System.Text;

namespace LibEDF_DotNet
{
    internal class EDFWriter : BinaryWriter
    {
        public EDFWriter(FileStream fs) : base(fs) { }

        public void WriteEDF(EDFFile edf, string edfFilePath)
        {
            edf.Header.NumberOfBytesInHeader.Value = CalcNumOfBytesInHeader(edf);

            //----------------- Fixed length header items -----------------
            WriteItem(edf.Header.Version);
            WriteItem(edf.Header.PatientID);
            WriteItem(edf.Header.RecordID);
            WriteItem(edf.Header.StartDate);
            WriteItem(edf.Header.StartTime);
            WriteItem(edf.Header.NumberOfBytesInHeader);
            WriteItem(edf.Header.Reserved);
            WriteItem(edf.Header.NumberOfDataRecords);
            WriteItem(edf.Header.DurationOfDataRecord);
            WriteItem(edf.Header.NumberOfSignals);

            //----------------- Variable length header items -----------------
            WriteItem(edf.Signals.Select(s => s.Label));
            WriteItem(edf.Signals.Select(s => s.TransducerType));
            WriteItem(edf.Signals.Select(s => s.PhysicalDimension));
            WriteItem(edf.Signals.Select(s => s.PhysicalMinimum));
            WriteItem(edf.Signals.Select(s => s.PhysicalMaximum));
            WriteItem(edf.Signals.Select(s => s.DigitalMinimum));
            WriteItem(edf.Signals.Select(s => s.DigitalMaximum));
            WriteItem(edf.Signals.Select(s => s.Prefiltering));
            WriteItem(edf.Signals.Select(s => s.NumberOfSamples));
            WriteItem(edf.Signals.Select(s => s.Reserved));

            Console.WriteLine("Writer position after header: " + BaseStream.Position);
            Console.WriteLine("Writing signals.");
            foreach (EDFSignal sig in edf.Signals) WriteSignal(sig);

            Close();
            Console.WriteLine("File size: " + File.ReadAllBytes(edfFilePath).Length);
        }

        private static int CalcNumOfBytesInHeader(EDFFile edf)
        {
            var totalFixedLength = 256;
            int ns = edf.Signals.Length;
            int totalVariableLength = ns * 16 + (ns * 80) * 2 + (ns * 8) * 6 + (ns * 32);
            return totalFixedLength + totalVariableLength;
        }

        public void WriteItem(HeaderItem headerItem)
        {
            string strItem = headerItem.ToAscii();
            byte[] itemBytes = AsciiToBytes(strItem);
            Write(itemBytes);
            Console.WriteLine(headerItem.Name + " [" + strItem + "] \n\n-- ** BYTES LENGTH: " + itemBytes.Length 
                + "> Position after write item: " + BaseStream.Position + "\n");
        }

        public void WriteItem(IEnumerable<HeaderItem> headerItems)
        {
            string joinedItems = StrJoin(headerItems);
            byte[] itemBytes = AsciiToBytes(joinedItems);
            Write(itemBytes);
            Console.WriteLine("[" + joinedItems + "] \n\n-- ** BYTES LENGTH: " + itemBytes.Length 
                + " Position after write item: " + BaseStream.Position + "\n");
        }

        private static string StrJoin(IEnumerable<HeaderItem> list) => 
            list.Aggregate("", (current, item) => current + item.ToAscii());

        private static byte[] AsciiToBytes(string strItem) => 
            Encoding.ASCII.GetBytes(strItem);

        public void WriteSignal(EDFSignal signal)
        {
            Console.WriteLine("Write position before signal: " + BaseStream.Position);
            for (var i = 0; i < signal.NumberOfSamples.Value; i++)
            {
                Write(BitConverter.GetBytes(signal.Samples[i]));
            }

            Console.WriteLine("Write position after signal: " + BaseStream.Position);
        }
    }
}
