﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Remote.Linq
{
    internal sealed class ObjectReferenceEqualityComparer<T> : IEqualityComparer<T>
    {
        internal static readonly ObjectReferenceEqualityComparer<T> Instance = new ObjectReferenceEqualityComparer<T>();

        private ObjectReferenceEqualityComparer()
        {
        }

        public bool Equals(T x, T y)
        {
            return ReferenceEquals(x, y);
        }

        public int GetHashCode(T obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }
}