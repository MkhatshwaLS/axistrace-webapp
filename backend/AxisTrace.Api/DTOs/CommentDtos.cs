using System.ComponentModel.DataAnnotations;

namespace AxisTrace.Api.DTOs
{
    public class CreateCommentDto
    {
        [Required]
        [MaxLength(2000)]
        public string Content { get; set; } = string.Empty;

        public int? ProjectId { get; set; }

        public int? MilestoneId { get; set; }

        public int? TaskId { get; set; }

        public int? ParentCommentId { get; set; }
    }

    public class UpdateCommentDto
    {
        [Required]
        [MaxLength(2000)]
        public string Content { get; set; } = string.Empty;
    }

    public class CommentDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int? ProjectId { get; set; }
        public string? ProjectName { get; set; }
        public int? MilestoneId { get; set; }
        public string? MilestoneName { get; set; }
        public int? TaskId { get; set; }
        public string? TaskName { get; set; }
        public int? ParentCommentId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public ICollection<CommentDto> Replies { get; set; } = new List<CommentDto>();
    }
}