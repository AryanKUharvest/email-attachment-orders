namespace email_attachment_orders.Dto
{
    public class EmailOrderDto
    {
        public string MessageId { get; set; } = String.Empty;
        public string SenderEmail { get; set; } = String.Empty;
        public string Body { get; set; } = String.Empty;
        public string Subject { get; set; } = String.Empty;
        public DateTime Timestamp { get; set; }
        public bool IsRead { get; set; }
        public List<ExcelOrderDto> Attachments { get; set; } = new List<ExcelOrderDto>();
        public string ReadStatus => IsRead ? "Read" : "Unread";
    }
}