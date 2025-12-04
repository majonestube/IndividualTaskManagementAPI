using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyShared.Models;
using TaskManagementAPI.Controllers;
using TaskManagementAPI.Services.TaskServices;
using TaskManagementAPI.Tests.Helpers;
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
        const int projectId = 1;
        const string userId = "user1";
        var tasks = new List<TaskItemDto>
        {
            new TaskItemDto { Title = "Task 1", Description = "Description 1", Status = "ToDo" },
            new TaskItemDto { Title = "Task 2", Description = "Description 2", Status = "InProgress" }
        };

        ControllerTestHelpers.SetUserClaims(_controller, userId);
        _taskServiceMock.Setup(x => x.GetTasksForProject(projectId, userId))
            .ReturnsAsync(tasks);

        // Act
        var result = await _controller.GetForProject(projectId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(tasks);
    }

    [Fact]
    public async Task GetForProject_ShouldReturnUnauthorized_WhenUserIdIsNull()
    {
        // Arrange
        const int projectId = 1;
        
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext
            {
                User = new System.Security.Claims.ClaimsPrincipal()
            }
        };

        // Act
        var result = await _controller.GetForProject(projectId);

        // Assert
        result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task GetForProject_ShouldReturnBadRequest_WhenExceptionThrown()
    {
        // Arrange
        const int projectId = 1;
        const string userId = "user1";

        ControllerTestHelpers.SetUserClaims(_controller, userId);
        _taskServiceMock.Setup(x => x.GetTasksForProject(projectId, userId))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.GetForProject(projectId);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be("Test exception");
    }

    [Fact]
    public async Task GetById_ShouldReturnOk_WhenTaskExists()
    {
        // Arrange
        const int taskId = 1;
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
        const int taskId = 999;

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
        const string userId = "user1";
        var task = new TaskItemCreateDto { Title = "New Task", Description = "Description" };

        ControllerTestHelpers.SetUserClaims(_controller, userId);
        _taskServiceMock.Setup(x => x.Create(task, userId))
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
        const string userId = "user1";
        var task = new TaskItemCreateDto { Title = "New Task", Description = "Description" };

        ControllerTestHelpers.SetUserClaims(_controller, userId);
        _taskServiceMock.Setup(x => x.Create(task, userId))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.Create(task);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be("Test exception");
    }

    [Fact]
    public async Task Create_ShouldReturnUnauthorized_WhenUserIdIsNull()
    {
        // Arrange
        var task = new TaskItemCreateDto { Title = "New Task", Description = "Description" };
        
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext
            {
                User = new System.Security.Claims.ClaimsPrincipal()
            }
        };

        // Act
        var result = await _controller.Create(task);

        // Assert
        result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task Update_ShouldReturnBadRequest_WhenModelStateInvalid()
    {
        // Arrange
        const int taskId = 1;
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
        const int taskId = 1;
        const string userId = "user1";
        var task = new TaskItemCreateDto { Title = "Updated Task", Description = "Updated Description" };

        ControllerTestHelpers.SetUserClaims(_controller, userId);
        _taskServiceMock.Setup(x => x.Update(taskId, task, userId))
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
        const int taskId = 999;
        const string userId = "user1";
        var task = new TaskItemCreateDto { Title = "Updated Task", Description = "Updated Description" };

        ControllerTestHelpers.SetUserClaims(_controller, userId);
        _taskServiceMock.Setup(x => x.Update(taskId, task, userId))
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
        const int taskId = 1;
        const string userId = "user1";
        var task = new TaskItemCreateDto { Title = "Updated Task", Description = "Updated Description" };

        ControllerTestHelpers.SetUserClaims(_controller, userId);
        _taskServiceMock.Setup(x => x.Update(taskId, task, userId))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.Update(taskId, task);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be("Test exception");
    }

    [Fact]
    public async Task Update_ShouldReturnUnauthorized_WhenUserIdIsNull()
    {
        // Arrange
        const int taskId = 1;
        var task = new TaskItemCreateDto { Title = "Updated Task", Description = "Updated Description" };
        
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext
            {
                User = new System.Security.Claims.ClaimsPrincipal()
            }
        };

        // Act
        var result = await _controller.Update(taskId, task);

        // Assert
        result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent_WhenDeleteSucceeds()
    {
        // Arrange
        const int taskId = 1;
        const string userId = "user1";

        ControllerTestHelpers.SetUserClaims(_controller, userId);
        _taskServiceMock.Setup(x => x.Delete(taskId, userId))
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
        const int taskId = 999;
        const string userId = "user1";

        ControllerTestHelpers.SetUserClaims(_controller, userId);
        _taskServiceMock.Setup(x => x.Delete(taskId, userId))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(taskId);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Delete_ShouldReturnUnauthorized_WhenUserIdIsNull()
    {
        // Arrange
        const int taskId = 1;
        
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext
            {
                User = new System.Security.Claims.ClaimsPrincipal()
            }
        };

        // Act
        var result = await _controller.Delete(taskId);

        // Assert
        result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task Delete_ShouldReturnBadRequest_WhenExceptionThrown()
    {
        // Arrange
        const int taskId = 1;
        const string userId = "user1";

        ControllerTestHelpers.SetUserClaims(_controller, userId);
        _taskServiceMock.Setup(x => x.Delete(taskId, userId))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.Delete(taskId);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be("Test exception");
    }
    
    [Fact]
    public async Task GetStatuses_ShouldReturnUnauthorized_IfUserIsNull()
    {
        // arrange
        ControllerTestHelpers.ClearUser(_controller);
        
        // act
        var result = await _controller.GetStatuses();
        
        //assert
        result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task GetStatuses_ShouldReturnOk_IfUserIsNotNull()
    {
        // arrange
        var userId = "user1";
        ControllerTestHelpers.SetUserClaims(_controller, userId);

        var statuses = new List<StatusDto>
        {
            new StatusDto { Id = 1, Name = "Success" },
        };
        
        _taskServiceMock.Setup(x => x.GetStatuses())
            .ReturnsAsync(statuses);
        
        // act
        var result = await _controller.GetStatuses();
        
        // assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetStatuses_ShouldReturnBadRequest_IfExceptionThrown()
    {
        // arrange
        var userId = "user1";
        ControllerTestHelpers.SetUserClaims(_controller, userId);
        
        _taskServiceMock.Setup(x => x.GetStatuses())
            .ThrowsAsync(new Exception("Test exception"));
        
        // act
        var result = await _controller.GetStatuses();
        
        // assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }
    
    
    [Fact]
    public async Task UpdateStatus_ShouldReturnNoContent_WhenUpdateSucceeds()
    {
        // Arrange
        const int taskId = 1;
        const int statusId = 2;
        const string userId = "user1";

        ControllerTestHelpers.SetUserClaims(_controller, userId);
        _taskServiceMock.Setup(x => x.UpdateStatus(taskId, statusId, userId))
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
        const int taskId = 999;
        const int statusId = 2;
        const string userId = "user1";

        ControllerTestHelpers.SetUserClaims(_controller, userId);
        _taskServiceMock.Setup(x => x.UpdateStatus(taskId, statusId, userId))
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
        const int taskId = 1;
        const int statusId = 2;
        const string userId = "user1";

        ControllerTestHelpers.SetUserClaims(_controller, userId);
        _taskServiceMock.Setup(x => x.UpdateStatus(taskId, statusId, userId))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.UpdateStatus(taskId, statusId);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be("Test exception");
    }

    [Fact]
    public async Task UpdateStatus_ShouldReturnUnauthorized_WhenUserIdIsNull()
    {
        // Arrange
        const int taskId = 1;
        const int statusId = 2;
        
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext
            {
                User = new System.Security.Claims.ClaimsPrincipal()
            }
        };

        // Act
        var result = await _controller.UpdateStatus(taskId, statusId);

        // Assert
        result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task AssignPossibleUsers_ShouldReturnOk_WithUsers()
    {
        // Arrange
        const int taskId = 1;
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
        const int taskId = 1;

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
        const int taskId = 1;
        const string userId = "user1";

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
        const int taskId = 999;
        const string userId = "user1";

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
        const int taskId = 1;
        const string userId = "user1";

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

