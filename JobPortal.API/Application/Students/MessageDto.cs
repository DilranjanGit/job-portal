
using System;

namespace JobPortal.API.Application.Students
{
    public class MessageDto
    {
        public Guid Id { get; set; }
        public Guid FromUserId { get; set; }
        public Guid ToUserId { get; set; }
        public string Body { get; set; } = "";
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }
    }

    public class MessageCreateDto
    {
        public Guid ToUserId { get; set; }
        public string Body { get; set; } = "";
    }
}
