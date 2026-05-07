using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AxisTrace.Api.Models
{
    public class Milestone
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public int ProjectId { get; set; }

        [ForeignKey("ProjectId")]
        public Project Project { get; set; } = null!;

        public DateTime? DueDate { get; set; }

        public DateTime? CompletedAt { get; set; }

        [Required]
        public MilestoneStatus Status { get; set; } = MilestoneStatus.NotStarted;

        [Range(0, 100)]
        public int ProgressPercentage { get; set; } = 0;

        public int Order { get; set; } // For ordering milestones within a project

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<ProgressUpdate> ProgressUpdates { get; set; } = new List<ProgressUpdate>();
    }

    public enum MilestoneStatus
    {
        NotStarted,
        InProgress,
        Completed,
        OnHold,
        Cancelled
    }
}