using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventOganizer.Migrations
{
    /// <inheritdoc />
    public partial class cartitemModified : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isBought",
                table: "CartItems",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isBought",
                table: "CartItems");
        }
    }
}
