using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace StringTokenFormatter.Tests
{
    public class ObjectPropertiesTokenValueContainerTests
    {
        [Fact]
        public void Property_Values_Are_Only_Got_Once()
        {
            var mockObject = new Mock<IMockPropertiesObject>();
            var mockMatchedToken = new Mock<IMatchedToken>();
            mockMatchedToken.SetupGet(x => x.Token).Returns("Prop1");
            var container = new ObjectPropertiesTokenValueContainer(mockObject.Object, TokenReplacer.DefaultMatcher);

            bool result = container.TryMap(mockMatchedToken.Object, out object mapped);
            result = container.TryMap(mockMatchedToken.Object, out mapped);

            Assert.True(result);
            mockObject.Verify(x => x.Prop1, Times.Once);
        }
    }

    public interface IMockPropertiesObject
    {
        int Prop1 { get; set; }
    }
}
