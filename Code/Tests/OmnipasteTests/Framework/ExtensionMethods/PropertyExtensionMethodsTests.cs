namespace OmnipasteTests.Framework.ExtensionMethods
{
    using FluentAssertions;
    using NUnit.Framework;
    using Omnipaste.Framework.ExtensionMethods;
    using OmniUI.Framework.Entities;

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