namespace email_attachment_orders.Models
{
    public class PurchaseOrderItem
    {
        public string? OutletName { get; set; }

        public string? S_No { get; set; }
        public string? PO_Number { get; set; }
        public string? ItemsNumber { get; set; }

        public string? ItemName { get; set; }
        public string? HSN_Code { get; set; }
        public string? Specification { get; set; }
        public string? UOM { get; set; }

        public string? PackSize { get; set; }

        public string? Price { get; set; }
        public string? Tax { get; set; }

        public string? DeliveryDate { get; set; }

        public string? Qty { get; set; }

        public string? Status { get; set; }

        public PurchaseOrderItem()
        {
            OutletName = "OutletName";
            Status = "Pending Confirmation";
        }
    }
}