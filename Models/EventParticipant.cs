using System;
using System.Collections.Generic;

namespace BSDSystem.API.Models;

public partial class EventParticipant
{
    public int EventId { get; set; }

    public int EmployeeId { get; set; }

    public bool Statuse { get; set; }

    public int ApproverId { get; set; }

    public virtual Employee Approver { get; set; } = null!;

    public virtual Employee Employee { get; set; } = null!;

    public virtual Evente Event { get; set; } = null!;
}
