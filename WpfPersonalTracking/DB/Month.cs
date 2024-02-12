using System;
using System.Collections.Generic;

namespace WpfPersonalTracking.DB;

public partial class Month
{
    public int Id { get; set; }

    public string MonthName { get; set; } = null!;

    public virtual ICollection<Salary> Salaries { get; set; } = new List<Salary>();
}
