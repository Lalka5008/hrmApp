

public class Survey
{
    public int SurveyId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public ICollection<SurveyResponse> Responses { get; set; }
}