using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Projekt.Migrations
{
    /// <inheritdoc />
    public partial class AddProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""

                create procedure CancelExpiredOrders as
                begin
                    update orders set status = 2 
                    where(status = 0 and isdeleted = 0 and date < dateadd(day, -7, getdate()));
                end

                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("drop procedure CancelExpiredOrders");
        }
    }
}
