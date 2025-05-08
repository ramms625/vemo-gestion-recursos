
namespace Vemo.Gestion.Recursos.Data.Models
{
    public class NoContentApiResponse
    {
        public NoContentApiResponse(string message = "")
        {
            Message = message;
        }

        public string Message { get; set; }
    }


    public class ApiResponse<T>
    {
        public ApiResponse(T data, string message = "")
        {
            Message = message;
            Data = data;
        }

        public string Message { get; set; }
        public T Data { get; set; }
    }
}