using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManagementAPI.Controllers;
using TaskManagementAPI.Models.DTO;
using TaskManagementAPI.Services;
using Xunit;

namespace TaskManagementAPI.Tests.Controllers;

public class TasksControllerTests
{
    private readonly Mock<ITaskService> _taskServiceMock;
    private readonly TasksController _controller;

    public TasksControllerTests()
    {
        _taskServiceMock = new Mock<ITaskService>();
        _controller = new TasksController(_taskServiceMock.Object);
    }

    [Fact]
    public async Task GetForProject_ShouldReturnOk_WithTasks()
    {
        // Arrange
        var projectId = 1;
        var tasks = new List<TaskItemDto>
        {
            new TaskItemDto { Title = "Task 1", Description = "Description 1", Status = "ToDo" },
            new TaskItemDto { Title = "Task 2", Description = "Description 2", Status = "InProgress" }
        };

        _taskServiceMock.Setup(x => x.GetTasksForProject(projectId))
            .ReturnsAsync(tasks);

        // Act
        var result = await _controller.GetForProject(projectId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(tasks);
    }

    [Fact]
    public async Task GetById_ShouldReturnOk_WhenTaskExists()
    {
        // Arrange
        var taskId = 1;
        var task = new TaskItemDto { Title = "Task 1", Description = "Description 1", Status = "ToDo" };

        _taskServiceMock.Setup(x => x.GetById(taskId))
            .ReturnsAsync(task);

        // Act
        var result = await _controller.GetById(taskId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(task);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenTaskDoesNotExist()
    {
        // Arrange
        var taskId = 999;

        _taskServiceMock.Setup(x => x.GetById(taskId))
            .ReturnsAsync((TaskItemDto?)null);

        // Act
        var result = await _controller.GetById(taskId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenModelStateInvalid()
    {
        // Arrange
        var task = new TaskItemCreateDto { Title = "" };

        _controller.ModelState.AddModelError("Title", "Title is required");

        // Act
        var result = await _controller.Create(task);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Create_ShouldReturnOk_WhenTaskCreated()
    {
        // Arrange
        var task = new TaskItemCreateDto { Title = "New Task", Description = "Description" };

        _taskServiceMock.Setup(x => x.Create(task))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Create(task);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(task);
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenExceptionThrown()
    {
        // Arrange
        var task = new TaskItemCreateDto { Title = "New Task", Description = "Description" };

        _taskServiceMock.Setup(x => x.Create(task))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.Create(task);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be("Test exception");
    }

    [Fact]
    public async Task Update_ShouldReturnBadRequest_WhenModelStateInvalid()
    {
        // Arrange
        var taskId = 1;
        var task = new TaskItemCreateDto { Title = "" };

        _controller.ModelState.AddModelError("Title", "Title is required");

        // Act
        var result = await _controller.Update(taskId, task);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Update_ShouldReturnNoContent_WhenUpdateSucceeds()
    {
        // Arrange
        var taskId = 1;
        var task = new TaskItemCreateDto { Title = "Updated Task", Description = "Updated Description" };

        _taskServiceMock.Setup(x => x.Update(taskId, task))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.Update(taskId, task);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenTaskDoesNotExist()
    {
        // Arrange
        var taskId = 999;
        var task = new TaskItemCreateDto { Title = "Updated Task", Description = "Updated Description" };

        _taskServiceMock.Setup(x => x.Update(taskId, task))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Update(taskId, task);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Update_ShouldReturnBadRequest_WhenExceptionThrown()
    {
        // Arrange
        var taskId = 1;
        var task = new TaskItemCreateDto { Title = "Updated Task", Description = "Updated Description" };

        _taskServiceMock.Setup(x => x.Update(taskId, task))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.Update(taskId, task);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be("Test exception");
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent_WhenDeleteSucceeds()
    {
        // Arrange
        var taskId = 1;

        _taskServiceMock.Setup(x => x.Delete(taskId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(taskId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenTaskDoesNotExist()
    {
        // Arrange
        var taskId = 999;

        _taskServiceMock.Setup(x => x.Delete(taskId))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(taskId);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task UpdateStatus_ShouldReturnNoContent_WhenUpdateSucceeds()
    {
        // Arrange
        var taskId = 1;
        var statusId = 2;

        _taskServiceMock.Setup(x => x.UpdateStatus(taskId, statusId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.UpdateStatus(taskId, statusId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task UpdateStatus_ShouldReturnNotFound_WhenTaskDoesNotExist()
    {
        // Arrange
        var taskId = 999;
        var statusId = 2;

        _taskServiceMock.Setup(x => x.UpdateStatus(taskId, statusId))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.UpdateStatus(taskId, statusId);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task UpdateStatus_ShouldReturnBadRequest_WhenExceptionThrown()
    {
        // Arrange
        var taskId = 1;
        var statusId = 2;

        _taskServiceMock.Setup(x => x.UpdateStatus(taskId, statusId))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.UpdateStatus(taskId, statusId);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be("Test exception");
    }

    [Fact]
    public async Task AssignPossibleUsers_ShouldReturnOk_WithUsers()
    {
        // Arrange
        var taskId = 1;
        var users = new List<UserDto>
        {
            new UserDto { Id = "user1", Username = "user1", Email = "user1@test.com" },
            new UserDto { Id = "user2", Username = "user2", Email = "user2@test.com" }
        };

        _taskServiceMock.Setup(x => x.GetUsers(taskId))
            .ReturnsAsync(users);

        // Act
        var result = await _controller.AssignPossibleUsers(taskId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(users);
    }

    [Fact]
    public async Task AssignPossibleUsers_ShouldReturnBadRequest_WhenExceptionThrown()
    {
        // Arrange
        var taskId = 1;

        _taskServiceMock.Setup(x => x.GetUsers(taskId))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.AssignPossibleUsers(taskId);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be("Test exception");
    }

    [Fact]
    public async Task AssignUser_ShouldReturnNoContent_WhenAssignmentSucceeds()
    {
        // Arrange
        var taskId = 1;
        var userId = "user1";

        _taskServiceMock.Setup(x => x.AssignUser(taskId, userId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.AssignUser(taskId, userId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task AssignUser_ShouldReturnNotFound_WhenTaskDoesNotExist()
    {
        // Arrange
        var taskId = 999;
        var userId = "user1";

        _taskServiceMock.Setup(x => x.AssignUser(taskId, userId))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.AssignUser(taskId, userId);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task AssignUser_ShouldReturnBadRequest_WhenExceptionThrown()
    {
        // Arrange
        var taskId = 1;
        var userId = "user1";

        _taskServiceMock.Setup(x => x.AssignUser(taskId, userId))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.AssignUser(taskId, userId);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be("Test exception");
    }
}

