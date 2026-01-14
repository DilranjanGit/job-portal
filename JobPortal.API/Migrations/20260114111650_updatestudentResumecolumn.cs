using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobPortal.API.Migrations
{
    /// <inheritdoc />
    public partial class updatestudentResumecolumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ResumeText",
                table: "Students",
                newName: "ResumeFileName");

            migrationBuilder.AddColumn<string>(
                name: "ResumeContentType",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "ResumeFile",
                table: "Students",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResumeUploadedAt",
                table: "Students",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResumeContentType",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "ResumeFile",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "ResumeUploadedAt",
                table: "Students");

            migrationBuilder.RenameColumn(
                name: "ResumeFileName",
                table: "Students",
                newName: "ResumeText");
        }
    }
}
