using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMSTrain.Migrators.PostgreSQL.Migrations.Application
{
    /// <inheritdoc />
    public partial class AdditionOfDescriptionOnClassTrainers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>( 
                name: "Description",
                table: "ClassTrainers",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "ClassTrainers");
        }
    }
}
