using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ApplicationRequestIt.Migrations
{
    public partial class meerderebehandelaars : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Aanvraag_ApplicationUser_Behandelaar",
                table: "Aanvragen");

            migrationBuilder.DropIndex(
                name: "IX_Aanvragen_BehandelaarId",
                table: "Aanvragen");

            migrationBuilder.AlterColumn<string>(
                name: "BehandelaarId",
                table: "Aanvragen",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Aanvragen",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AanvraagBehandelaar",
                columns: table => new
                {
                    AanvraagId = table.Column<int>(type: "int", nullable: false),
                    BehandelaarId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AanvraagBehandelaar", x => new { x.AanvraagId, x.BehandelaarId });
                    table.ForeignKey(
                        name: "FK_AanvraagBehandelaar_Aanvragen_AanvraagId",
                        column: x => x.AanvraagId,
                        principalTable: "Aanvragen",
                        principalColumn: "Id");
                    //onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AanvraagBehandelaar_AspNetUsers_BehandelaarId",
                        column: x => x.BehandelaarId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                        //onDelete: ReferentialAction.Cascade)
                });

            migrationBuilder.CreateIndex(
                name: "IX_Aanvragen_ApplicationUserId",
                table: "Aanvragen",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AanvraagBehandelaar_BehandelaarId",
                table: "AanvraagBehandelaar",
                column: "BehandelaarId");

            migrationBuilder.AddForeignKey(
                name: "FK_Aanvragen_AspNetUsers_ApplicationUserId",
                table: "Aanvragen",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Aanvragen_AspNetUsers_ApplicationUserId",
                table: "Aanvragen");

            migrationBuilder.DropTable(
                name: "AanvraagBehandelaar");

            migrationBuilder.DropIndex(
                name: "IX_Aanvragen_ApplicationUserId",
                table: "Aanvragen");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Aanvragen");

            migrationBuilder.AlterColumn<string>(
                name: "BehandelaarId",
                table: "Aanvragen",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Aanvragen_BehandelaarId",
                table: "Aanvragen",
                column: "BehandelaarId");

            migrationBuilder.AddForeignKey(
                name: "FK_Aanvraag_ApplicationUser_Behandelaar",
                table: "Aanvragen",
                column: "BehandelaarId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
