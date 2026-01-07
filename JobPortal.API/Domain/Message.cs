namespace JobPortal.API.Domain
{
    public enum SenderRole
    {
        Student = 0,
        Company = 1,
        Admin = 2
    }
    public class Message
    {
        public int Id { get; set; }
        public SenderRole Sender { get; set; }
        public required string SenderEmail { get; set; }
        public required string ReceiverEmail { get; set; }
        public string Content { get; set; } = default!;
        public DateTime SentUtc { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; } = false;
    }
}