namespace BackEndElog.Shared.Results;
public class Error(int code, string description)
{
    public Error(string description) : this(0, description)
    {
        Description = description;
    }
    public int Code { get; set; } = code;
    public string Description { get; set; } = description;
}
