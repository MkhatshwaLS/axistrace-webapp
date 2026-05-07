using System.ComponentModel.DataAnnotations;
using AxisTrace.Api.Models;

namespace AxisTrace.Api.DTOs
{
    public class CreateMilestoneDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public int ProjectId { get; set; }

        public DateTime? DueDate { get; set; }

        public int Order { get; set; }
    }

    public class UpdateMilestoneDto
    {
        [MaxLength(200)]
        public string? Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }

        public MilestoneStatus? Status { get; set; }

        [Range(0, 100)]
        public int? ProgressPercentage { get; set; }

        public int? Order { get; set; }
    }

    public class MilestoneDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
        public DateTime? CompletedAt { get; set; }
        public MilestoneStatus Status { get; set; }
        public int ProgressPercentage { get; set; }
        public int Order { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
    }
}