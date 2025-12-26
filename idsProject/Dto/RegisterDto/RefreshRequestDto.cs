namespace idsProject.Dtos.Auth
{
    public class RefreshRequestDto
    {
        public string RefreshToken { get; set; } = null!;
    }

    public class LogoutRequestDto
    {
        public string RefreshToken { get; set; } = null!;

        public bool LogoutAllDevices { get; set; } = false;
    }
}