using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManagementAPI.Controllers;
using TaskManagementAPI.Models.DTO;
using TaskManagementAPI.Services;
using TaskManagementAPI.Services.UserServices;
using Xunit;

namespace TaskManagementAPI.Tests.Controllers;

public class UserControllerTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly UserController _controller;

    public UserControllerTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _controller = new UserController(_userServiceMock.Object);
    }

    [Fact]
    public async Task GetUsers_ShouldReturnOk_WithUsers()
    {
        // Arrange
        var users = new List<UserDto>
        {
            new UserDto { Id = "user1", Username = "user1", Email = "user1@test.com" },
            new UserDto { Id = "user2", Username = "user2", Email = "user2@test.com" }
        };

        _userServiceMock.Setup(x => x.GetUsers())
            .ReturnsAsync(users);

        // Act
        var result = await _controller.GetUsers();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(users);
    }

    [Fact]
    public async Task GetUser_ShouldReturnOk_WhenUserExists()
    {
        // Arrange
        var userId = "user1";
        var user = new UserDto { Id = userId, Username = "user1", Email = "user1@test.com" };

        _userServiceMock.Setup(x => x.GetUserById(userId))
            .ReturnsAsync(user);

        // Act
        var result = await _controller.GetUser(userId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(user);
    }

    [Fact]
    public async Task GetUser_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = "nonexistent";

        _userServiceMock.Setup(x => x.GetUserById(userId))
            .ReturnsAsync((UserDto?)null);

        // Act
        var result = await _controller.GetUser(userId);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task UpdateUser_ShouldReturnBadRequest_WhenModelStateInvalid()
    {
        // Arrange
        var userId = "user1";
        var updateDto = new UserUpdateDto { Email = "invalid-email" };

        _controller.ModelState.AddModelError("Email", "Email is invalid");

        // Act
        var result = await _controller.UpdateUser(userId, updateDto);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task UpdateUser_ShouldReturnOk_WhenUpdateSucceeds()
    {
        // Arrange
        var userId = "user1";
        var updateDto = new UserUpdateDto { Email = "updated@test.com" };
        var updatedUser = new UserDto { Id = userId, Username = "user1", Email = "updated@test.com" };

        _userServiceMock.Setup(x => x.Update(userId, updateDto))
            .ReturnsAsync(updatedUser);

        // Act
        var result = await _controller.UpdateUser(userId, updateDto);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(updatedUser);
    }

    [Fact]
    public async Task UpdateUser_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = "nonexistent";
        var updateDto = new UserUpdateDto { Email = "updated@test.com" };

        _userServiceMock.Setup(x => x.Update(userId, updateDto))
            .ReturnsAsync((UserDto?)null);

        // Act
        var result = await _controller.UpdateUser(userId, updateDto);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task UpdateUser_ShouldReturnBadRequest_WhenExceptionThrown()
    {
        // Arrange
        var userId = "user1";
        var updateDto = new UserUpdateDto { Email = "updated@test.com" };

        _userServiceMock.Setup(x => x.Update(userId, updateDto))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.UpdateUser(userId, updateDto);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be("Test exception");
    }

    [Fact]
    public async Task DeleteUser_ShouldReturnNoContent_WhenDeleteSucceeds()
    {
        // Arrange
        var userId = "user1";

        _userServiceMock.Setup(x => x.Delete(userId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteUser(userId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteUser_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = "nonexistent";

        _userServiceMock.Setup(x => x.Delete(userId))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteUser(userId);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }
}

