using Developist.Core.Cqrs.Queries;

namespace Developist.Core.Cqrs.Tests.Fixture.Queries;

public record QueryWithMultipleHandlers : IQuery<SampleQueryResult>;
