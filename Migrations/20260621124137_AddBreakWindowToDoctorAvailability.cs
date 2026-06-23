using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PatientMS.Migrations
{
    /// <inheritdoc />
    public partial class AddBreakWindowToDoctorAvailability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Patient",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "BreakEnd",
                table: "DoctorAvailability",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "BreakStart",
                table: "DoctorAvailability",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BreakEnd",
                table: "DoctorAvailability");

            migrationBuilder.DropColumn(
                name: "BreakStart",
                table: "DoctorAvailability");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Patient",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");
        }
    }
}
