using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GOCAP.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class GOCAP_V5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpireTime",
                table: "Stories");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ExpireTime",
                table: "Stories",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
