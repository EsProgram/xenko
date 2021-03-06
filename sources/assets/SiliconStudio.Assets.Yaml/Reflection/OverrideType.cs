// Copyright (c) 2014-2017 Silicon Studio Corp. All rights reserved. (https://www.siliconstudio.co.jp)
// See LICENSE.md for full license information.
using System;

namespace SiliconStudio.Core.Reflection
{
    /// <summary>
    /// A Type of override used on a member value.
    /// </summary>
    [Flags]
    public enum OverrideType
    {
        /// <summary>
        /// The value is taken from a base value or this instance if no base (default).
        /// </summary>
        Base = 0,  // This is strictly not a correct value for a flag, but it is used to make sure default value is always base. When testing for this value, better use IsBase() extension method.

        /// <summary>
        /// The value is new and overridden locally. Base value is ignored.
        /// </summary>
        New = 1,

        /// <summary>
        /// The value is sealed and cannot be changed by inherited instances.
        /// </summary>
        Sealed = 2,
    }
}
