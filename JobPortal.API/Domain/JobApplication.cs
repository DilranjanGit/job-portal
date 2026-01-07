using System;

namespace JobPortal.API.Domain
{
public enum ApplicationStatus
{
    Applied = 0,
    Reviewed = 1,
    Shortlisted = 2,
    Rejected = 3,
    InterviewScheduled = 4,
    Hired = 5
}

public class JobApplication
{
    public int Id { get; set; }
    public int JobId { get; set; }
    public Job Job { get; set; } = default!;
    public int StudentProfileId { get; set; }
    public StudentProfile Student { get; set; } = default!;
    public DateTime AppliedUtc { get; set; } = DateTime.UtcNow;
    public ApplicationStatus Status { get; set; } = ApplicationStatus.Applied;
    
}
}