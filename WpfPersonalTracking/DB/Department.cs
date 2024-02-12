﻿using System;
using System.Collections.Generic;

namespace WpfPersonalTracking.DB;

public partial class Department
{
    public int Id { get; set; }

    public string DepartmentName { get; set; } = null!;

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<Position> Positions { get; set; } = new List<Position>();
}
