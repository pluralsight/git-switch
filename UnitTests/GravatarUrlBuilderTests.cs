using GitSwitch;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class GravatarUrlBuilderTests
    {
        private GravatarUrlBuilder ClassUnderTest;

        [SetUp]
        public void Setup()
        {
            ClassUnderTest = new GravatarUrlBuilder();
        }

        [Test]
        public void NormalizedEmailsContainNoSpaces()
        {
            var input = " test@example.com ";

            var output = ClassUnderTest.NormalizeEmail(input);

            Assert.That(output, Is.EqualTo("test@example.com"));
        }

        [Test]
        public void NormalizedEmailsAreLowerCase()
        {
            var input = "Test@Example.com";

            var output = ClassUnderTest.NormalizeEmail(input);

            Assert.That(output, Is.EqualTo("test@example.com"));
        }


        [Test]
        public void EmailsAreMd5Hashed()
        {
            var input = "test@example.com";

            var output = ClassUnderTest.HashEmail(input);

            Assert.That(output, Is.EqualTo("55502f40dc8b7c769880b10874abc9d0"));
        }

        [Test]
        public void UrlsAreProperlyFormatted()
        {
            var input = " Test@Example.com ";

            var output = ClassUnderTest.GetUrlForEmail(input);

            Assert.That(output, Is.EqualTo("http://gravatar.com/avatar/55502f40dc8b7c769880b10874abc9d0?s=64&r=g"));
        }
    }
}
