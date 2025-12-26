using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace idsProject.Migrations
{
    /// <inheritdoc />
    public partial class AddReplacedByTokenHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReplacedByToken",
                table: "RefreshTokens",
                newName: "ReplacedByTokenHash");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReplacedByTokenHash",
                table: "RefreshTokens",
                newName: "ReplacedByToken");
        }
    }
}
