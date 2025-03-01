using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ProjectTaskRemade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_tasks");

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "project_tasks",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "timezone('utc', now())");

            migrationBuilder.AddColumn<Guid>(
                name: "creator_id",
                table: "project_tasks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_project_tasks_creator_id",
                table: "project_tasks",
                column: "creator_id");

            migrationBuilder.AddForeignKey(
                name: "fk_project_tasks_users_creator_id",
                table: "project_tasks",
                column: "creator_id",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_project_tasks_users_creator_id",
                table: "project_tasks");

            migrationBuilder.DropIndex(
                name: "ix_project_tasks_creator_id",
                table: "project_tasks");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "project_tasks");

            migrationBuilder.DropColumn(
                name: "creator_id",
                table: "project_tasks");

            migrationBuilder.CreateTable(
                name: "user_tasks",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    project_task_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_tasks", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_tasks_project_tasks_project_task_id",
                        column: x => x.project_task_id,
                        principalTable: "project_tasks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_tasks_users_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_user_tasks_project_task_id",
                table: "user_tasks",
                column: "project_task_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_tasks_user_id",
                table: "user_tasks",
                column: "user_id");
        }
    }
}
