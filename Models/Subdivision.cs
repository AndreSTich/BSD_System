using System;
using System.Collections.Generic;

namespace BSDSystem.API.Models;

public partial class Subdivision
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<Duty> Duties { get; set; } = new List<Duty>();

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
