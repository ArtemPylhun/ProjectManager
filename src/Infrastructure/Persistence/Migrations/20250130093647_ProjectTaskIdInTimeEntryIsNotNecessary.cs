using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ProjectTaskIdInTimeEntryIsNotNecessary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_time_entries_project_tasks_project_task_id",
                table: "time_entries");

            migrationBuilder.AlterColumn<Guid>(
                name: "project_task_id",
                table: "time_entries",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "fk_time_entries_project_tasks_project_task_id",
                table: "time_entries",
                column: "project_task_id",
                principalTable: "project_tasks",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_time_entries_project_tasks_project_task_id",
                table: "time_entries");

            migrationBuilder.AlterColumn<Guid>(
                name: "project_task_id",
                table: "time_entries",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_time_entries_project_tasks_project_task_id",
                table: "time_entries",
                column: "project_task_id",
                principalTable: "project_tasks",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
