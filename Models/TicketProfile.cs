namespace MovieTicketWebsite.Models
{
    public class InvoiceFood
    {
        public int IdInvoiceFood { get; set; }
        public int IdInvoice { get; set; }
        public int IdFood { get; set; }
        public int Quantity { get; set; }
        public int TotalPrice { get; set; }
    }
    public class Invoice
    {
        public int IdInvoice { get; set; }
        public int IdUser { get; set; }
        public DateTime CreateAt { get; set; }
        public int TotalPrice { get; set; }
        public int? IdVoucher { get; set; }
        public int PointUsed { get; set; }
        public int PointEarned { get; set; }
        public int Status { get; set; }
        public List<string> Seats { get; set; }
        public List<InvoiceFood> Foods { get; set; }
    }
}
