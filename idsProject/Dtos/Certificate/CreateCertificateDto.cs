namespace idsProject.Dtos.Certificate
{
    public class CreateCertificateDto
    {
        public string DownloadUrl { get; set; } = null!;
        public int CourseId { get; set; }
        public int UserId { get; set; }
    }
}
