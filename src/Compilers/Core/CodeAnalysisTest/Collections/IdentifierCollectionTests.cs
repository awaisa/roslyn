﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis.Text;
using Xunit;

namespace Microsoft.CodeAnalysis.UnitTests.Collections
{
    public class IdentifierCollectionTests
    {
        [Fact]
        public void TestSupportedCollectionInterface()
        {
            TestCases(new string[] { });
            TestCases("a");
            TestCases("a", "b", "c");
            TestCases("c", "b", "a");
            TestCases("alpha", "Alpha", "alphA", "AlphA");
            TestCases("alpha", "beta", "gamma", "Alpha", "betA", "gaMMa");
        }

        private void TestCases(params string[] strings)
        {
            TestCaseSensitive(strings);
            TestCaseInsensitive(strings);
        }

        private void TestCaseSensitive(params string[] strings)
        {
            var idcol = new IdentifierCollection(strings).AsCaseSensitiveCollection();

            Assert.Equal(strings.Length, idcol.Count);
            Assert.Equal(true, Enumerable.SequenceEqual(strings.OrderBy(x => x), idcol.OrderBy(x => x)));
            Assert.Equal(idcol.GetEnumerator().GetType(), ((System.Collections.IEnumerable)idcol).GetEnumerator().GetType());

            AssertContains(idcol, strings);
            AssertNotContains(idcol, strings.Select(s => s.ToUpper()));
            AssertNotContains(idcol, "x");

            var copy = new string[strings.Length];
            idcol.CopyTo(copy, 0);
            Assert.Equal(true, Enumerable.SequenceEqual(strings.OrderBy(x => x), copy.OrderBy(x => x)));
        }

        private void TestCaseInsensitive(params string[] strings)
        {
            var idcol = new IdentifierCollection(strings).AsCaseInsensitiveCollection();

            Assert.Equal(strings.Length, idcol.Count);
            Assert.Equal(true, Enumerable.SequenceEqual(strings.OrderBy(x => x), idcol.OrderBy(x => x)));
            Assert.Equal(idcol.GetEnumerator().GetType(), ((System.Collections.IEnumerable)idcol).GetEnumerator().GetType());

            AssertContains(idcol, strings);
            AssertContains(idcol, strings.Select(s => s.ToUpper()));
            AssertNotContains(idcol, "x");

            var copy = new string[strings.Length];
            idcol.CopyTo(copy, 0);
            Assert.Equal(true, Enumerable.SequenceEqual(strings.OrderBy(x => x), copy.OrderBy(x => x)));
        }

        [Fact]
        public void TestUnsupportedCollectionInterface()
        {
            var strs = new string[] { "a", "b", "c" };
            TestReadOnly(new IdentifierCollection(strs).AsCaseSensitiveCollection());
            TestReadOnly(new IdentifierCollection(strs).AsCaseInsensitiveCollection());
        }

        private void TestReadOnly(ICollection<string> collection)
        {
            Assert.Equal(true, collection.IsReadOnly);
            Assert.Throws<NotSupportedException>(() => collection.Add("x"));
            Assert.Throws<NotSupportedException>(() => collection.Remove("x"));
            Assert.Throws<NotSupportedException>(() => collection.Clear());
        }

        private void AssertContains(ICollection<string> collection, params string[] strings)
        {
            AssertContains(collection, (IEnumerable<string>)strings);
        }

        private void AssertContains(ICollection<string> collection, IEnumerable<string> strings)
        {
            foreach (var str in strings)
            {
                Assert.Equal(true, collection.Contains(str));
            }
        }

        private void AssertNotContains(ICollection<string> collection, params string[] strings)
        {
            AssertNotContains(collection, (IEnumerable<string>)strings);
        }

        private void AssertNotContains(ICollection<string> collection, IEnumerable<string> strings)
        {
            foreach (var str in strings)
            {
                Assert.Equal(false, collection.Contains(str));
            }
        }
    }
}
