namespace email_attachment_orders.Dto
{
    public class ExcelOrderDto
    {
        public int SNo { get; set; }

        public string PoNumber { get; set; } = string.Empty;

        public string OutletName { get; set; } = string.Empty;

        public int ItemNumber { get; set; }

        public string ItemName { get; set; } = string.Empty;

        public string HsnCode { get; set; } = string.Empty;

        public string Specification { get; set; } = string.Empty;

        public string Uom { get; set; } = string.Empty;

        public int PackSize { get; set; }

        public decimal Price { get; set; }

        public decimal Tax { get; set; }

        public DateTime DeliveryDate { get; set; }

        public int Qty { get; set; }
    }
}