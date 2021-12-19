// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Cqrs.Queries;

using System;

namespace Developist.Core.Cqrs.Tests
{
    public class GetMessageById : IQuery<Message>
    {
        public Guid Id { get; set; }
    }
}
