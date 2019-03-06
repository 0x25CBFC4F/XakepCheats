namespace ExternalCheat.Memory.Pattern
{
    public class AnyMemoryPatternPart : IMemoryPatternPart
    {
        public bool Matches(byte b) => true;
    }
}
