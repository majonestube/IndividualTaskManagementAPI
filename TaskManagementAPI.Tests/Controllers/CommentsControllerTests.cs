using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManagementAPI.Controllers;
using TaskManagementAPI.Models.DTO;
using TaskManagementAPI.Services;
using TaskManagementAPI.Services.CommentServices;
using TaskManagementAPI.Tests.Helpers;
using Xunit;

namespace TaskManagementAPI.Tests.Controllers;

public class CommentsControllerTests
{
    private readonly Mock<ICommentService> _commentServiceMock;
    private readonly CommentsController _controller;

    public CommentsControllerTests()
    {
        _commentServiceMock = new Mock<ICommentService>();
        _controller = new CommentsController(_commentServiceMock.Object);
    }

    [Fact]
    public async Task GetForTask_ShouldReturnOk_WithComments()
    {
        // Arrange
        const int taskItemId = 1;
        const string userId = "user1";
        var comments = new List<CommentDto>
        {
            new CommentDto { Text = "Test comment 1", Username = "user1" },
            new CommentDto { Text = "Test comment 2", Username = "user2" }
        };

        ControllerTestHelpers.SetUserClaims(_controller, userId);
        _commentServiceMock.Setup(x => x.GetByTask(taskItemId, userId))
            .ReturnsAsync(comments);

        // Act
        var result = await _controller.GetByTask(taskItemId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(comments);
    }

    [Fact]
    public async Task GetByTask_ShouldReturnUnauthorized_WhenUserIdIsNull()
    {
        // Arrange
        const int taskItemId = 1;
        
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext
            {
                User = new System.Security.Claims.ClaimsPrincipal()
            }
        };

        // Act
        var result = await _controller.GetByTask(taskItemId);

        // Assert
        result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task GetByTask_ShouldReturnBadRequest_WhenExceptionThrown()
    {
        // Arrange
        const int taskItemId = 1;
        const string userId = "user1";

        ControllerTestHelpers.SetUserClaims(_controller, userId);
        _commentServiceMock.Setup(x => x.GetByTask(taskItemId, userId))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.GetByTask(taskItemId);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be("Test exception");
    }

    [Fact]
    public async Task GetById_ShouldReturnOk_WhenCommentExists()
    {
        // Arrange
        const int commentId = 1;
        var comment = new CommentDto { Text = "Test comment", Username = "user1" };

        _commentServiceMock.Setup(x => x.GetById(commentId))
            .ReturnsAsync(comment);

        // Act
        var result = await _controller.GetById(commentId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(comment);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenCommentDoesNotExist()
    {
        // Arrange
        const int commentId = 999;

        _commentServiceMock.Setup(x => x.GetById(commentId))
            .ReturnsAsync((CommentDto?)null);

        // Act
        var result = await _controller.GetById(commentId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenModelStateInvalid()
    {
        // Arrange
        const int taskId = 1;
        const string userId = "user1";
        var comment = new CommentCreateDto { Text = "" };

        ControllerTestHelpers.SetUserClaims(_controller, userId);
        _controller.ModelState.AddModelError("Text", "Text is required");

        // Act
        var result = await _controller.Create(taskId, comment);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Create_ShouldReturnOk_WhenCommentCreated()
    {
        // Arrange
        const int taskId = 1;
        const string userId = "user1";
        var comment = new CommentCreateDto { Text = "New comment" };

        ControllerTestHelpers.SetUserClaims(_controller, userId);
        _commentServiceMock.Setup(x => x.Create(userId, taskId, comment))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Create(taskId, comment);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(comment);
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenExceptionThrown()
    {
        // Arrange
        const int taskId = 1;
        const string userId = "user1";
        var comment = new CommentCreateDto { Text = "New comment" };

        ControllerTestHelpers.SetUserClaims(_controller, userId);
        _commentServiceMock.Setup(x => x.Create(userId, taskId, comment))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.Create(taskId, comment);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be("Test exception");
    }

    [Fact]
    public async Task Create_ShouldReturnUnauthorized_WhenUserIdIsNull()
    {
        // Arrange
        const int taskId = 1;
        var comment = new CommentCreateDto { Text = "New comment" };
        
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext
            {
                User = new System.Security.Claims.ClaimsPrincipal()
            }
        };

        // Act
        var result = await _controller.Create(taskId, comment);

        // Assert
        result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task Update_ShouldReturnBadRequest_WhenModelStateInvalid()
    {
        // Arrange
        const int commentId = 1;
        const string userId = "user1";
        var comment = new CommentDto { Id = commentId, Text = "", TaskItemId = 1, Username = "user1" };

        ControllerTestHelpers.SetUserClaims(_controller, userId);
        _controller.ModelState.AddModelError("Text", "Text is required");

        // Act
        var result = await _controller.Update(commentId, comment);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Update_ShouldReturnNoContent_WhenUpdateSucceeds()
    {
        // Arrange
        const int commentId = 1;
        const string userId = "user1";
        var comment = new CommentDto { Id = commentId, Text = "Updated comment", TaskItemId = 1, Username = "user1" };

        ControllerTestHelpers.SetUserClaims(_controller, userId);
        _commentServiceMock.Setup(x => x.Update(comment, userId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.Update(commentId, comment);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenCommentDoesNotExist()
    {
        // Arrange
        const int commentId = 999;
        const string userId = "user1";
        var comment = new CommentDto { Id = commentId, Text = "Updated comment", TaskItemId = 1, Username = "user1" };

        ControllerTestHelpers.SetUserClaims(_controller, userId);
        _commentServiceMock.Setup(x => x.Update(comment, userId))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Update(commentId, comment);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Update_ShouldReturnBadRequest_WhenExceptionThrown()
    {
        // Arrange
        const int commentId = 1;
        const string userId = "user1";
        var comment = new CommentDto { Id = commentId, Text = "Updated comment", TaskItemId = 1, Username = "user1" };

        ControllerTestHelpers.SetUserClaims(_controller, userId);
        _commentServiceMock.Setup(x => x.Update(comment, userId))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.Update(commentId, comment);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be("Test exception");
    }

    [Fact]
    public async Task Update_ShouldReturnUnauthorized_WhenUserIdIsNull()
    {
        // Arrange
        const int commentId = 1;
        var comment = new CommentDto { Id = commentId, Text = "Updated comment", TaskItemId = 1, Username = "user1" };
        
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext
            {
                User = new System.Security.Claims.ClaimsPrincipal()
            }
        };

        // Act
        var result = await _controller.Update(commentId, comment);

        // Assert
        result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent_WhenDeleteSucceeds()
    {
        // Arrange
        const int commentId = 1;
        const string userId = "user1";

        ControllerTestHelpers.SetUserClaims(_controller, userId);
        _commentServiceMock.Setup(x => x.Delete(commentId, userId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(commentId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenCommentDoesNotExist()
    {
        // Arrange
        const int commentId = 999;
        const string userId = "user1";

        ControllerTestHelpers.SetUserClaims(_controller, userId);
        _commentServiceMock.Setup(x => x.Delete(commentId, userId))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(commentId);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Delete_ShouldReturnUnauthorized_WhenUserIdIsNull()
    {
        // Arrange
        const int commentId = 1;
        
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext
            {
                User = new System.Security.Claims.ClaimsPrincipal()
            }
        };

        // Act
        var result = await _controller.Delete(commentId);

        // Assert
        result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task Delete_ShouldReturnBadRequest_WhenExceptionThrown()
    {
        // Arrange
        const int commentId = 1;
        const string userId = "user1";

        ControllerTestHelpers.SetUserClaims(_controller, userId);
        _commentServiceMock.Setup(x => x.Delete(commentId, userId))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.Delete(commentId);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be("Test exception");
    }
}

