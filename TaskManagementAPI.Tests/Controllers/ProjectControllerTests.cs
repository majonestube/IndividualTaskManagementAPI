using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyShared.Models;
using TaskManagementAPI.Controllers;
using TaskManagementAPI.Models.DTO;
using TaskManagementAPI.Services.ProjectServices;
using TaskManagementAPI.Tests.Helpers;
using Xunit;

namespace TaskManagementAPI.Tests.Controllers;

public class ProjectControllerTests
{
    private readonly Mock<IProjectService> _projectServiceMock;
    private readonly ProjectController _controller;

    public ProjectControllerTests()
    {
        _projectServiceMock = new Mock<IProjectService>();
        _controller = new ProjectController(_projectServiceMock.Object);
    }

    [Fact]
    public async Task GetAllProjects_ShouldReturnOk_WithProjects()
    {
        // Arrange
        const string userId = "admin1";
        var projects = new List<ProjectDto>
        {
            new ProjectDto { Name = "Project 1", Description = "Description 1", Username = "user1" },
            new ProjectDto { Name = "Project 2", Description = "Description 2", Username = "user2" }
        };

        ControllerTestHelpers.SetUserClaimsWithAdminRole(_controller, userId);
        _projectServiceMock.Setup(x => x.GetAllProjects())
            .ReturnsAsync(projects);

        // Act
        var result = await _controller.GetAllProjects();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(projects);
    }

