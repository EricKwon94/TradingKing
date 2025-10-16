using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTracker : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 데이터베이스 차원 Change Tracking 켜기
            migrationBuilder.Sql("""
            ALTER DATABASE [TradingKing]
            SET CHANGE_TRACKING = ON
            (CHANGE_RETENTION = 2 DAYS, AUTO_CLEANUP = ON);
        """);

            // Orders 테이블 Change Tracking 활성화
            migrationBuilder.Sql("""
            ALTER TABLE [dbo].[Orders]
            ENABLE CHANGE_TRACKING;
        """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Orders 테이블 Change Tracking 비활성화
            migrationBuilder.Sql("""
            ALTER TABLE [dbo].[Orders]
            DISABLE CHANGE_TRACKING;
        """);

            // 데이터베이스 차원 Change Tracking 끄기
            migrationBuilder.Sql("""
            ALTER DATABASE [TradingKing]
            SET CHANGE_TRACKING = OFF;
        """);
        }
    }
}
