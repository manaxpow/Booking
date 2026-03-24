using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Chapter6.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cars",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    LicensePlate = table.Column<string>(
                        type: "character varying(20)",
                        maxLength: 20,
                        nullable: false
                    ),
                    Brand = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    Model = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    SeatCount = table.Column<int>(type: "integer", nullable: false),
                    Color = table.Column<string>(
                        type: "character varying(50)",
                        maxLength: 50,
                        nullable: true
                    ),
                    ManufactureYear = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cars", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Destinations",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    Name = table.Column<string>(
                        type: "character varying(120)",
                        maxLength: 120,
                        nullable: false
                    ),
                    Address = table.Column<string>(
                        type: "character varying(250)",
                        maxLength: 250,
                        nullable: true
                    ),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Destinations", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Drivers",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    FullName = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drivers", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Schedules",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    CarId = table.Column<int>(type: "integer", nullable: false),
                    DriverId = table.Column<int>(type: "integer", nullable: false),
                    PickupDestinationId = table.Column<int>(type: "integer", nullable: false),
                    DropoffDestinationId = table.Column<int>(type: "integer", nullable: false),
                    StartTime = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    EndTime = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<string>(
                        type: "character varying(20)",
                        maxLength: 20,
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Schedules_Cars_CarId",
                        column: x => x.CarId,
                        principalTable: "Cars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_Schedules_Destinations_DropoffDestinationId",
                        column: x => x.DropoffDestinationId,
                        principalTable: "Destinations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_Schedules_Destinations_PickupDestinationId",
                        column: x => x.PickupDestinationId,
                        principalTable: "Destinations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_Schedules_Drivers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "SeatBookings",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    ScheduleId = table.Column<int>(type: "integer", nullable: false),
                    SeatNumber = table.Column<int>(type: "integer", nullable: false),
                    IsBooking = table.Column<bool>(type: "boolean", nullable: false),
                    IsHold = table.Column<bool>(type: "boolean", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeatBookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SeatBookings_Schedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "Schedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Cars_LicensePlate",
                table: "Cars",
                column: "LicensePlate",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_Destinations_Name",
                table: "Destinations",
                column: "Name"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_FullName",
                table: "Drivers",
                column: "FullName"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_CarId",
                table: "Schedules",
                column: "CarId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_DriverId",
                table: "Schedules",
                column: "DriverId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_DropoffDestinationId",
                table: "Schedules",
                column: "DropoffDestinationId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_PickupDestinationId",
                table: "Schedules",
                column: "PickupDestinationId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_SeatBookings_ScheduleId_SeatNumber",
                table: "SeatBookings",
                columns: new[] { "ScheduleId", "SeatNumber" },
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "SeatBookings");

            migrationBuilder.DropTable(name: "Schedules");

            migrationBuilder.DropTable(name: "Cars");

            migrationBuilder.DropTable(name: "Destinations");

            migrationBuilder.DropTable(name: "Drivers");
        }
    }
}