    [Fact]
    public async Task GetAllProjects_ShouldReturnUnauthorized_WhenUserIdIsNull()
    {
        // Arrange
        _controller.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
        {
            HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext
            {
                User = new System.Security.Claims.ClaimsPrincipal()
            }
        };

        // Act
        var result = await _controller.GetAllProjects();

        // Assert
        result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task GetAllProjects_ShouldReturnBadRequest_WhenExceptionThrown()
    {
        // Arrange
        const string userId = "admin1";

        ControllerTestHelpers.SetUserClaimsWithAdminRole(_controller, userId);
        _projectServiceMock.Setup(x => x.GetAllProjects())
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.GetAllProjects();

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be("Test exception");
    }

    [Fact]
    public async Task GetVisibleProjects_ShouldReturnOk_WithProjects()
    {
        // Arrange
        const string userId = "user1";
        var projects = new List<ProjectDto>
        {
            new ProjectDto { Name = "Project 1", Description = "Description 1", Username = "user1" },
            new ProjectDto { Name = "Project 2", Description = "Description 2", Username = "user2" }
        };

        ControllerTestHelpers.SetUserClaims(_controller, userId);
        _projectServiceMock.Setup(x => x.GetAllVisibleProjects(userId))
            .ReturnsAsync(projects);

        // Act
        var result = await _controller.GetVisibleProjects();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(projects);
    }

    [Fact]
    public async Task GetVisibleProjects_ShouldReturnUnauthorized_WhenUserIdIsNull()
    {
        // Arrange
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext
            {
                User = new System.Security.Claims.ClaimsPrincipal()
            }
        };

        // Act
        var result = await _controller.GetVisibleProjects();

        // Assert
        result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task GetVisibleProjects_ShouldReturnBadRequest_WhenExceptionThrown()
    {
        // Arrange
        const string userId = "user1";

        ControllerTestHelpers.SetUserClaims(_controller, userId);
        _projectServiceMock.Setup(x => x.GetAllVisibleProjects(userId))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.GetVisibleProjects();

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be("Test exception");
    }

    [Fact]
    public async Task GetForUser_ShouldReturnOk_WithProjects()
    {
        // Arrange
        const string userId = "user1";
        var projects = new List<ProjectDto>
        {
            new ProjectDto { Name = "Project 1", Description = "Description 1", Username = "user1" }
        };

        ControllerTestHelpers.SetUserClaims(_controller, userId);
        _projectServiceMock.Setup(x => x.GetProjectsForUser(userId))
            .ReturnsAsync(projects);

        // Act
        var result = await _controller.GetForUser();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(projects);
    }

    [Fact]
    public async Task GetForUser_ShouldReturnUnauthorized_WhenUserIdIsNull()
    {
        // Arrange
        _controller.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
        {
            HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext
            {
                User = new System.Security.Claims.ClaimsPrincipal()
            }
        };

        // Act
        var result = await _controller.GetForUser();

        // Assert
        result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task GetForUser_ShouldReturnBadRequest_WhenExceptionThrown()
    {
        // Arrange
        const string userId = "user1";

        ControllerTestHelpers.SetUserClaims(_controller, userId);
        _projectServiceMock.Setup(x => x.GetProjectsForUser(userId))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.GetForUser();

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be("Test exception");
    }

    [Fact]
    public async Task GetById_ShouldReturnOk_WhenProjectExists()
    {
        // Arrange
        const int projectId = 1;
        var project = new ProjectDto { Name = "Project 1", Description = "Description 1", Username = "user1" };

        _projectServiceMock.Setup(x => x.GetById(projectId))
            .ReturnsAsync(project);

        // Act
        var result = await _controller.GetById(projectId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(project);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenProjectDoesNotExist()
    {
        // Arrange
        const int projectId = 999;

        _projectServiceMock.Setup(x => x.GetById(projectId))
            .ReturnsAsync((ProjectDto?)null);

        // Act
        var result = await _controller.GetById(projectId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenModelStateInvalid()
    {
        // Arrange
        var project = new ProjectCreateDto { Name = "" };

        _controller.ModelState.AddModelError("Name", "Name is required");

        // Act
        var result = await _controller.Create(project);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Create_ShouldReturnOk_WhenProjectCreated()
    {
        // Arrange
        var project = new ProjectCreateDto { Name = "New Project", Description = "Description" };
        var createdProject = new ProjectDto { Name = "New Project", Description = "Description", Username = "user1" };

        _projectServiceMock.Setup(x => x.Create(project))
            .ReturnsAsync(createdProject);

        // Act
        var result = await _controller.Create(project);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(project);
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenExceptionThrown()
    {
        // Arrange
        var project = new ProjectCreateDto { Name = "New Project", Description = "Description" };

        _projectServiceMock.Setup(x => x.Create(project))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.Create(project);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be("Test exception");
    }

    [Fact]
    public async Task Update_ShouldReturnBadRequest_WhenModelStateInvalid()
    {
        // Arrange
        const int projectId = 1;
        var project = new ProjectCreateDto { Name = "" };

        _controller.ModelState.AddModelError("Name", "Name is required");

        // Act
        var result = await _controller.Update(projectId, project);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Update_ShouldReturnNoContent_WhenUpdateSucceeds()
    {
        // Arrange
        const int projectId = 1;
        const string userId = "user1";
        var project = new ProjectCreateDto { Name = "Updated Project", Description = "Updated Description" };

        ControllerTestHelpers.SetUserClaims(_controller, userId);
        _projectServiceMock.Setup(x => x.Update(projectId, project, userId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.Update(projectId, project);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenProjectDoesNotExist()
    {
        // Arrange
        const int projectId = 999;
        const string userId = "user1";
        var project = new ProjectCreateDto { Name = "Updated Project", Description = "Updated Description" };

        ControllerTestHelpers.SetUserClaims(_controller, userId);
        _projectServiceMock.Setup(x => x.Update(projectId, project, userId))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Update(projectId, project);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Update_ShouldReturnBadRequest_WhenExceptionThrown()
    {
        // Arrange
        const int projectId = 1;
        const string userId = "user1";
        var project = new ProjectCreateDto { Name = "Updated Project", Description = "Updated Description" };

        ControllerTestHelpers.SetUserClaims(_controller, userId);
        _projectServiceMock.Setup(x => x.Update(projectId, project, userId))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.Update(projectId, project);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be("Test exception");
    }

    [Fact]
    public async Task Update_ShouldReturnUnauthorized_WhenUserIdIsNull()
    {
        // Arrange
        const int projectId = 1;
        var project = new ProjectCreateDto { Name = "Updated Project", Description = "Updated Description" };
        
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext
            {
                User = new System.Security.Claims.ClaimsPrincipal()
            }
        };

        // Act
        var result = await _controller.Update(projectId, project);

        // Assert
        result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent_WhenDeleteSucceeds()
    {
        // Arrange
        const int projectId = 1;
        const string userId = "user1";

        ControllerTestHelpers.SetUserClaims(_controller, userId);
        _projectServiceMock.Setup(x => x.Delete(projectId, userId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(projectId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenProjectDoesNotExist()
    {
        // Arrange
        const int projectId = 999;
        const string userId = "user1";

        ControllerTestHelpers.SetUserClaims(_controller, userId);
        _projectServiceMock.Setup(x => x.Delete(projectId, userId))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(projectId);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Delete_ShouldReturnUnauthorized_WhenUserIdIsNull()
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
        var result = await _controller.Delete(projectId);

        // Assert
        result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task ShareProject_ShouldReturnOk_WhenShareSucceeds()
    {
        // Arrange
        const int projectId = 1;
        const string userId = "user1";
        const string sharedUserId = "user2";
        var projectShare = new ProjectShareDto { sharedUserId = sharedUserId };

        ControllerTestHelpers.SetUserClaims(_controller, userId);
        _projectServiceMock.Setup(x => x.ShareProject(projectId, userId, sharedUserId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.ShareProject(projectId, projectShare);

        // Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task ShareProject_ShouldReturnNotFound_WhenProjectDoesNotExist()
    {
        // Arrange
        const int projectId = 999;
        const string userId = "user1";
        const string sharedUserId = "user2";
        var projectShare = new ProjectShareDto { sharedUserId = sharedUserId };

        ControllerTestHelpers.SetUserClaims(_controller, userId);
        _projectServiceMock.Setup(x => x.ShareProject(projectId, userId, sharedUserId))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.ShareProject(projectId, projectShare);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task ShareProject_ShouldReturnUnauthorized_WhenUserIdIsNull()
    {
        // Arrange
        const int projectId = 1;
        const string sharedUserId = "user2";
        var projectShare = new ProjectShareDto { sharedUserId = sharedUserId };
        
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext
            {
                User = new System.Security.Claims.ClaimsPrincipal()
            }
        };

        // Act
        var result = await _controller.ShareProject(projectId, projectShare);

        // Assert
        result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task ShareProject_ShouldReturnBadRequest_WhenExceptionThrown()
    {
        // Arrange
        const int projectId = 1;
        const string userId = "user1";
        const string sharedUserId = "user2";
        var projectShare = new ProjectShareDto { sharedUserId = sharedUserId };

        ControllerTestHelpers.SetUserClaims(_controller, userId);
        _projectServiceMock.Setup(x => x.ShareProject(projectId, userId, sharedUserId))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.ShareProject(projectId, projectShare);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be("Test exception");
    }
}

