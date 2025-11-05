using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Models.Entities;

namespace TaskManagementAPI.Data;

public class TaskManagementDbContext(DbContextOptions<TaskManagementDbContext> options) : DbContext(options)
{
    public DbSet<Project> Projects { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<TaskItem> Tasks { get; set; }
    public DbSet<Status> Status { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.Entity<Project>().ToTable("Projects");
        builder.Entity<TaskItem>().ToTable("Tasks");
        builder.Entity<Comment>().ToTable("Comments");
        builder.Entity<User>().ToTable("Users");
        builder.Entity<Status>().ToTable("Status");
        
        // SEEDING
        
        // Users
        builder.Entity<User>().HasData(
            new User { Id = 1, Username = "admin", Password = "AdminPassword123!", Email = "admin@example.com"},
            new User {Id = 2, Username = "RandomUser", Password = "RandomPassword123!", Email = "random@example.com"},
            new User {Id = 3, Username = "Martine", Password = "MartinePassword123!", Email = "martine@example.com"}
        );
        
        // Projects
        builder.Entity<Project>().HasData(
            // for user 3
            new Project { Id = 1, Name = "Bachelor project", Description = "Plan for bachelor-project spring 26.", UserId = 3},
            // for user 2
            new Project {Id = 2, Name = "Fix up house", Description = "Plan for house remodel", UserId = 2},
            new Project { Id = 3, Name = "Turn into Batman", Description = "Plan to become the Dark Knight", UserId = 2}
        );
        
        // Status
        builder.Entity<Status>().HasData(
            new Status {Id = 1, Name = "To Do"},
            new Status {Id = 2, Name = "In Progress"},
            new Status {Id = 3, Name = "Done"}
        );
        
        // TaskItems
        builder.Entity<TaskItem>().HasData(
            // For bachelor project
            new TaskItem {Id = 1, ProjectId = 1, AssignedUserId = 3, Title = "Have meeting with client", Description = "Before starting the project, define task parameters and expectations with the client.", StatusId = 2, DueDate = new DateTime(2025, 11, 7)},
            new TaskItem {Id = 2, ProjectId = 1, AssignedUserId = 3, Title = "Planning meeting with student", Description = "A meeting between all students working on the project, to define roles and a plan of action.", StatusId = 1, DueDate = new DateTime(2026, 1, 10)},
            
            // For remodel project
            new TaskItem {Id = 3, ProjectId = 2, AssignedUserId = 2, Title = "Close doorframe", Description = "Finish closing the doorframe in the guest bedroom.", StatusId = 2, DueDate = new DateTime(2026, 1, 15)},
            new TaskItem {Id = 4, ProjectId = 2, AssignedUserId = 2, Title = "Paint wall", Description = "Paint the walls in the guest bedroom.", StatusId = 1, DueDate = new DateTime(2026, 1, 20)},
            
            // For Batman project
            new TaskItem {Id = 5, ProjectId = 3, AssignedUserId = 2, Title = "Become handsome", Description = "Become drop dead gorgeous", StatusId = 3, DueDate = new DateTime(2025, 12, 31)},
            new TaskItem {Id = 6, ProjectId = 3, AssignedUserId = 2, Title = "Become rich", Description = "Somehow, become very rich. Maybe rob a bank?", StatusId = 1, DueDate = new DateTime(2026, 3, 1)}
        );
        
        // Comments
        builder.Entity<Comment>().HasData(
            new Comment {Id = 1, TaskItemId = 5, UserId = 1, Text = "This is stupid"}
        );
        
        // Define foreign keys
        builder.Entity<Project>()
            .HasOne(p => p.User)
            .WithMany(u => u.Projects)
            .HasForeignKey(p => p.UserId);

        builder.Entity<TaskItem>()
            .HasOne(t => t.Project)
            .WithMany(p => p.Tasks)
            .HasForeignKey(t => t.ProjectId);

        builder.Entity<TaskItem>()
            .HasOne(t => t.AssignedUser)
            .WithMany(u => u.Tasks)
            .HasForeignKey(t => t.AssignedUserId);

        builder.Entity<TaskItem>()
            .HasOne(t => t.Status)
            .WithMany(s => s.Tasks)
            .HasForeignKey(t => t.StatusId);

        builder.Entity<Comment>()
            .HasOne(c => c.TaskItem)
            .WithMany(t => t.Comments)
            .HasForeignKey(c => c.TaskItemId);

        builder.Entity<Comment>()
            .HasOne(c => c.User)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.UserId);
    }
}
