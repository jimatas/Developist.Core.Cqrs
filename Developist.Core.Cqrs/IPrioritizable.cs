namespace Developist.Core.Cqrs
{
    public interface IPrioritizable
    {
        PriorityLevel Priority => PriorityLevel.Normal;
    }
}
