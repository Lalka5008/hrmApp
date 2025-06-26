public class SurveyResponse
{
    public int ResponseId { get; set; }
    public int SurveyId { get; set; }
    public Survey Survey { get; set; }
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; }
    public string Answers { get; set; } // JSON строка с ответами
}