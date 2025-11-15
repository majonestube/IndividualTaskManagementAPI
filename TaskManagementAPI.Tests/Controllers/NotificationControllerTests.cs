using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManagementAPI.Controllers;
using TaskManagementAPI.Models.DTO;
using TaskManagementAPI.Services;
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
    public async Task GetNotificationsForUser_ShouldReturnOk_WithNotifications()
    {
        // Arrange
        var userId = "user1";
        var notifications = new List<NotificationDto>
        {
            new NotificationDto { Message = "Notification 1", IsRead = false, Created = DateTime.Now },
            new NotificationDto { Message = "Notification 2", IsRead = true, Created = DateTime.Now }
        };

        _notificationServiceMock.Setup(x => x.GetNotificationsForUser(userId))
            .ReturnsAsync(notifications);

        // Act
        var result = await _controller.GetNotificationsForUser(userId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(notifications);
    }

    [Fact]
    public async Task GetNotificationsForUser_ShouldReturnBadRequest_WhenExceptionThrown()
    {
        // Arrange
        var userId = "user1";

        _notificationServiceMock.Setup(x => x.GetNotificationsForUser(userId))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.GetNotificationsForUser(userId);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be("Test exception");
    }

    [Fact]
    public async Task AddNotification_ShouldReturnOk_WhenNotificationCreated()
    {
        // Arrange
        var userId = "user1";
        var createDto = new NotificationCreateDto
        {
            ProjectId = 1,
            TaskItemId = 1,
            Message = "New notification"
        };
        var notification = new NotificationDto
        {
            Message = "New notification",
            IsRead = false,
            Created = DateTime.Now
        };

        _notificationServiceMock.Setup(x => x.AddNotification(
                createDto.ProjectId,
                createDto.TaskItemId,
                userId,
                createDto.Message))
            .ReturnsAsync(notification);

        // Act
        var result = await _controller.AddNotification(userId, createDto);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(notification);
    }

    [Fact]
    public async Task AddNotification_ShouldReturnBadRequest_WhenExceptionThrown()
    {
        // Arrange
        var userId = "user1";
        var createDto = new NotificationCreateDto
        {
            ProjectId = 1,
            TaskItemId = 1,
            Message = "New notification"
        };

        _notificationServiceMock.Setup(x => x.AddNotification(
                createDto.ProjectId,
                createDto.TaskItemId,
                userId,
                createDto.Message))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.AddNotification(userId, createDto);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be("Test exception");
    }

    [Fact]
    public async Task MarkAsRead_ShouldReturnNoContent_WhenSucceeds()
    {
        // Arrange
        var notificationId = 1;

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
        var notificationId = 999;

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
        var notificationId = 1;

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
        var notificationId = 999;

        _notificationServiceMock.Setup(x => x.MarkAsUnread(notificationId))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.MarkAsUnread(notificationId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }
}

