using email_attachment_orders.Data;
using email_attachment_orders.Dto;
using Microsoft.EntityFrameworkCore;

namespace email_attachment_orders.Services
{
    public class OutletMappingService
    {
        private readonly AppDbContext _db;

        public OutletMappingService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<OutletMappingDto?> GetMappingByEmailAsync(string email)
        {
            return await _db.Database
                .SqlQuery<OutletMappingDto>($@"
                    SELECT
                        id AS OutletId,
                        customerOrgId AS CustomerOrgId,
                        warehouseId AS WarehouseId
                    FROM deliverit_dev.customer_outlet
                    WHERE LOWER(email) = LOWER({email})
                ")
                .FirstOrDefaultAsync();
        }
    }
}