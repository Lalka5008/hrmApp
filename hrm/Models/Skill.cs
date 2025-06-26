using hrm.Models;

public class Skill
{
    public int SkillId { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }

    public ICollection<EmployeeSkill> EmployeeSkills { get; set; }
}