namespace LibEDF_DotNet
{
    public class EDFSignal
    {
        public FixedLengthString Label              { get; } = new(HeaderItems.Label);
        public FixedLengthString TransducerType     { get; } = new(HeaderItems.TransducerType);
        public FixedLengthString PhysicalDimension  { get; } = new(HeaderItems.PhysicalDimension);
        public FixedLengthDouble PhysicalMinimum    { get; } = new(HeaderItems.PhysicalMinimum);
        public FixedLengthDouble PhysicalMaximum    { get; } = new(HeaderItems.PhysicalMaximum);
        public FixedLengthInt DigitalMinimum        { get; } = new(HeaderItems.DigitalMinimum);
        public FixedLengthInt DigitalMaximum        { get; } = new(HeaderItems.DigitalMaximum);
        public FixedLengthString Prefiltering       { get; } = new(HeaderItems.Prefiltering);
        public FixedLengthInt NumberOfSamples       { get; } = new(HeaderItems.NumberOfSamplesInDataRecord);
        public FixedLengthString Reserved           { get; } = new(HeaderItems.SignalsReserved);

        public short[] Samples { get; set; } = { };

        public override string ToString() => 
            $"{Label} {NumberOfSamples} [{string.Join(",", Samples.Skip(0).Take(10).ToArray())} ...]";
    }
}
