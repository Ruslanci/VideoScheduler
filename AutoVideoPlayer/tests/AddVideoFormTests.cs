using NUnit.Framework;
using System;

namespace AutoVideoPlayer
{
    [TestFixture]
    public class AddVideoFormTests
    {
        [Test]
        public void ValidateAndSetValues_ValidDateTimeAndPlays_ReturnsTrue()
        {
            var form = new AddVideoForm();
            var validDateTime = DateTime.Now.AddMinutes(10);
            var validPlays = 5;

            var result = form.ValidateAndSetValues(validDateTime, validPlays);

            Assert.Equals(result, true);
            Assert.Equals(validDateTime, form.ChosenTime);
            Assert.Equals(validPlays, form.NumberOfPlays);
        }

        [Test]
        public void ValidateAndSetValues_InvalidDateTime_ReturnsFalse()
        {
            var form = new AddVideoForm();
            var invalidDateTime = DateTime.Now.AddMinutes(-10);
            var validPlays = 5;

            var result = form.ValidateAndSetValues(invalidDateTime, validPlays);

            Assert.Equals(result, false);
        }

        [Test]
        public void ValidateAndSetValues_InvalidPlays_ReturnsFalse()
        {
            var form = new AddVideoForm();
            var validDateTime = DateTime.Now.AddMinutes(10);
            var invalidPlays = -1;

            var result = form.ValidateAndSetValues(validDateTime, invalidPlays);

            Assert.Equals(result, false);
        }
    }
}
