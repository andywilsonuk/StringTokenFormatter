using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace StringTokenFormatter.Tests
{
    public class ObjectPropertiesTokenValueContainerTests {
        [Fact]
        public void Property_Values_Are_Only_Got_Once() {
            var mockObject = new Mock<IMockPropertiesObject>();
            var mockMatchedToken = new Mock<IMatchedToken>();
            mockMatchedToken.SetupGet(x => x.Token).Returns("Prop1");
            var container = TokenValueContainer.FromObject(mockObject.Object, TokenParser.Default);

            bool result = container.TryMap(mockMatchedToken.Object, out object mapped);
            result = container.TryMap(mockMatchedToken.Object, out mapped);

            Assert.True(result);
            mockObject.Verify(x => x.Prop1, Times.Once);
        }


        public interface IMockPropertiesObject {
            int Prop1 { get; set; }
        }


        [Fact]
        public void Interface_Excludes_Class_Members() {
            var Test = new TestClass() {
                First = 1,
                Second = "Second"
            };

            var pattern = "{First} {Second}";
            var actual = pattern.FormatToken<IInterface1>(Test);
            var expected = "1 {Second}";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Derived_Interface_Includes_Base_Interfaces() {
            var Test = new DerivedClass() {
                First = 1,
                Second = "Second",
                Third = "Third"
            };

            var pattern = "{First} {Second} {Third}";
            var actual = pattern.FormatToken<IInterface3>(Test);
            var expected = "1 Second Third";

            Assert.Equal(expected, actual);
        }


        [Fact]
        public void Derived_Includes_Base_Members() {
            var Test = new DerivedClass() {
                First = 1,
                Second = "Second",
                Third = "Third"

            };

            var pattern = "{First} {Second} {Third}";
            var actual = pattern.FormatToken(Test);
            var expected = "1 Second Third";

            Assert.Equal(expected, actual);
        }


        private interface IInterface1 {
            int First { get; set; }
        }
        private interface IInterface2 {
            string Second { get; set; }
        }

        private interface IInterface3 : IInterface1, IInterface2 {
            string Third { get; set; }
        }


        public class TestClass : IInterface1 {
            public int First { get; set; }
            public string Second { get; set; }
        }

        public class DerivedClass : TestClass, IInterface3 {
            public string Third { get; set; }
        }
    }
}
