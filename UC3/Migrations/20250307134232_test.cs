﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UC3.Migrations
{
    /// <inheritdoc />
    public partial class test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "randomNumber",
                table: "UserModels");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "randomNumber",
                table: "UserModels",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
