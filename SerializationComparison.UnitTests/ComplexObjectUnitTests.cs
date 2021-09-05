using System;
using System.Collections.Generic;
using Xunit;

namespace SerializationComparison.UnitTests
{
    public class ComplexObjectUnitTests
    {
        [Fact]
        public void Should_return_true_when_both_are_equals() => 
            Assert.Equal(GetComplexObject(), GetComplexObject());

        [Theory]
        [MemberData(nameof(GetNotEqualComplexObjects))]
        public void Should_return_false_when_both_are_not_equals(ComplexObject obj1, Func<ComplexObject> malversador) =>
            Assert.NotEqual(obj1, malversador());

        public static IEnumerable<object[]> GetNotEqualComplexObjects() =>
            new[]
            {
                new object[]
                {
                    GetComplexObject(),
                    (Func<ComplexObject>)(() => { var obj = GetComplexObject(); obj.Name = " DON'T TOUCH!"; return obj; })
                },
                new object[]
                {
                    GetComplexObject(),
                    (Func<ComplexObject>)(() => { var obj = GetComplexObject(); obj.Cookies = null; return obj; })
                },
                new object[]
                {
                    GetComplexObject(),
                    (Func<ComplexObject>)(() => { var obj = GetComplexObject(); obj.Cookies[0].Toppings = null; return obj; })
                },
                new object[]
                {
                    GetComplexObject(),
                    (Func<ComplexObject>)(() => { var obj = GetComplexObject(); ((ChocolateTopping)obj.Cookies[0].Toppings[0]).Origin = " -- Really!"; return obj; })
                },
                new object[]
                {
                    GetComplexObject(),
                    (Func<ComplexObject>)(() => { var obj = GetComplexObject(); ((PeanutTopping)obj.Cookies[0].Toppings[1]).Fat += 1; return obj; })
                },
                new object[]
                {
                    GetComplexObject(),
                    (Func<ComplexObject>)(() => { var obj = GetComplexObject(); ((SomethingGreenTopping)obj.Cookies[0].Toppings[2]).IsItSafe = !((SomethingGreenTopping)obj.Cookies[0].Toppings[2]).IsItSafe;  return obj; })
                }
            };

        private static ComplexObject GetComplexObject() => new()
        {
            Name = "Gordon's Jar",
            Cookies = new List<Cookie>
            {
                new Cookie
                {
                    Toppings = new List<Topping>
                    {
                        new ChocolateTopping
                        {
                            Origin = "Xen"
                        },
                        new PeanutTopping
                        {
                            Fat = 100
                        },
                        new SomethingGreenTopping
                        {
                            IsItSafe = false
                        }
                    }
                }
            }
        };
    }
}
