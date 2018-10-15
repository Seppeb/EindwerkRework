using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ApplicationRequestIt.Migrations
{
    public partial class aanvraaggeschiedenis : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AanvraagGeschiedenis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AanvraagId = table.Column<int>(type: "int", nullable: false),
                    Achternaam = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Actie = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Voornaam = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    time = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AanvraagGeschiedenis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AanvraagGeschiedenis_Aanvragen_AanvraagId",
                        column: x => x.AanvraagId,
                        principalTable: "Aanvragen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AanvraagGeschiedenis_AanvraagId",
                table: "AanvraagGeschiedenis",
                column: "AanvraagId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AanvraagGeschiedenis");
        }
    }
}
