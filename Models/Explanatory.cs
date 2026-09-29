using System;
using System.Collections.Generic;

namespace BSDSystem.API.Models;

public partial class Explanatory
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public string DisturberLastName { get; set; } = null!;

    public string DisturberFirstName { get; set; } = null!;

    public string? DisturberMiddleName { get; set; }

    public string? DisturberRoomNumber { get; set; }

    public string DisturberType { get; set; } = null!;

    public DateOnly Dates { get; set; }

    public int ApproverId { get; set; }

    public virtual Employee Approver { get; set; } = null!;

    public virtual Employee Employee { get; set; } = null!;
}
