using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyShared.Models;
using TaskManagementAPI.Controllers;
using TaskManagementAPI.Services.AuthServices;
using Xunit;

namespace TaskManagementAPI.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _userManagerMock = new Mock<UserManager<IdentityUser>>(
            Mock.Of<IUserStore<IdentityUser>>(),
            null!, null!, null!, null!, null!, null!, null!, null!);
        _controller = new AuthController(_authServiceMock.Object, _userManagerMock.Object);
    }

    [Fact]
    public async Task RegisterUser_ShouldReturnConflict_WhenUserExists()
    {
        // Arrange
        var loginDto = new LoginDto { Username = "testuser", Password = "password123" };
        var existingUser = new IdentityUser { UserName = "testuser" };

        _userManagerMock.Setup(x => x.FindByNameAsync(loginDto.Username))
            .ReturnsAsync(existingUser);

        // Act
        var result = await _controller.RegisterUser(loginDto);

        // Assert
        result.Should().BeOfType<ConflictObjectResult>();
        var conflictResult = result as ConflictObjectResult;
        conflictResult!.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task RegisterUser_ShouldReturnOk_WhenRegistrationSucceeds()
    {
        // Arrange
        var loginDto = new LoginDto { Username = "testuser", Password = "password123" };

        _userManagerMock.Setup(x => x.FindByNameAsync(loginDto.Username))
            .ReturnsAsync((IdentityUser?)null);
        _authServiceMock.Setup(x => x.RegisterUser(loginDto))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.RegisterUser(loginDto);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task RegisterUser_ShouldReturnBadRequest_WhenRegistrationFails()
    {
        // Arrange
        var loginDto = new LoginDto { Username = "testuser", Password = "password123" };

        _userManagerMock.Setup(x => x.FindByNameAsync(loginDto.Username))
            .ReturnsAsync((IdentityUser?)null);
        _authServiceMock.Setup(x => x.RegisterUser(loginDto))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.RegisterUser(loginDto);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task LoginUser_ShouldReturnBadRequest_WhenUserNotFound()
    {
        // Arrange
        var loginDto = new LoginDto { Username = "testuser", Password = "password123" };

        _userManagerMock.Setup(x => x.FindByNameAsync(loginDto.Username))
            .ReturnsAsync((IdentityUser?)null);

        // Act
        var result = await _controller.LoginUser(loginDto);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task LoginUser_ShouldReturnBadRequest_WhenModelStateInvalid()
    {
        // Arrange
        var loginDto = new LoginDto { Username = "testuser", Password = "password123" };
        var existingUser = new IdentityUser { UserName = "testuser" };

        _userManagerMock.Setup(x => x.FindByNameAsync(loginDto.Username))
            .ReturnsAsync(existingUser);
        _controller.ModelState.AddModelError("Password", "Password is required");

        // Act
        var result = await _controller.LoginUser(loginDto);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task LoginUser_ShouldReturnBadRequest_WhenLoginFails()
    {
        // Arrange
        var loginDto = new LoginDto { Username = "testuser", Password = "password123" };
        var existingUser = new IdentityUser { UserName = "testuser" };

        _userManagerMock.Setup(x => x.FindByNameAsync(loginDto.Username))
            .ReturnsAsync(existingUser);
        _authServiceMock.Setup(x => x.LoginUser(loginDto))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.LoginUser(loginDto);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task LoginUser_ShouldReturnOkWithToken_WhenLoginSucceeds()
    {
        // Arrange
        var loginDto = new LoginDto { Username = "testuser", Password = "password123" };
        var existingUser = new IdentityUser { UserName = "testuser" };
        const string token = "test-token-string";

        _userManagerMock.Setup(x => x.FindByNameAsync(loginDto.Username))
            .ReturnsAsync(existingUser);
        _authServiceMock.Setup(x => x.LoginUser(loginDto))
            .ReturnsAsync(true);
        _authServiceMock.Setup(x => x.GenerateTokenString(existingUser))
            .ReturnsAsync(token);

        // Act
        var result = await _controller.LoginUser(loginDto);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().NotBeNull();
    }
}

