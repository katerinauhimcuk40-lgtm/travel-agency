using Xunit;

using TravelAgency.Core.Common; 

namespace TravelAgency.Tests
{
    public class PassportUtilsTests
    {
        [Theory]
        [InlineData("AB123456", "AB****56")]
        [InlineData("123456789", "12*****89")]
        public void MaskPassport_ShouldMaskMiddleDigits(string input, string expected)
        {
            var result = PassportUtils.Mask(input);
            Assert.Equal(expected, result);
        }
    }
}