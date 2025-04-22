 public class ReportUserRequest
    {
        [Required]
        public Guid TargetId { get; set; }

        [Required]
        public Guid ReporterId { get; set; }

        [Required]
        public ReportTypeEnum ReportType { get; set; }

        public string? Description { get; set; }
    }