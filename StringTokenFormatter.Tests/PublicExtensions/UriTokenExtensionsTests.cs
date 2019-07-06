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
            Uri input = new Uri("http://temp.org/{id}?id={id}");
            Uri expected = new Uri("http://temp.org/10?id=10");

            Uri actual = input.FormatToken("id", "10");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Url_With_Object_Dictionary_Of_Token_Is_Mapped_Successfully()
        {
            Uri input = new Uri("http://temp.org/{path}?id={id}");
            Uri expected = new Uri("http://temp.org/people?id=10");

            Uri actual = input.FormatDictionary(new Dictionary<string, object>
            {
                { "id" , 10},
                { "path", "people" }
            });

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Url_With_String_Dictionary_Of_Token_Is_Mapped_Successfully()
        {
            Uri input = new Uri("http://temp.org/{path}?id={id}");
            Uri expected = new Uri("http://temp.org/people?id=10");

            Uri actual = input.FormatDictionary(new Dictionary<string, string>
            {
                { "id" , "10"},
                { "path", "people" }
            });

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Url_With_Object_Token_Is_Mapped_Successfully()
        {
            Uri input = new Uri("http://temp.org/{id}?id={id}");
            Uri expected = new Uri("http://temp.org/10?id=10");
            var propertiesObject = new
            {
                id =  10,
                path = "people",
            };

            Uri actual = input.FormatToken(propertiesObject);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Relative_Url_With_Single_Token_Is_Mapped_Successfully()
        {
            Uri input = new Uri("/{id}", UriKind.Relative);
            Uri expected = new Uri("/10", UriKind.Relative);

            Uri actual = input.FormatToken("id", "10");

            Assert.Equal(expected, actual);
        }
    }
}
