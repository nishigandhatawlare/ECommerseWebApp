using static ECommerseWebApp.Web.Utility.SD;

namespace ECommerseWebApp.Web.Models
{
    public class RequestDto
    {
        public ApiType ApiType { get; set; } = ApiType.Get;
        public string Url { get; set; }
        public object Data {  get; set; } //will populated when create or update,
        public string AccessToken { get; set; } //will use at the time of authentication and authorization.

    }
}
