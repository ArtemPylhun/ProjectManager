using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UniqueConfigForProjectUsersAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_project_users_user_id",
                table: "project_users");

            migrationBuilder.CreateIndex(
                name: "ix_project_users_user_id_project_id_role_id",
                table: "project_users",
                columns: new[] { "user_id", "project_id", "role_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_project_users_user_id_project_id_role_id",
                table: "project_users");

            migrationBuilder.CreateIndex(
                name: "ix_project_users_user_id",
                table: "project_users",
                column: "user_id");
        }
    }
}
