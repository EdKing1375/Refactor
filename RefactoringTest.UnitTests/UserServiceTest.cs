using LegacyApp;
using LegacyApp.Interfaces;
using Moq;
using System;
using Xunit;

namespace RefactoringTest.UnitTests;

public class UserServiceTest
{

    private readonly UserService _userService;
    private readonly Mock<IClientRepository> _mockClientRepository;
    private DateTime _olderPerson = new DateTime(1979, 12, 31);
    private DateTime _youngerPerson = new DateTime(2015, 12, 31);
    private DateTime _currentTime = new DateTime(2022, 01, 31);
    public UserServiceTest()
    {
        bool isTest = true;
        _mockClientRepository = new Mock<IClientRepository>();
        _userService = new UserService(_mockClientRepository.Object, isTest);
    }
    [Theory]
    [InlineData("", "", "test@test.com", false)]
    [InlineData("Test", "", "test@test.com", false)]
    [InlineData("", "Test", "test@test.com", false)]
    [InlineData("test", "Test", "test@test.com", true)]

    public void IsUserValid_ShouldCheckToSeeIfAUsersNameIsValid(string firname, string surname, string email, bool result)
    {
        UserService _userService1 = _userService;
        var isValidUser = _userService1.IsUserValid(firname, surname, email, _olderPerson, _currentTime);
        Assert.Equal(result, isValidUser);
    }

    [Theory]
    [InlineData("test", "test", "test@test.com", true)]
    [InlineData("Test", "test", "testtestcom", false)]

    public void IsUserValid_ShouldCheckToSeeIfAUsersEmailIsValid(string firname, string surname, string email, bool result)
    {
        UserService _userService1 = _userService;
        var isValidUser = _userService1.IsUserValid(firname, surname, email, _olderPerson, _currentTime);
        Assert.Equal(result, isValidUser);
    }
    [Fact]
    public void IsUserValid_ShouldReturnUserIsInValid_WhenAUserIsYoungerThan21()
    {
        UserService _userService1 = _userService;
        var isValidUser = _userService1.IsUserValid("test", "Test", "test@test.com", _youngerPerson, _currentTime);
        Assert.False(isValidUser);
    }
    [Fact]
    public void IsUserValid_ShouldReturnIsValid_WhenAUserIsYoungerThan21()
    {
        UserService _userService1 = _userService;
        var isValidUser = _userService1.IsUserValid("test", "Test", "test@test.com", _olderPerson, _currentTime);
        Assert.True(isValidUser);
    }

    [Theory]
    [InlineData("Bill", "Gates", "test@test.com", "ImportantClient", 1, true)]
    [InlineData("Joe", "Gates", "test@test.com", "NotThatImportant", 1, false)]
    [InlineData("Jeff", "Bezos", "test@test.com", "VeryImportantClient", 1, true)]
    public void AddUser_ShouldAddUserWhenUserHasAHighEnoughCreditLimit(string firname, string surname, string email, string clientName, int clientId, bool result)
    {
        var client = new Client
        {
            ClientStatus = ClientStatus.none,
            Id = clientId,
            Name = clientName
        };
        _mockClientRepository.Setup(m => m.GetById(It.IsAny<int>())).Returns(client);

        UserService _userService1 = _userService;

        var isUserAdded = _userService1.AddUser(firname, surname, email, _olderPerson, clientId);

        Assert.Equal(result, isUserAdded);
    }

}