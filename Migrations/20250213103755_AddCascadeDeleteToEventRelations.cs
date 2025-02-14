using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventPlanner.Migrations
{
    /// <inheritdoc />
    public partial class AddCascadeDeleteToEventRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventTasks_Events_EventID",
                table: "EventTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Guests_Events_EventID",
                table: "Guests");

            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Events_EventID",
                table: "Ratings");

            migrationBuilder.AddForeignKey(
                name: "FK_EventTasks_Events_EventID",
                table: "EventTasks",
                column: "EventID",
                principalTable: "Events",
                principalColumn: "EventID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Guests_Events_EventID",
                table: "Guests",
                column: "EventID",
                principalTable: "Events",
                principalColumn: "EventID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_Events_EventID",
                table: "Ratings",
                column: "EventID",
                principalTable: "Events",
                principalColumn: "EventID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventTasks_Events_EventID",
                table: "EventTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Guests_Events_EventID",
                table: "Guests");

            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Events_EventID",
                table: "Ratings");

            migrationBuilder.AddForeignKey(
                name: "FK_EventTasks_Events_EventID",
                table: "EventTasks",
                column: "EventID",
                principalTable: "Events",
                principalColumn: "EventID");

            migrationBuilder.AddForeignKey(
                name: "FK_Guests_Events_EventID",
                table: "Guests",
                column: "EventID",
                principalTable: "Events",
                principalColumn: "EventID");

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_Events_EventID",
                table: "Ratings",
                column: "EventID",
                principalTable: "Events",
                principalColumn: "EventID");
        }
    }
}
