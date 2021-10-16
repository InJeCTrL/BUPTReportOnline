using Microsoft.EntityFrameworkCore.Migrations;

namespace BUPTReportOnline.Migrations
{
    public partial class init_user : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Cookie", "Email", "EndHour", "EndMinute", "GUID", "IsAdmin", "LastMessage", "LastResult", "LastTime", "Registered", "SendInform", "StartHour", "StartMinute" },
                values: new object[] { 1, "", null, 0, 0, "REMOVE_INIT", true, null, false, null, true, false, 0, 0 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
