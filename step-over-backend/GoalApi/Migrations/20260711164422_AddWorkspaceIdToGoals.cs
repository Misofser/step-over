using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoalApi.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkspaceIdToGoals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WorkspaceId",
                table: "Goals",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Goals_WorkspaceId",
                table: "Goals",
                column: "WorkspaceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Goals_Workspaces_WorkspaceId",
                table: "Goals",
                column: "WorkspaceId",
                principalTable: "Workspaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Goals_Workspaces_WorkspaceId",
                table: "Goals");

            migrationBuilder.DropIndex(
                name: "IX_Goals_WorkspaceId",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "WorkspaceId",
                table: "Goals");
        }
    }
}
