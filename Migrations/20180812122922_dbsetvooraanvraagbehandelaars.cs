using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ApplicationRequestIt.Migrations
{
    public partial class dbsetvooraanvraagbehandelaars : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AanvraagBehandelaar_Aanvragen_AanvraagId",
                table: "AanvraagBehandelaar");

            migrationBuilder.DropForeignKey(
                name: "FK_AanvraagBehandelaar_AspNetUsers_BehandelaarId",
                table: "AanvraagBehandelaar");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AanvraagBehandelaar",
                table: "AanvraagBehandelaar");

            migrationBuilder.RenameTable(
                name: "AanvraagBehandelaar",
                newName: "AanvraagBehandelaars");

            migrationBuilder.RenameIndex(
                name: "IX_AanvraagBehandelaar_BehandelaarId",
                table: "AanvraagBehandelaars",
                newName: "IX_AanvraagBehandelaars_BehandelaarId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AanvraagBehandelaars",
                table: "AanvraagBehandelaars",
                columns: new[] { "AanvraagId", "BehandelaarId" });

            migrationBuilder.AddForeignKey(
                name: "FK_AanvraagBehandelaars_Aanvragen_AanvraagId",
                table: "AanvraagBehandelaars",
                column: "AanvraagId",
                principalTable: "Aanvragen",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AanvraagBehandelaars_AspNetUsers_BehandelaarId",
                table: "AanvraagBehandelaars",
                column: "BehandelaarId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AanvraagBehandelaars_Aanvragen_AanvraagId",
                table: "AanvraagBehandelaars");

            migrationBuilder.DropForeignKey(
                name: "FK_AanvraagBehandelaars_AspNetUsers_BehandelaarId",
                table: "AanvraagBehandelaars");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AanvraagBehandelaars",
                table: "AanvraagBehandelaars");

            migrationBuilder.RenameTable(
                name: "AanvraagBehandelaars",
                newName: "AanvraagBehandelaar");

            migrationBuilder.RenameIndex(
                name: "IX_AanvraagBehandelaars_BehandelaarId",
                table: "AanvraagBehandelaar",
                newName: "IX_AanvraagBehandelaar_BehandelaarId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AanvraagBehandelaar",
                table: "AanvraagBehandelaar",
                columns: new[] { "AanvraagId", "BehandelaarId" });

            migrationBuilder.AddForeignKey(
                name: "FK_AanvraagBehandelaar_Aanvragen_AanvraagId",
                table: "AanvraagBehandelaar",
                column: "AanvraagId",
                principalTable: "Aanvragen",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AanvraagBehandelaar_AspNetUsers_BehandelaarId",
                table: "AanvraagBehandelaar",
                column: "BehandelaarId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
