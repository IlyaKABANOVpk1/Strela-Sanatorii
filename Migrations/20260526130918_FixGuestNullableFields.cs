using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Strela_Sanatorii.Migrations
{
    /// <inheritdoc />
    public partial class FixGuestNullableFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Snils",
                table: "Guests",
                newName: "snils");

            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "Guests",
                newName: "phone");

            migrationBuilder.RenameColumn(
                name: "Gender",
                table: "Guests",
                newName: "gender");

            migrationBuilder.RenameColumn(
                name: "Contraindications",
                table: "Guests",
                newName: "contraindications");

            migrationBuilder.RenameColumn(
                name: "Allergies",
                table: "Guests",
                newName: "allergies");

            migrationBuilder.RenameColumn(
                name: "PersonnelNumber",
                table: "Guests",
                newName: "personnel_number");

            migrationBuilder.RenameColumn(
                name: "PassportSeries",
                table: "Guests",
                newName: "passport_series");

            migrationBuilder.RenameColumn(
                name: "PassportNumber",
                table: "Guests",
                newName: "passport_number");

            migrationBuilder.RenameColumn(
                name: "EmergencyContactPhone",
                table: "Guests",
                newName: "emergency_contact_phone");

            migrationBuilder.RenameColumn(
                name: "EmergencyContactName",
                table: "Guests",
                newName: "emergency_contact_name");

            migrationBuilder.RenameColumn(
                name: "BirthDate",
                table: "Guests",
                newName: "birth_date");

            migrationBuilder.RenameIndex(
                name: "IX_Guests_PersonnelNumber",
                table: "Guests",
                newName: "IX_Guests_personnel_number");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "snils",
                table: "Guests",
                newName: "Snils");

            migrationBuilder.RenameColumn(
                name: "phone",
                table: "Guests",
                newName: "Phone");

            migrationBuilder.RenameColumn(
                name: "gender",
                table: "Guests",
                newName: "Gender");

            migrationBuilder.RenameColumn(
                name: "contraindications",
                table: "Guests",
                newName: "Contraindications");

            migrationBuilder.RenameColumn(
                name: "allergies",
                table: "Guests",
                newName: "Allergies");

            migrationBuilder.RenameColumn(
                name: "personnel_number",
                table: "Guests",
                newName: "PersonnelNumber");

            migrationBuilder.RenameColumn(
                name: "passport_series",
                table: "Guests",
                newName: "PassportSeries");

            migrationBuilder.RenameColumn(
                name: "passport_number",
                table: "Guests",
                newName: "PassportNumber");

            migrationBuilder.RenameColumn(
                name: "emergency_contact_phone",
                table: "Guests",
                newName: "EmergencyContactPhone");

            migrationBuilder.RenameColumn(
                name: "emergency_contact_name",
                table: "Guests",
                newName: "EmergencyContactName");

            migrationBuilder.RenameColumn(
                name: "birth_date",
                table: "Guests",
                newName: "BirthDate");

            migrationBuilder.RenameIndex(
                name: "IX_Guests_personnel_number",
                table: "Guests",
                newName: "IX_Guests_PersonnelNumber");
        }
    }
}
