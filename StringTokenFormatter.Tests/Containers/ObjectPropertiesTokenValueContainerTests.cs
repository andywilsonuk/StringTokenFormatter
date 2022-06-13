using Moq;
using Xunit;

namespace StringTokenFormatter.Tests {
    public partial class ObjectPropertiesTokenValueContainerTests {
        [Fact]
        public void Property_Values_Are_Only_Got_Once() {
            var mockObject = new Mock<IMockPropertiesObject>();
            var mockMatchedToken = new Mock<ITokenMatch>();
            mockMatchedToken.SetupGet(x => x.Token).Returns("Prop1");
            var container = TokenValueContainer.FromObject(mockObject.Object);

            var (result, _) = container.TryMap(mockMatchedToken.Object);
            (result, _) = container.TryMap(mockMatchedToken.Object);

            Assert.True(result);
            mockObject.Verify(x => x.Prop1, Times.Once);
        }


        public interface IMockPropertiesObject {
            int Prop1 { get; set; }
        }

        [Fact]
        public void Object_Uses_Generic_Formatter() {
            var Test = (object) new TestClass() {
                First = 1,
                Second = "Second"
            };

            var pattern = "{First} {Second}";
            var actual = pattern.FormatToken(Test);
            var expected = "1 Second";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Performance_Comparison_Between_Generic_and_Non_Generic() {
            var Test1 = new TestClass() {
                First = 1,
                Second = "Second"
            };

            var Test2 = (object)Test1;

            var pattern = "{First} {Second}";
            var actual1 = pattern.FormatToken(Test1);
            var actual2 = pattern.FormatToken(Test2);

            var sw1 = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < 100000; i++) {
                actual1 = pattern.FormatToken(Test1);
            }
            sw1.Stop();

            var sw2 = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < 100000; i++) {
                actual2 = pattern.FormatToken(Test2);
            }

            actual2.Equals(actual2);

            sw2.Stop();




            var expected = "1 Second";

            Assert.Equal(expected, actual1);
            Assert.Equal(expected, actual1);
        }





        [Fact]
        public void Interface_Excludes_Class_Members() {
            var Test = new TestClass() {
                First = 1,
                Second = "Second"
            };

            var pattern = "{First} {Second}";
            var actual = pattern.FormatToken<IFirst>(Test);
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
            var actual = pattern.FormatToken<IExtra>(Test);
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


        [Fact]
        public void AnonymousObject() {
            string pattern = "first {two} third";
            var tokenValues = new { two = "second" };

            string actual = pattern.FormatToken(tokenValues);

            string expected = "first second third";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AnonymousObjectCaseMatch() {
            string pattern = "first {two} third";
            var tokenValues = new { Two = "second" };

            string actual = pattern.FormatToken(tokenValues);

            string expected = "first second third";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AnonymousObjectWithCulture() {
            string pattern = "first {two} third";
            var tokenValues = new { two = "second" };

            var Settings = new InterpolationSettingsBuilder
            {
                TokenValueFormatter = TokenValueFormatters.CurrentCulture,
            }.Build();

            string actual = pattern.FormatToken(tokenValues, Settings);

            string expected = "first second third";
            Assert.Equal(expected, actual);
        }

    }
}
