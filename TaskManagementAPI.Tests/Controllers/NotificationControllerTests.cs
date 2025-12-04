using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyShared.Models;
using TaskManagementAPI.Controllers;
using TaskManagementAPI.Services.NotificationServices;
using TaskManagementAPI.Tests.Helpers;
using Xunit;

namespace TaskManagementAPI.Tests.Controllers;

public class NotificationControllerTests
{
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly NotificationController _controller;

    public NotificationControllerTests()
    {
        _notificationServiceMock = new Mock<INotificationService>();
        _controller = new NotificationController(_notificationServiceMock.Object);
    }

    [Fact]
    public async Task GetNotifications_ShouldReturnUnauthorized_WhenUserIsNull()
    {
        // arrange
        ControllerTestHelpers.ClearUser(_controller);

        var notification = new NotificationDto
        {
            Id = 1,
            ProjectId = 1,
            ProjectName = "Project",
            TaskItemId = 2,
            TaskName = "A Task",
            Message = "This is a notification",
            Created = default,
            IsRead = false
        };

        // act
        var result = await _controller.GetNotificationsForUser();
        
        // assert
        result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task GetNotifications_ShouldReturnOk_WhenUserIsNotNull()
    {
        // arrange
        var userId = "userId";
        ControllerTestHelpers.SetUserClaims(_controller, userId);

        var notifications = new List<NotificationDto> 
        {
            new NotificationDto
            {
                Id = 1,
                ProjectId = 1,
                ProjectName = "Project",
                TaskItemId = 2,
                TaskName = "A Task",
                Message = "This is a notification",
                Created = default,
                IsRead = false
            }
        };

        _notificationServiceMock.Setup(x => x.GetNotificationsForUser("userId")).ReturnsAsync(notifications);

        // act
        var result = await _controller.GetNotificationsForUser();
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetNotifications_ShouldReturnBadRequest_WhenExceptionIsThrown()
    {
        // arrange
        var userId = "userId";
        ControllerTestHelpers.SetUserClaims(_controller, userId);

        _notificationServiceMock.Setup(x => x.GetNotificationsForUser(userId))
            .ThrowsAsync(new Exception("Something went wrong!"));
        
        //act
        var result = await _controller.GetNotificationsForUser();
        
        //assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task AddNotification_ShouldReturnOk_WhenNotificationCreated()
    {
        // Arrange
        var userId = "user";
        ControllerTestHelpers.SetUserClaims(_controller, userId);
        var createDto = new NotificationCreateDto
        {
            ProjectId = 1,
            TaskItemId = 1,
            Message = "New notification"
        };

        _notificationServiceMock.Setup(x => x.AddNotification(
                createDto.ProjectId,
                createDto.TaskItemId,
                createDto.Message,
                userId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.AddNotification(createDto);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task AddNotification_ShouldReturnUnauthorized_WhenUserIsNull()
    {
        // arrange
        ControllerTestHelpers.ClearUser(_controller);
        var createDto = new NotificationCreateDto
        {
            ProjectId = 1,
            TaskItemId = 1,
            Message = "New notification"
        };
        
        // Act
        var result = await _controller.AddNotification(createDto);
        
        // Assert
        result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task AddNotification_ShouldReturnBadRequest_WhenExceptionThrown()
    {
        // Arrange
        var userId = "user";
        ControllerTestHelpers.SetUserClaims(_controller, userId);
        var createDto = new NotificationCreateDto
        {
            ProjectId = 1,
            TaskItemId = 1,
            Message = "New notification"
        };

        _notificationServiceMock.Setup(x => x.AddNotification(
                createDto.ProjectId,
                createDto.TaskItemId,
                createDto.Message,
                userId))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.AddNotification(createDto);

        // Assert
        result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task MarkAsRead_ShouldReturnNoContent_WhenSucceeds()
    {
        // Arrange
        const int notificationId = 1;

        _notificationServiceMock.Setup(x => x.MarkAsRead(notificationId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.MarkAsRead(notificationId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task MarkAsRead_ShouldReturnNotFound_WhenNotificationDoesNotExist()
    {
        // Arrange
        const int notificationId = 999;

        _notificationServiceMock.Setup(x => x.MarkAsRead(notificationId))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.MarkAsRead(notificationId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task MarkAsUnread_ShouldReturnNoContent_WhenSucceeds()
    {
        // Arrange
        const int notificationId = 1;

        _notificationServiceMock.Setup(x => x.MarkAsUnread(notificationId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.MarkAsUnread(notificationId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task MarkAsUnread_ShouldReturnNotFound_WhenNotificationDoesNotExist()
    {
        // Arrange
        const int notificationId = 999;

        _notificationServiceMock.Setup(x => x.MarkAsUnread(notificationId))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.MarkAsUnread(notificationId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }
}

