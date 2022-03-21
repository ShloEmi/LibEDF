namespace LibEDF_DotNet
{
    public interface IEDFFile
    {
        // ReSharper disable UnusedMemberInSuper.Global
        void Open(string edfFilePath);
        void Open(byte[] edfBytes);
        void Save(string edfFilePath);
        // ReSharper restore UnusedMemberInSuper.Global
    }

    public class EDFFile : IEDFFile
    {
        public EDFHeader Header { get; set; } = new();
        public EDFSignal[] Signals { get; set; }

        public EDFFile() { }
        public EDFFile(string edfFilePath) => 
            Open(edfFilePath);

        public EDFFile(byte[] edfBytes){
            Open(edfBytes);
        }

        public void ReadBase64(string edfBase64)
        {
            byte[] edfBytes = Convert.FromBase64String(edfBase64);
            Open(edfBytes);
        }

        public void Open(string edfFilePath)
        {
            using var reader = new EDFReader(File.Open(edfFilePath, FileMode.Open));
            Header = reader.ReadHeader();
            Signals = reader.ReadSignals();
        }

        public void Open(byte[] edfBytes)
        {
            using var r = new EDFReader(edfBytes);
            Header = r.ReadHeader();
            Signals = r.ReadSignals();
        }

        public void Save(string edfFilePath)
        {
            using var writer = new EDFWriter(File.Open(edfFilePath, FileMode.Create));
            writer.WriteEDF(this, edfFilePath);
        }
    }
}
