﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using Roslyn.Utilities;
using System.Diagnostics;

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    internal partial class SyntaxList
    {
        internal sealed class WithLotsOfChildren : WithManyChildrenBase
        {
            private readonly int[] _childOffsets;

            internal WithLotsOfChildren(ArrayElement<CSharpSyntaxNode>[] children)
                : base(children)
            {
                _childOffsets = CalculateOffsets(children);
            }

            internal WithLotsOfChildren(ObjectReader reader)
                : base(reader)
            {
                _childOffsets = CalculateOffsets(this.children);
            }

            internal override void WriteTo(ObjectWriter writer)
            {
                base.WriteTo(writer);
                // don't write offsets out, recompute them on construction
            }

            internal override Func<ObjectReader, object> GetReader()
            {
                return r => new WithLotsOfChildren(r);
            }

            public override int GetSlotOffset(int index)
            {
                return _childOffsets[index];
            }

            /// <summary>
            /// Find the slot that contains the given offset.
            /// </summary>
            /// <param name="offset">The target offset. Must be between 0 and <see cref="GreenNode.FullWidth"/>.</param>
            /// <returns>The slot index of the slot containing the given offset.</returns>
            /// <remarks>
            /// This implementation uses a binary search to find the first slot that contains
            /// the given offset.
            /// </remarks>
            public override int FindSlotIndexContainingOffset(int offset)
            {
                Debug.Assert(offset >= 0 && offset < FullWidth);
                int idx = _childOffsets.BinarySearch(offset);

                if (idx < 0)
                {
                    idx = (~idx - 1);
                }

                // skip zero-length nodes (they won't ever contain the offset)
                while (idx < _childOffsets.Length - 1 && _childOffsets[idx] == _childOffsets[idx + 1])
                {
                    idx++;
                }

                return idx;
            }

            private static int[] CalculateOffsets(ArrayElement<CSharpSyntaxNode>[] children)
            {
                int n = children.Length;
                var childOffsets = new int[n];
                int offset = 0;
                for (int i = 0; i < n; i++)
                {
                    childOffsets[i] = offset;
                    offset += children[i].Value.FullWidth;
                }
                return childOffsets;
            }
        }
    }
}
