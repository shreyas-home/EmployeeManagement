using System;
using System.Collections.Generic;

namespace WpfPersonalTracking.DB;

public partial class TaskState
{
    public int Id { get; set; }

    public string StateName { get; set; } = null!;

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
