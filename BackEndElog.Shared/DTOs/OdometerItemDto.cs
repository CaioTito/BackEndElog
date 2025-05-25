namespace BackEndElog.Shared.DTOs;

public class OdometerItemDto
{
    public int VehicleId { get; set; }
    public int? DivisionId { get; set; }
    public int? OperationId { get; set; }
    public int? DriverId { get; set; }
    public int? DriverDivisionId { get; set; }
    public int? DriverOperationId { get; set; }
    public int? Timezone { get; set; }
    public int? Quality { get; set; }
    public double? Interval { get; set; }
    public double? Odometer { get; set; }
    public double? OdometerKm { get; set; }
    public double? Bearing { get; set; }
    public double? Speed { get; set; }
    public bool Moving { get; set; }
    public bool Delayed { get; set; }
    public bool? Ignition { get; set; }
    public DateTime DateProcess { get; set; }
    public DateTime Date { get; set; }
    public string VehicleIdTms { get; set; } = string.Empty;
    public string DivisionName { get; set; } = string.Empty;
    public string OperationName { get; set; } = string.Empty;
    public string Connector { get; set; } = string.Empty;
    public string LicensePlate { get; set; } = string.Empty;
    public string DriverName { get; set; } = string.Empty;
    public string DriverIdTms { get; set; } = string.Empty;
    public string DriverDivisionName { get; set; } = string.Empty;
    public string DriverOperationName { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
    public string IgnitionStatus { get; set; } = string.Empty;
    public PointDto Point { get; set; } = new();
}

public class PointDto
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
