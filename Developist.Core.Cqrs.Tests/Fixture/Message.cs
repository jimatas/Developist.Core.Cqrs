// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System;

namespace Developist.Core.Cqrs.Tests.Fixture
{
    public class Message
    {
        public Message() { }
        public Message(Guid id) => Id = id;

        public Guid Id { get; }
        public string Text { get; set; }

        public override string ToString() => Text;
    }
}
