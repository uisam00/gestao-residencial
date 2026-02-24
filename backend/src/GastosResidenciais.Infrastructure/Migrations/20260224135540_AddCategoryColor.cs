using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GastosResidenciais.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryColor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ColorHex",
                table: "Categories",
                type: "character varying(7)",
                maxLength: 7,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ColorHex",
                table: "Categories");
        }
    }
}
