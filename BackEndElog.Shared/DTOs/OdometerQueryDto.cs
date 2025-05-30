namespace BackEndElog.Shared.DTOs;

public class OdometerQueryDto
{
    /// <summary>
    /// Data inicial para consulta.
    /// </summary>
    public DateTime StartDate { get; set; }
    /// <summary>
    /// Data final para consulta.
    /// </summary>
    public DateTime EndDate { get; set; }
    public List<string>? IdTms { get; set; }
    public List<string>? LicensePlate { get; set; }
    public List<int>? DivisionId { get; set; }
    public int? Rows { get; set; }
    public int? Page { get; set; }
}