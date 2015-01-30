namespace OmnipasteTests.ExtensionMethods
{
    using FluentAssertions;
    using NUnit.Framework;
    using Omnipaste.ExtensionMethods;
    using Omnipaste.Models;

    [TestFixture]
    public class PropertyExtensionMethodsTests
    {
        [Test]
        public void GetPropertyName_WhenObjectIsNull_ReturnsCorrectName()
        {
            var testModel = new BaseModel();
            
            testModel.GetPropertyName(t => t.IsDeleted).Should().Be("IsDeleted");
        }
    }
}
