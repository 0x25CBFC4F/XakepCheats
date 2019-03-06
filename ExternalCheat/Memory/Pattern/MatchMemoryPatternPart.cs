namespace ExternalCheat.Memory.Pattern
{
    public class MatchMemoryPatternPart : IMemoryPatternPart
    {
        public byte ValidByte { get; }

        public MatchMemoryPatternPart(byte valid)
        {
            ValidByte = valid;
        }

        public bool Matches(byte b) => ValidByte == b;
    }
}
