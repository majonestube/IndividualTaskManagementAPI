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
        
        builder.Entity<Project>().ToTable("Projects");
        builder.Entity<TaskItem>().ToTable("Tasks");
        builder.Entity<Comment>().ToTable("Comments");
        builder.Entity<Status>().ToTable("Status");
        builder.Entity<Notification>().ToTable("Notifications");
        builder.Entity<ProjectVisibility>().ToTable("ProjectVisibility");
        
        // Definer foreign keys
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
