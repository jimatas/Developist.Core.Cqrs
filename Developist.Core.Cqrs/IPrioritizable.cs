// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

namespace Developist.Core.Cqrs
{
    public interface IPrioritizable
    {
        sbyte Priority { get; }
    }
}
