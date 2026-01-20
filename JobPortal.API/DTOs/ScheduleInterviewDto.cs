
namespace JobPortal.API.DTOs
{
    public class ScheduleInterviewDto
    {
        public int JobApplicationId { get; set; }
        public int Id {get;set;}=0;
        public string  LocationOrLink { get; set; } = "Team Meeting Link";
        public int Mode { get; set; }
        public DateTime InterviewDate { get; set; }

    }
}
