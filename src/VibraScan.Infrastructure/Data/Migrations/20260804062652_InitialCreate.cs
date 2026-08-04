using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VibraScan.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InspectionRules",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MeasurementProfiles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasurementProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Workshops",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workshops", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InspectionIntervals",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TargetCondition = table.Column<int>(type: "int", nullable: false),
                    DaysCount = table.Column<int>(type: "int", nullable: false),
                    InspectionRuleId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionIntervals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InspectionIntervals_InspectionRules_InspectionRuleId",
                        column: x => x.InspectionRuleId,
                        principalTable: "InspectionRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Engines",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Condition = table.Column<int>(type: "int", nullable: false),
                    LastInspectionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextInspectionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WorkshopId = table.Column<long>(type: "bigint", nullable: false),
                    InspectionRuleId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Engines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Engines_InspectionRules_InspectionRuleId",
                        column: x => x.InspectionRuleId,
                        principalTable: "InspectionRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Engines_Workshops_WorkshopId",
                        column: x => x.WorkshopId,
                        principalTable: "Workshops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Points",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EngineId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Points", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Points_Engines_EngineId",
                        column: x => x.EngineId,
                        principalTable: "Engines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VibrationMeasurements",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AxisType = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MeasurementDomain = table.Column<int>(type: "int", nullable: false),
                    CapturedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RawData = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    Rms = table.Column<float>(type: "real", nullable: false),
                    PointId = table.Column<long>(type: "bigint", nullable: false),
                    MeasurementProfileId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VibrationMeasurements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VibrationMeasurements_MeasurementProfiles_MeasurementProfileId",
                        column: x => x.MeasurementProfileId,
                        principalTable: "MeasurementProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VibrationMeasurements_Points_PointId",
                        column: x => x.PointId,
                        principalTable: "Points",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Engines_InspectionRuleId",
                table: "Engines",
                column: "InspectionRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Engines_WorkshopId_Name",
                table: "Engines",
                columns: new[] { "WorkshopId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InspectionIntervals_InspectionRuleId_TargetCondition",
                table: "InspectionIntervals",
                columns: new[] { "InspectionRuleId", "TargetCondition" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InspectionRules_Name",
                table: "InspectionRules",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementProfiles_Description",
                table: "MeasurementProfiles",
                column: "Description",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Points_EngineId_Name",
                table: "Points",
                columns: new[] { "EngineId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VibrationMeasurements_AxisType",
                table: "VibrationMeasurements",
                column: "AxisType");

            migrationBuilder.CreateIndex(
                name: "IX_VibrationMeasurements_CapturedAt_MeasurementDomain",
                table: "VibrationMeasurements",
                columns: new[] { "CapturedAt", "MeasurementDomain" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VibrationMeasurements_MeasurementProfileId",
                table: "VibrationMeasurements",
                column: "MeasurementProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_VibrationMeasurements_PointId_AxisType_MeasurementProfileId_CapturedAt",
                table: "VibrationMeasurements",
                columns: new[] { "PointId", "AxisType", "MeasurementProfileId", "CapturedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Workshops_Name",
                table: "Workshops",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InspectionIntervals");

            migrationBuilder.DropTable(
                name: "VibrationMeasurements");

            migrationBuilder.DropTable(
                name: "MeasurementProfiles");

            migrationBuilder.DropTable(
                name: "Points");

            migrationBuilder.DropTable(
                name: "Engines");

            migrationBuilder.DropTable(
                name: "InspectionRules");

            migrationBuilder.DropTable(
                name: "Workshops");
        }
    }
}
