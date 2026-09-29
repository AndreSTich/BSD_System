using System;
using System.Collections.Generic;

namespace BSDSystem.API.Models;

public partial class Employee
{
    public int Id { get; set; }

    public int RoleId { get; set; }

    public int SubdivisionId { get; set; }

    public string LastName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public string BadgeNumber { get; set; } = null!;

    public string Logine { get; set; } = null!;

    public string Passworde { get; set; } = null!;

    public string? Photo { get; set; }

    public int? Rating { get; set; }

    public virtual ICollection<Evente> Eventes { get; set; } = new List<Evente>();

    public virtual ICollection<Explanatory> ExplanatoryApprovers { get; set; } = new List<Explanatory>();

    public virtual ICollection<Explanatory> ExplanatoryEmployees { get; set; } = new List<Explanatory>();

    public virtual Role Role { get; set; } = null!;

    public virtual Subdivision Subdivision { get; set; } = null!;
}
