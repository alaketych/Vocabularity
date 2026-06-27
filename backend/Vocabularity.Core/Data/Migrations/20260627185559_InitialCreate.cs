using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vocabularity.Core.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Dictionaries",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Dictionary_name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    user_id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    language_id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    original_word = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    original_transcriptioned_word = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    translated_word = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    ttl = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dictionaries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    language_name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    language_image = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    ttl = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    user_login = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    user_pseudonym = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    user_password = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    user_password_salt = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    ttl = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Dictionaries");

            migrationBuilder.DropTable(
                name: "Languages");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
