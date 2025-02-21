using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScanBarcode.Migrations
{
    /// <inheritdoc />
    public partial class AddProdmod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "CycleTime",
                table: "ProdModels",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HeadCon",
                table: "ProdModels",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CycleTime",
                table: "ProdModels");

            migrationBuilder.DropColumn(
                name: "HeadCon",
                table: "ProdModels");
        }
    }
}
