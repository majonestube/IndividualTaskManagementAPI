using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using TaskManagementAPI.Models.Entities;

namespace TaskManagementAPI;

public sealed class TaskManagementDataSeeder
{
    public static async Task SeedDataAsync(DbContext dbContext, CancellationToken cancellationToken = default)
    {
        SeedDataCore(dbContext);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public static void SeedData(DbContext dbContext)
    {
        SeedDataCore(dbContext);

        dbContext.SaveChanges();
    }

    private static void SeedDataCore(DbContext dbContext)
    {
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

        // Brukere
        dbContext.AddOrUpdate(adminUser, randomUser, martyUser);

        // Roller
        dbContext.AddOrUpdate(
            new IdentityRole { Id = "role-admin", Name = "Admin", NormalizedName = "ADMIN" }
        );

        // Tildel rolle til admin
        dbContext.AddOrUpdate(
            new IdentityUserRole<string> { RoleId = "role-admin", UserId = "user-admin" }
        );

        // Prosjekter
        dbContext.AddOrUpdate(
            // for bruker marty
            new Project { Id = 1, Name = "Bachelor project", Description = "Plan for bachelor-project spring 26.", UserId = "user-marty" },
            // for bruker random
            new Project { Id = 2, Name = "Fix up house", Description = "Plan for house remodel", UserId = "user-random" },
            new Project { Id = 3, Name = "Turn into Batman", Description = "Plan to become the Dark Knight", UserId = "user-random" }
        );

        // Prosjekt synlighet
        dbContext.AddOrUpdate(
            // Alle prosjekter synlig for admin
            new ProjectVisibility { Id = 1, ProjectId = 1, UserId = "user-admin" },
            new ProjectVisibility { Id = 2, ProjectId = 2, UserId = "user-admin" },
            new ProjectVisibility { Id = 3, ProjectId = 3, UserId = "user-admin" },

            // Alle prosjekter synlig for eier
            new ProjectVisibility { Id = 4, ProjectId = 1, UserId = "user-marty" },
            new ProjectVisibility { Id = 5, ProjectId = 2, UserId = "user-random" },
            new ProjectVisibility { Id = 6, ProjectId = 3, UserId = "user-random" }
        );

        // Status
        dbContext.AddOrUpdate(
            new Status { Id = 1, Name = "To Do" },
            new Status { Id = 2, Name = "In Progress" },
            new Status { Id = 3, Name = "Done" }
        );

        // TaskItems
        dbContext.AddOrUpdate(
            // For bachelor prosjekt
            new TaskItem { Id = 1, ProjectId = 1, AssignedUserId = "user-marty", Title = "Have meeting with client", Description = "Before starting the project, define task parameters and expectations with the client.", StatusId = 2, DueDate = new DateTime(2025, 11, 7) },
            new TaskItem { Id = 2, ProjectId = 1, AssignedUserId = "user-marty", Title = "Planning meeting with student", Description = "A meeting between all students working on the project, to define roles and a plan of action.", StatusId = 1, DueDate = new DateTime(2026, 1, 10) },

            // For oppussingsprosjekt
            new TaskItem { Id = 3, ProjectId = 2, AssignedUserId = "user-random", Title = "Close doorframe", Description = "Finish closing the doorframe in the guest bedroom.", StatusId = 2, DueDate = new DateTime(2026, 1, 15) },
            new TaskItem { Id = 4, ProjectId = 2, AssignedUserId = "user-random", Title = "Paint wall", Description = "Paint the walls in the guest bedroom.", StatusId = 1, DueDate = new DateTime(2026, 1, 20) },

            // For Batman prosjekt
            new TaskItem { Id = 5, ProjectId = 3, AssignedUserId = "user-random", Title = "Become handsome", Description = "Become drop dead gorgeous", StatusId = 3, DueDate = new DateTime(2025, 12, 31) },
            new TaskItem { Id = 6, ProjectId = 3, AssignedUserId = "user-random", Title = "Become rich", Description = "Somehow, become very rich. Maybe rob a bank?", StatusId = 1, DueDate = new DateTime(2026, 3, 1) }
        );

        // Kommentarer
        dbContext.AddOrUpdate(
            new Comment { Id = 1, TaskItemId = 5, UserId = "user-admin", Text = "This is stupid" }
        );

        // Varsler for bruker marty
        dbContext.AddOrUpdate(
            new Notification { Id = 1, Message = "A new person has commented", Created = new DateTime(2025, 11, 11), IsRead = false, ProjectId = 1, TaskItemId = 2, UserId = "user-marty" }
        );
    }
}