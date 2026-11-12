using System.ComponentModel.DataAnnotations;

namespace backend_bicigo.Fleet.Interfaces.REST.Resources;

/// <summary>Create bike-point payload (POST /bike-points).</summary>
public sealed class CreateBikePointResource
{
    /// <summary>Station name.</summary>
    [Required] public string Name { get; set; } = string.Empty;

    /// <summary>District.</summary>
    [Required] public string District { get; set; } = string.Empty;

    /// <summary>Address.</summary>
    [Required] public string Address { get; set; } = string.Empty;

    /// <summary>Latitude.</summary>
    public double Latitude { get; set; }

    /// <summary>Longitude.</summary>
    public double Longitude { get; set; }

    /// <summary>Capacity (&gt;= 1).</summary>
    [Range(1, int.MaxValue)] public int Capacity { get; set; }
}
