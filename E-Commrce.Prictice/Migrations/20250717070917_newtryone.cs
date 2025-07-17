using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Commrce.Prictice.Migrations
{
    /// <inheritdoc />
    public partial class newtryone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVerify",
                table: "OtpTokens",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVerify",
                table: "OtpTokens");
        }
    }
}
