namespace Developist.Core.Cqrs
{
    public enum PriorityLevel : sbyte
    {
        Highest = 127,
        VeryHigh = 96,
        Higher = 64,
        High = 32,
        Normal = 0,
        Low = -32,
        Lower = -64,
        VeryLow = -96,
        Lowest = -128
    }
}
