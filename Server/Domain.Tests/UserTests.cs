using FluentAssertions;

namespace Domain.Tests;

public class UserTests
{
    [Theory]
    [InlineData("asd2@asd.com")]
    [InlineData("asd@asd.com")]
    [InlineData("asd@asd.co.kr")]
    public void email_is_valid(string email)
    {
        User sut = new User();
        bool expectedResult = sut.Register(email);
        expectedResult.Should().BeTrue();
    }

    [Theory]
    [InlineData("asd2asd.com")]
    [InlineData("asd@asdcom")]
    public void email_is_not_valid(string email)
    {
        User sut = new User();
        bool expectedResult = sut.Register(email);
        expectedResult.Should().BeFalse();
    }
}
