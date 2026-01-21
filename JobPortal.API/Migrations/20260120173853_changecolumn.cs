using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobPortal.API.Migrations
{
    /// <inheritdoc />
    public partial class changecolumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isActive",
                table: "Students",
                newName: "InActive");

            migrationBuilder.RenameColumn(
                name: "isActive",
                table: "Companies",
                newName: "InActive");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InActive",
                table: "Students",
                newName: "isActive");

            migrationBuilder.RenameColumn(
                name: "InActive",
                table: "Companies",
                newName: "isActive");
        }
    }
}
