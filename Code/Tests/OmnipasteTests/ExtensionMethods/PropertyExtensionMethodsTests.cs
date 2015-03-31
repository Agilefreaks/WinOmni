namespace OmnipasteTests.ExtensionMethods
{
    using FluentAssertions;
    using NUnit.Framework;
    using Omnipaste.ExtensionMethods;
    using Omnipaste.Models;
    using OmniUI.Entities;

    [TestFixture]
    public class PropertyExtensionMethodsTests
    {
        [Test]
        public void GetPropertyName_WhenObjectIsNull_ReturnsCorrectName()
        {
            var testModel = new TestModel();

            testModel.GetPropertyName(t => t.IsDeleted).Should().Be("IsDeleted");
        }

        #region Nested type: TestModel

        public class TestModel : Entity
        {
        }

        #endregion
    }
}