// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System;

namespace Developist.Core.Cqrs.Tests
{
    public class CreateMessage : ICommand
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
    }
}
