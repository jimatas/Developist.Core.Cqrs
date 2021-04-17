// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System;

namespace Developist.Core.Cqrs.Samples.Common.Diagnostics
{
    internal class DeferredToString
    {
        private readonly Lazy<string> initializer;
        public DeferredToString(Func<string> toString) => initializer = new(toString);
        public override string ToString() => initializer.Value;
    }
}
