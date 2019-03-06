using System;
using System.Collections.Generic;
using System.Globalization;

namespace ExternalCheat.Memory.Pattern
{
    public class MemoryPattern
    {
        public readonly List<IMemoryPatternPart> PatternParts = new List<IMemoryPatternPart>();
        public int Size => PatternParts.Count;

        public MemoryPattern(string pattern)
        {
            Parse(pattern);
        }

        private void Parse(string pattern)
        {
            var parts = pattern.Split(' ');
            PatternParts.Clear();

            foreach (var part in parts)
            {
                if (part.Length != 2)
                {
                    throw new Exception("Invalid pattern.");
                }

                if (part.Equals("??"))
                {
                    PatternParts.Add(new AnyMemoryPatternPart());
                    continue;
                }

                if (!byte.TryParse(part, NumberStyles.HexNumber, null, out var result))
                {
                    throw new Exception("Invalid pattern.");
                }

                PatternParts.Add(new MatchMemoryPatternPart(result));
            }
        }
    }
}
