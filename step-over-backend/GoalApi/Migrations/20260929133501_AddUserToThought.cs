using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoalApi.Migrations
{
    /// <inheritdoc />
    public partial class AddUserToThought : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Thoughts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Thoughts_UserId",
                table: "Thoughts",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Thoughts_Users_UserId",
                table: "Thoughts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Thoughts_Users_UserId",
                table: "Thoughts");

            migrationBuilder.DropIndex(
                name: "IX_Thoughts_UserId",
                table: "Thoughts");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Thoughts");
        }
    }
}
