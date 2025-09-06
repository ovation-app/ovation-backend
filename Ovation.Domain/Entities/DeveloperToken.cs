using System;
using System.Collections.Generic;

namespace Ovation.Domain.Entities;

public partial class DeveloperToken
{
    public byte[] TokenId { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string CountryCode { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string City { get; set; } = null!;

    public string Country { get; set; } = null!;

    public string Platform { get; set; } = null!;

    public sbyte Active { get; set; }

    public sbyte Role { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
