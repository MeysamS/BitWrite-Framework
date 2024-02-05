using Bw.XCommonHttpclient.Models;

namespace Bw.XCommonHttpclient;

public interface ICommonHttpClient<in TGetRequestModel, in TPostRequestModel, TResponseModel>
{
    Task<TResponseModel> GetAsync(TGetRequestModel model);


    // Post Method
    Task<TResponseModel> SendAsync(TPostRequestModel model);
}

public interface ICommonHttpClient : ICommonHttpClient<GetRequestModel, PostRequestModel, ResponseModel>
{
}
public interface ICommonHttpClient<THttpSeperator> : ICommonHttpClient
{
}
