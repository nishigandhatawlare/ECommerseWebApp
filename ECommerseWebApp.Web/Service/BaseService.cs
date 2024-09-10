using ECommerseWebApp.Web.Models;
using ECommerseWebApp.Web.Service.IService;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using static ECommerseWebApp.Web.Utility.SD;

namespace ECommerseWebApp.Web.Service
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public BaseService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<ResponseDto?> SendAsync(RequestDto requestDto)
        {
            try
            {
                // Create the HttpClient using the factory
                HttpClient client = _httpClientFactory.CreateClient("ECommerseWebAppApi");

                // Create the HttpRequestMessage
                HttpRequestMessage message = new();
                message.Headers.Add("Accept", "application/json");

                // Set the request URI
                message.RequestUri = new Uri(requestDto.Url);

                // Add request body content if available
                if (requestDto.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(requestDto.Data), Encoding.UTF8, "application/json");
                }

                // Set the HTTP method based on the ApiType
                switch (requestDto.ApiType)
                {
                    case ApiType.Post:
                        message.Method = HttpMethod.Post;
                        break;
                    case ApiType.Delete:
                        message.Method = HttpMethod.Delete;
                        break;
                    case ApiType.Put:
                        message.Method = HttpMethod.Put;
                        break;
                    default:
                        message.Method = HttpMethod.Get;
                        break;
                }

                // Send the HTTP request
                HttpResponseMessage? apiResponse = await client.SendAsync(message);

                // Handle different HTTP status codes
                switch (apiResponse.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        return new ResponseDto { IsSuccess = false, Message = "Not Found" };
                    case HttpStatusCode.Forbidden:
                        return new ResponseDto { IsSuccess = false, Message = "Access Denied" };
                    case HttpStatusCode.Unauthorized:
                        return new ResponseDto { IsSuccess = false, Message = "Unauthorized" };
                    case HttpStatusCode.InternalServerError:
                        return new ResponseDto { IsSuccess = false, Message = "Internal Server Error" };
                    default:
                        // If status code indicates success
                        if (apiResponse.IsSuccessStatusCode)
                        {
                            // Read and deserialize the response content
                            var apiContent = await apiResponse.Content.ReadAsStringAsync();
                            var apiResponseDto = JsonConvert.DeserializeObject<ResponseDto>(apiContent);

                            // Set the success flag and message if deserialization succeeds
                            if (apiResponseDto != null)
                            {
                                apiResponseDto.IsSuccess = true;
                                apiResponseDto.Message = "Request successful!";
                                return apiResponseDto;
                            }
                            else
                            {
                                return new ResponseDto { IsSuccess = false, Message = "Failed to deserialize response" };
                            }
                        }
                        else
                        {
                            // If not successful, handle unexpected statuses
                            return new ResponseDto { IsSuccess = false, Message = "Unexpected status code: " + apiResponse.StatusCode };
                        }
                }
            }
            catch (Exception ex)
            {
                // Catch and return error response
                return new ResponseDto
                {
                    Message = ex.Message.ToString(),
                    IsSuccess = false
                };
            }
        }
    }
}
