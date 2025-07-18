﻿using hrm.Models;

public class EmployeeSkill
{
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; }
    public int SkillId { get; set; }
    public Skill Skill { get; set; }
    public string Level { get; set; } // "Начальный", "Средний", "Продвинутый", "Эксперт"
}