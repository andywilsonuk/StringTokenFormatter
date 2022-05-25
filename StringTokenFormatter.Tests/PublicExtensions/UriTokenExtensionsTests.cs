using System;
using System.Collections.Generic;
using Xunit;

namespace StringTokenFormatter.Tests
{
    public class UriTokenExtensionsTests
    {
        [Fact]
        public void Url_With_Single_Token_Is_Mapped_Successfully()
        {
            var input = new Uri("http://temp.org/{id}?id={id}");
            var expected = new Uri("http://temp.org/10?id=10");

            var actual = input.FormatToken("id", "10");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Url_With_Object_Dictionary_Of_Token_Is_Mapped_Successfully()
        {
            var input = new Uri("http://temp.org/{path}?id={id}");
            var expected = new Uri("http://temp.org/people?id=10");

            var actual = input.FormatDictionary(new Dictionary<string, object>
            {
                { "id" , 10},
                { "path", "people" }
            });

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Url_With_String_Dictionary_Of_Token_Is_Mapped_Successfully()
        {
            var input = new Uri("http://temp.org/{path}?id={id}");
            var expected = new Uri("http://temp.org/people?id=10");

            var actual = input.FormatDictionary(new Dictionary<string, string>
            {
                { "id" , "10"},
                { "path", "people" }
            });

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Url_With_Object_Token_Is_Mapped_Successfully()
        {
            var input = new Uri("http://temp.org/{id}?id={id}");
            var expected = new Uri("http://temp.org/10?id=10");
            var propertiesObject = new
            {
                id =  10,
                path = "people",
            };

            var actual = input.FormatToken(propertiesObject);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Relative_Url_With_Single_Token_Is_Mapped_Successfully()
        {
            var input = new Uri("/{id}", UriKind.Relative);
            var expected = new Uri("/10", UriKind.Relative);

            var actual = input.FormatToken("id", "10");

            Assert.Equal(expected, actual);
        }
    }
}
