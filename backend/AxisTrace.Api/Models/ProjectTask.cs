using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AxisTrace.Api.Models
{
    public class ProjectTask
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public int MilestoneId { get; set; }

        [ForeignKey("MilestoneId")]
        public Milestone Milestone { get; set; } = null!;

        public int? AssignedUserId { get; set; }

        [ForeignKey("AssignedUserId")]
        public User? AssignedUser { get; set; }

        public DateTime? DueDate { get; set; }

        public DateTime? CompletedAt { get; set; }

        [Required]
        public TaskStatus Status { get; set; } = TaskStatus.NotStarted;

        [Required]
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;

        public int EstimatedHours { get; set; }

        public int ActualHours { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<ProgressUpdate> ProgressUpdates { get; set; } = new List<ProgressUpdate>();
    }

    public enum TaskStatus
    {
        NotStarted,
        InProgress,
        InReview,
        Completed,
        OnHold,
        Cancelled
    }

    public enum TaskPriority
    {
        Low,
        Medium,
        High,
        Critical
    }
}