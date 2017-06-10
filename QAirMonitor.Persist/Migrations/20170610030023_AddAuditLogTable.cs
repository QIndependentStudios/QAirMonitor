using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace QAirMonitor.Persist.Migrations
{
    public partial class AddAuditLogTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLog",
                columns: table => new
                {
                    AuditLogID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EventDateTime = table.Column<DateTime>(nullable: false),
                    EventType = table.Column<int>(nullable: false),
                    Message = table.Column<string>(maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLog", x => x.AuditLogID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLog");
        }
    }
}
