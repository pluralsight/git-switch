using GitSwitch;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class GravatarUrlBuilderTests
    {
        GravatarUrlBuilder classUnderTest;

        [SetUp]
        public void Setup()
        {
            classUnderTest = new GravatarUrlBuilder();
        }

        [Test]
        public void NormalizeEmail_TrimsSpaces()
        {
            var input = " test@example.com ";
            var output = classUnderTest.NormalizeEmail(input);

            Assert.That(output, Is.EqualTo("test@example.com"));
        }

        [Test]
        public void NormalizeEmail_ReturnsToLowerCase()
        {
            var input = "Test@Example.com";
            var output = classUnderTest.NormalizeEmail(input);

            Assert.That(output, Is.EqualTo("test@example.com"));
        }

        [Test]
        public void HashEmail_ReturnsMd5Hash()
        {
            var input = "test@example.com";
            var output = classUnderTest.HashEmail(input);

            Assert.That(output, Is.EqualTo("55502f40dc8b7c769880b10874abc9d0"));
        }

        [Test]
        public void GetUrlForEmail()
        {
            var input = " Test@Example.com ";
            var output = classUnderTest.GetUrlForEmail(input);

            Assert.That(output, Is.EqualTo(string.Format(AppConstants.GravatarUrlFormat, "55502f40dc8b7c769880b10874abc9d0")));
        }
    }
}
