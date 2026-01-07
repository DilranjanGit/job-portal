namespace JobPortal.API.Domain
{
    public enum InterviewStatus
    {
        Scheduled = 0,
        Completed = 1,
        Cancelled = 2
    }
    public enum InterviewMode
    {
        InPerson = 0,
        Online = 1,
        Phone = 2
    }

    public class Interview
    {
        public int Id { get; set; }
        public int JobApplicationId { get; set; }
        public JobApplication Application { get; set; } = default!;
        public DateTime ScheduledAtLocal { get; set; }
        public InterviewMode Mode { get; set; } = InterviewMode.Online;
        public string LocationOrLink { get; set; } = "";
        public InterviewStatus Status { get; set; } = InterviewStatus.Scheduled;
        
    }
}