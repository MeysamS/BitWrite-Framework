using System.Net;

namespace Bw.XCommonHttpclient.Models;

public class ResponseModel
{
    public string Data { get; set; } = default!;
    public Exception Exception { get; set; } = default!;
    public bool IsSuccess => Exception == null;
    public HttpStatusCode StatusCode { get; set; }
}
