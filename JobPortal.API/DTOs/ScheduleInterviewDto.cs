
namespace JobPortal.API.DTOs
{
    public class ScheduleInterviewDto
    {
        public int JobApplicationId { get; set; }

        public string  LocationOrLink { get; set; } = "Team Meeting Link";
        public int Mode { get; set; }
        public DateTime InterviewDate { get; set; }

    }
}
