using System;
using System.Collections.Generic;

namespace BSDSystem.API.Models;

public partial class DutyRegistration
{
    public int DutyId { get; set; }

    public int EmployeeId { get; set; }

    public int ApproverId { get; set; }

    public virtual Employee Approver { get; set; } = null!;

    public virtual Duty Duty { get; set; } = null!;

    public virtual Employee Employee { get; set; } = null!;
}
