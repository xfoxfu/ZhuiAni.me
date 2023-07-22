using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Me.Xfox.ZhuiAnime.Migrations
{
    /// <inheritdoc />
    public partial class TimeForEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // see https://diesel.rs/
            migrationBuilder.Sql("""
            CREATE OR REPLACE FUNCTION za_ef_manage_updated_at(_tbl regclass) RETURNS VOID AS $$
            BEGIN
                EXECUTE format('CREATE TRIGGER set_updated_at BEFORE UPDATE ON %s
                                FOR EACH ROW EXECUTE PROCEDURE za_ef_set_updated_at()', _tbl);
            END;
            $$ LANGUAGE plpgsql;

            CREATE OR REPLACE FUNCTION za_ef_set_updated_at() RETURNS TRIGGER AS $$
            BEGIN
                IF (
                    NEW IS DISTINCT FROM OLD AND
                    NEW.updated_at IS NOT DISTINCT FROM OLD.updated_at
                ) THEN
                    NEW.updated_at := current_timestamp;
                END IF;
                RETURN NEW;
            END;
            $$ LANGUAGE plpgsql;
            """);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at",
                table: "link",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updated_at",
                table: "link",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at",
                table: "item",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updated_at",
                table: "item",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at",
                table: "category",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updated_at",
                table: "category",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.Sql("""
            SELECT za_ef_manage_updated_at('link');
            SELECT za_ef_manage_updated_at('item');
            SELECT za_ef_manage_updated_at('category');
            """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
            DROP TRIGGER IF EXISTS set_updated_at ON link;  
            DROP TRIGGER IF EXISTS set_updated_at ON item;
            DROP TRIGGER IF EXISTS set_updated_at ON category;
            DROP FUNCTION IF EXISTS za_ef_manage_updated_at(_tbl regclass);
            DROP FUNCTION IF EXISTS za_ef_set_updated_at();
            """);

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "link");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "link");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "item");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "item");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "category");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "category");
        }
    }
}
