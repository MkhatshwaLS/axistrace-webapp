using System.ComponentModel.DataAnnotations;
using AxisTrace.Api.Models;
using TaskStatus = AxisTrace.Api.Models.TaskStatus;

namespace AxisTrace.Api.DTOs
{
    public class CreateProjectTaskDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public int MilestoneId { get; set; }

        public int? AssignedUserId { get; set; }

        public DateTime? DueDate { get; set; }

        public TaskPriority Priority { get; set; } = TaskPriority.Medium;

        public int EstimatedHours { get; set; }
    }

    public class UpdateProjectTaskDto
    {
        [MaxLength(200)]
        public string? Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public int? AssignedUserId { get; set; }

        public DateTime? DueDate { get; set; }

        public TaskStatus? Status { get; set; }

        public TaskPriority? Priority { get; set; }

        public int? EstimatedHours { get; set; }

        public int? ActualHours { get; set; }
    }

    public class ProjectTaskDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int MilestoneId { get; set; }
        public string MilestoneName { get; set; } = string.Empty;
        public int? AssignedUserId { get; set; }
        public string? AssignedUserName { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? CompletedAt { get; set; }
        public TaskStatus Status { get; set; }
        public TaskPriority Priority { get; set; }
        public int EstimatedHours { get; set; }
        public int ActualHours { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}