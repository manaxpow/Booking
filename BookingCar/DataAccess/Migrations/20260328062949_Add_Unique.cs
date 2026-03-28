using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Add_Unique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SeatBookings_ScheduleId",
                table: "SeatBookings");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Phone",
                table: "Users",
                column: "Phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SeatBookings_ScheduleId_SeatId",
                table: "SeatBookings",
                columns: new[] { "ScheduleId", "SeatId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_StartTime_FromDestinationId_ToDestinationId",
                table: "Schedules",
                columns: new[] { "StartTime", "FromDestinationId", "ToDestinationId" });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_Status",
                table: "Payments",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Phone",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_SeatBookings_ScheduleId_SeatId",
                table: "SeatBookings");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_StartTime_FromDestinationId_ToDestinationId",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Payments_Status",
                table: "Payments");

            migrationBuilder.CreateIndex(
                name: "IX_SeatBookings_ScheduleId",
                table: "SeatBookings",
                column: "ScheduleId");
        }
    }
}
