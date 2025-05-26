using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinimalAPIProfesional.Data.Migrations
{
    /// <inheritdoc />
    public partial class Supp_Prop_AJeter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ajeter",
                table: "Personnes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Ajeter",
                table: "Personnes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
