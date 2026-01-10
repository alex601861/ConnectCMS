using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMSTrain.Migrators.PostgreSQL.Migrations.Application
{
    /// <inheritdoc />
    public partial class AdditionOfLocationCoordinates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                table: "Trainings",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                table: "Trainings",
                type: "numeric",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Trainings");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Trainings");
        }
    }
}
