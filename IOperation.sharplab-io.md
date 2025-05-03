
view on [Sharplab.io](https://sharplab.io/#v2:https://sharplab.io/#v2:C4LghgzsA0AmIGoA+ABATARgLACgUAYACFDAFgG5cDiMA6AGQEsA7AR0ryIDEAbAVwCmzYAEEIEAQCdgjAPbMIHagFlGAY0myIsgGbBaANUYQ+YHgGVgfWHNoAVAVDuzZPCLQCqzRsAdQWAOYcVADMxGiEfsAAwjyQEIQA3riEqYQpaQDaUcoCwAAWsrAAuhmpKGEopITRrjwCajLy5oV8PLC1wmAs5oH1ABQAlGVJI2mEAPQThGCSkmDMAQJjaQBus4RqdQ1NzIQAvITMAgDuhExQADwswAB8SYQYhAC+HOPjK6nrkoQ8LKwHTbbRpyZifd60ADq+SkAn6AA8DvdEfcAKzDHDvLGEexgADWcIw+Ax2IhABFjDJmI0huDxrQAPKSWBSWmY0lpWjmPgAWyGwXZWKmhFcsBm4ikuxA4K2PHqIPktFqfGEQy5rXaaoAQoTBm9sbL5bslbIVfoWqbNYNaDr+hg9TLgcaLW1YGqABJgVYCZWq+0C0nC45neKS0HSwXvQ07UHqy1u62dYDdZi9RYDB2R564bM4XDMMA8xwABzAagEhF4gmEYgk0lBCVwyUjFRoADZwoQAAquACeOkYcqrQlEEvr8ggI2bpNbJA7ADleVJ1LWwxPrsJ7i7NQVjIQbjNGqYeAYzIJBkjCAVNGdg4R57JgABJHnF+pF4QCWAAUXh5eLuz8uCs4YB2ADiQjLmotRyjG8iruOCiXHYW4arAyG3P0u4JCQIQYYeVhmKe/ACBe+z3Nesi3qc96Pi+b4CB+wBfr+/6AZmwGVBEi5FpIK5jrsEAYeC04cuUlTVLaB4CPCxY7F+F6JC84K5jOXGEBBxx8dBTqgghgnCZG4yiWJs7VJ63q+sA/TSbJ8kdKawiKcpRnYmZNTyMmPR9HCzmqdi/mpLmuYFkWECluWhCqBoWi6PoRgmGYljWLYUTOK47heD4USBE2IyZCIwDAHxABGfDMR4EBgEs/SFcVjBlcxdizEswDuLkBRFIMpQtupOR5IUsB1aV5UViAhDDQ1o0PMFQA===)

```cs
using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

public class TestClass {
    
    [TestMethod]
    public void CollectionShouldContainSingle()
    {
        // arrange
        var collection = new List<int> { 1 };
        
        var linq = collection
            .Where(x => x > 5)
            .Take(10)
            .Distinct()
            .Order()
            .Sum();

        // old assertion:
        collection.Count().Should().Be(1);
        collection.Count.Should().Be(1);
        collection.Should().HaveCount(1);

        // new assertion:
        collection.Should().ContainSingle();
    }
}

namespace FluentAssertions 
{
    public static class PolyfillFluentAssertions
    {
        public static NumericAssertions<int> Should(this int actualValue) => throw new NotImplementedException();
        public static GenericCollectionAssertions<T> Should<T>(this IEnumerable<T> actualValue) => throw new NotImplementedException();

        public class NumericAssertions<T>
        {
            public void Be(int expected) { }
        }
        public class GenericCollectionAssertions<T>
        {
            public void HaveCount(int expectedCount) { }
            public void ContainSingle() { }
        }
    }
}
namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
    [AttributeUsage(AttributeTargets.Method)]
    public class TestMethodAttribute : Attribute { }
}
```