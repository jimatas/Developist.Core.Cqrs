// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

namespace Developist.Core.Cqrs
{
    public static class Priorities
    {
        public const sbyte Highest = sbyte.MaxValue;
        public const sbyte VeryHigh = (sbyte)((sbyte.MaxValue + 1) * .75);
        public const sbyte Higher = (sbyte)((sbyte.MaxValue + 1) * .5);
        public const sbyte High = (sbyte)((sbyte.MaxValue + 1) * .25);
        public const sbyte Normal = default;
        public const sbyte Low = (sbyte)(sbyte.MinValue * .25);
        public const sbyte Lower = (sbyte)(sbyte.MinValue * .5);
        public const sbyte VeryLow = (sbyte)(sbyte.MinValue * .75);
        public const sbyte Lowest = sbyte.MinValue;
    }
}
