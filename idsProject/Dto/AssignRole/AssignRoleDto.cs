namespace idsProject.Dto.AssignRole
{
    public class AssignRoleDto
    {
        public string Role { get; set; } = null!;
        public bool ReplaceExistingRoles { get; set; } = false; // optional
    }
}
