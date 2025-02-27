using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CascadeDeleteBehavioursAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_time_entries_project_tasks_project_task_id",
                table: "time_entries");

            migrationBuilder.AddForeignKey(
                name: "fk_time_entries_project_tasks_project_task_id",
                table: "time_entries",
                column: "project_task_id",
                principalTable: "project_tasks",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_time_entries_project_tasks_project_task_id",
                table: "time_entries");

            migrationBuilder.AddForeignKey(
                name: "fk_time_entries_project_tasks_project_task_id",
                table: "time_entries",
                column: "project_task_id",
                principalTable: "project_tasks",
                principalColumn: "id");
        }
    }
}
