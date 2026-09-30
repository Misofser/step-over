using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoalApi.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkspaceToThought : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WorkspaceId",
                table: "Thoughts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Thoughts_WorkspaceId",
                table: "Thoughts",
                column: "WorkspaceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Thoughts_Workspaces_WorkspaceId",
                table: "Thoughts",
                column: "WorkspaceId",
                principalTable: "Workspaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Thoughts_Workspaces_WorkspaceId",
                table: "Thoughts");

            migrationBuilder.DropIndex(
                name: "IX_Thoughts_WorkspaceId",
                table: "Thoughts");

            migrationBuilder.DropColumn(
                name: "WorkspaceId",
                table: "Thoughts");
        }
    }
}
