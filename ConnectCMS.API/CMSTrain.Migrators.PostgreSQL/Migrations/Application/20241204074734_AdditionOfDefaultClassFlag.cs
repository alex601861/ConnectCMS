using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMSTrain.Migrators.PostgreSQL.Migrations.Application
{
    /// <inheritdoc />
    public partial class AdditionOfDefaultClassFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Organizations",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDefaultClass",
                table: "Classes",
                type: "boolean",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDefaultClass",
                table: "Classes");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Organizations",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
