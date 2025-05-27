namespace BackEndElog.Shared.DTOs;

public class OdometerItemDto
{
    public string VehicleIdTms { get; set; } = string.Empty;
    public string DivisionName { get; set; } = string.Empty;
    public string OperationName { get; set; } = string.Empty;
    public double? OdometerKm { get; set; }
    public string DriverName { get; set; } = string.Empty;
    public double? Speed { get; set; }
    public bool Moving { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
    public string IgnitionStatus { get; set; } = string.Empty;
}
