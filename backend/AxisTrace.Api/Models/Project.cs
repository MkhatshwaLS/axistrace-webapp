using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AxisTrace.Api.Models
{
    public class Project
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public int OwnerId { get; set; }

        [ForeignKey("OwnerId")]
        public User Owner { get; set; } = null!;

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? EstimatedCompletionDate { get; set; }

        [Required]
        public ProjectStatus Status { get; set; } = ProjectStatus.Planning;

        [MaxLength(500)]
        public string Location { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Budget { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public ICollection<Milestone> Milestones { get; set; } = new List<Milestone>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }

    public enum ProjectStatus
    {
        Planning,
        InProgress,
        OnHold,
        Completed,
        Cancelled
    }
}