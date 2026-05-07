using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AxisTrace.Api.Models
{
    public class ProgressUpdate
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        public int? ProjectId { get; set; }

        [ForeignKey("ProjectId")]
        public Project? Project { get; set; }

        public int? MilestoneId { get; set; }

        [ForeignKey("MilestoneId")]
        public Milestone? Milestone { get; set; }

        public int? TaskId { get; set; }

        [ForeignKey("TaskId")]
        public ProjectTask? Task { get; set; }

        [Range(0, 100)]
        public int? ProgressPercentage { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Ensure only one of ProjectId, MilestoneId, or TaskId is set
        // This can be enforced in the application logic or with a custom validator
    }
}