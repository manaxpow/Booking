using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chapter10.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Update_Index : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_SeatBooking_Availability",
                table: "SeatBookings",
                columns: new[] { "ScheduleId", "IsBooking", "IsHold" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SeatBooking_Availability",
                table: "SeatBookings");
        }
    }
}
