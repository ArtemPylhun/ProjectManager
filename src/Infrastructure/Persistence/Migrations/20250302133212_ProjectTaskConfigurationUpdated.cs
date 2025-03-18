using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ProjectTaskConfigurationUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_project_tasks_users_creator_id",
                table: "project_tasks");

            migrationBuilder.AddForeignKey(
                name: "fk_project_tasks_users_creator_id",
                table: "project_tasks",
                column: "creator_id",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_project_tasks_users_creator_id",
                table: "project_tasks");

            migrationBuilder.AddForeignKey(
                name: "fk_project_tasks_users_creator_id",
                table: "project_tasks",
                column: "creator_id",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
