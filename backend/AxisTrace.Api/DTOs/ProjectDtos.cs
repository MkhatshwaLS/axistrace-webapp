using System.ComponentModel.DataAnnotations;
using AxisTrace.Api.Models;

namespace AxisTrace.Api.DTOs
{
    public class CreateProjectDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? EstimatedCompletionDate { get; set; }

        [MaxLength(500)]
        public string Location { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal Budget { get; set; }
    }

    public class UpdateProjectDto
    {
        [MaxLength(200)]
        public string? Name { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? EstimatedCompletionDate { get; set; }

        public ProjectStatus? Status { get; set; }

        [MaxLength(500)]
        public string? Location { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? Budget { get; set; }
    }

    public class ProjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int OwnerId { get; set; }
        public string OwnerName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? EstimatedCompletionDate { get; set; }
        public ProjectStatus Status { get; set; }
        public string Location { get; set; } = string.Empty;
        public decimal Budget { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int TotalMilestones { get; set; }
        public int CompletedMilestones { get; set; }
        public int ProgressPercentage { get; set; }
    }

    public class ProjectSummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ProjectStatus Status { get; set; }
        public DateTime? EndDate { get; set; }
        public int ProgressPercentage { get; set; }
    }
}