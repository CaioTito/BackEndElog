namespace BackEndElog.Application.Queries;
public class GetOdometerQuery
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<string>? IdTms { get; set; }
    public List<string>? LicensePlate { get; set; }
    public List<int>? DivisionId { get; set; }
    public int? Rows { get; set; }
    public int? Page { get; set; }
}
