namespace idsProject.Dtos.Certificate
{
    public class CertificateResponseDto
    {
        public int Id { get; set; }
        public string DownloadUrl { get; set; } = null!;
        public int CourseId { get; set; }
        public string UserId { get; set; } = null!;
        public DateTime GeneratedAt { get; set; }
    }
}
