namespace BackEndElog.Shared.DTOs;

public class OdometerResultDto
{
    public List<OdometerItemDto> Data { get; set; } = [];
    public int TotalItems { get; set; }
    public int NumberOfRowPage { get; set; }
    public int PageActive { get; set; }
    public int TotalPages { get; set; }
}
