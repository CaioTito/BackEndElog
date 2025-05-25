using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEndElog.Shared.Configurations;

public class ElogApiSettings
{
    public string BaseUrl { get; set; } = string.Empty;
    public string OdometerPath { get; set; } = string.Empty;
    public string AuthorizationToken { get; set; } = string.Empty;
}
