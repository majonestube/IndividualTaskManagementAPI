using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Models.Entities;

namespace TaskManagementAPI.Data;

public class TaskManagementDbContext(DbContextOptions<TaskManagementDbContext> options)
    : IdentityDbContext<IdentityUser>(options)
{
    public DbSet<Project> Projects { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<TaskItem> Tasks { get; set; }
    public DbSet<Status> Status { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<ProjectVisibility> ProjectVisibility { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // SEEDING PREPERATION 
        var hasher = new PasswordHasher<IdentityUser>();

        var adminUser = new IdentityUser
        {
            Id = "user-admin",
            UserName = "Admin",
            NormalizedUserName = "ADMIN",
            Email = "admin@example.com",
            NormalizedEmail = "ADMIN@EXAMPLE.COM",
            EmailConfirmed = true,
            PasswordHash = hasher.HashPassword(null, "AdminPassword123!"),
            SecurityStamp = string.Empty
        };
        
        var randomUser = new IdentityUser
        {
            Id = "user-random",
            UserName = "RandomUser",
            NormalizedUserName = "RANDOMUSER",
            Email = "randomuser@example.com",
            NormalizedEmail = "RANDOMUSER@EXAMPLE.COM",
            EmailConfirmed = true,
            PasswordHash = hasher.HashPassword(null, "RandomPassword123!"),
            SecurityStamp = string.Empty
        };
        
        var martyUser = new IdentityUser
        {
            Id = "user-marty",
            UserName = "Marty",
            NormalizedUserName = "MARTY",
            Email = "marty@example.com",
            NormalizedEmail = "MARTY@EXAMPLE.COM",
            EmailConfirmed = true,
            PasswordHash = hasher.HashPassword(null, "MartyPassword123!"),
            SecurityStamp = string.Empty
        };
        
        builder.Entity<Project>().ToTable("Projects");
        builder.Entity<TaskItem>().ToTable("Tasks");
        builder.Entity<Comment>().ToTable("Comments");
        builder.Entity<Status>().ToTable("Status");
        builder.Entity<Notification>().ToTable("Notifications");
        builder.Entity<ProjectVisibility>().ToTable("ProjectVisibility");
        
        // SEEDING
        
        // Users
        builder.Entity<IdentityUser>().HasData(adminUser, martyUser, randomUser);
        
        // Roles
        builder.Entity<IdentityRole>().HasData(
            new IdentityRole { Id = "role-admin", Name = "Admin", NormalizedName = "ADMIN" }
        );

        // Assign role to admin
        builder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string> { RoleId = "role-admin", UserId = "user-admin" }
        );
        
        // Projects
        builder.Entity<Project>().HasData(
            // for user 3
            new Project { Id = 1, Name = "Bachelor project", Description = "Plan for bachelor-project spring 26.", UserId = "user-marty"},
            // for user 2
            new Project {Id = 2, Name = "Fix up house", Description = "Plan for house remodel", UserId = "user-random"},
            new Project { Id = 3, Name = "Turn into Batman", Description = "Plan to become the Dark Knight", UserId = "user-random"}
        );
        
        // Project visibility
        builder.Entity<ProjectVisibility>().HasData(
            // All projects available to admin
            new ProjectVisibility { Id = 1, ProjectId = 1, UserId = "user-admin"},
            new ProjectVisibility { Id = 2, ProjectId = 2, UserId = "user-admin"},
            new ProjectVisibility { Id = 3, ProjectId = 3, UserId = "user-admin"},
            
            // Projects available to project owner 
            new ProjectVisibility { Id = 4, ProjectId = 1, UserId = "user-marty"},
            new ProjectVisibility { Id = 5, ProjectId = 2, UserId = "user-random"},
            new ProjectVisibility { Id = 6, ProjectId = 3, UserId = "user-random"}
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
            new TaskItem {Id = 1, ProjectId = 1, AssignedUserId = "user-marty", Title = "Have meeting with client", Description = "Before starting the project, define task parameters and expectations with the client.", StatusId = 2, DueDate = new DateTime(2025, 11, 7)},
            new TaskItem {Id = 2, ProjectId = 1, AssignedUserId = "user-marty", Title = "Planning meeting with student", Description = "A meeting between all students working on the project, to define roles and a plan of action.", StatusId = 1, DueDate = new DateTime(2026, 1, 10)},
            
            // For remodel project
            new TaskItem {Id = 3, ProjectId = 2, AssignedUserId = "user-random", Title = "Close doorframe", Description = "Finish closing the doorframe in the guest bedroom.", StatusId = 2, DueDate = new DateTime(2026, 1, 15)},
            new TaskItem {Id = 4, ProjectId = 2, AssignedUserId = "user-random", Title = "Paint wall", Description = "Paint the walls in the guest bedroom.", StatusId = 1, DueDate = new DateTime(2026, 1, 20)},
            
            // For Batman project
            new TaskItem {Id = 5, ProjectId = 3, AssignedUserId = "user-random", Title = "Become handsome", Description = "Become drop dead gorgeous", StatusId = 3, DueDate = new DateTime(2025, 12, 31)},
            new TaskItem {Id = 6, ProjectId = 3, AssignedUserId = "user-random", Title = "Become rich", Description = "Somehow, become very rich. Maybe rob a bank?", StatusId = 1, DueDate = new DateTime(2026, 3, 1)}
        );
        
        // Comments
        builder.Entity<Comment>().HasData(
            new Comment {Id = 1, TaskItemId = 5, UserId = "user-admin", Text = "This is stupid"}
        );
        
        // Notifications for user 1
        builder.Entity<Notification>().HasData(
            new Notification {Id = 1, Message = "A new person has commented", Created = new DateTime(2025, 11, 11), IsRead = false, ProjectId = 1, TaskItemId = 2, UserId = "user-marty"}
            );
        
        // Define foreign keys
        builder.Entity<Project>()
            .HasOne(p => p.User)
            .WithMany()
            .HasForeignKey(p => p.UserId);

        builder.Entity<TaskItem>()
            .HasOne(t => t.Project)
            .WithMany(p => p.Tasks)
            .HasForeignKey(t => t.ProjectId);

        builder.Entity<TaskItem>()
            .HasOne(t => t.AssignedUser)
            .WithMany()
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
            .WithMany()
            .HasForeignKey(c => c.UserId);
        
        builder.Entity<ProjectVisibility>()
            .HasOne(pv => pv.Project)
            .WithMany(p => p.ProjectVisibility)
            .HasForeignKey(pv => pv.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ProjectVisibility>()
            .HasOne(pv => pv.User)
            .WithMany()
            .HasForeignKey(pv => pv.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Entity<Notification>()
            .HasOne(n => n.Project)
            .WithMany(p => p.Notifications)
            .HasForeignKey(n => n.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Notification>()
            .HasOne(n => n.User)
            .WithMany()
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
