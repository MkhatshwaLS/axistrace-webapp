using System.ComponentModel.DataAnnotations;

namespace AxisTrace.Api.DTOs
{
    public class CreateProgressUpdateDto
    {
        [Required]
        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        public int? ProjectId { get; set; }

        public int? MilestoneId { get; set; }

        public int? TaskId { get; set; }

        [Range(0, 100)]
        public int? ProgressPercentage { get; set; }
    }

    public class ProgressUpdateDto
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int? ProjectId { get; set; }
        public string? ProjectName { get; set; }
        public int? MilestoneId { get; set; }
        public string? MilestoneName { get; set; }
        public int? TaskId { get; set; }
        public string? TaskName { get; set; }
        public int? ProgressPercentage { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}