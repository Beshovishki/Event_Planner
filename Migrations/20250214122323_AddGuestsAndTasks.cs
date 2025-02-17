using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventPlanner.Migrations
{
    /// <inheritdoc />
    public partial class AddGuestsAndTasks : Migration
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

            migrationBuilder.AlterColumn<string>(
                name: "RSVPStatus",
                table: "Guests",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "GuestName",
                table: "Guests",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "EventID",
                table: "Guests",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Guests",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EventPlace",
                table: "Events",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Events",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

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

            migrationBuilder.AlterColumn<string>(
                name: "RSVPStatus",
                table: "Guests",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "GuestName",
                table: "Guests",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "EventID",
                table: "Guests",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Guests",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "EventPlace",
                table: "Events",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Events",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

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
