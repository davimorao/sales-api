namespace Sales.Application
{
    public class BaseResponse<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public string ErrorMessage { get; set; }
        public List<string> ValidationErrors { get; set; }

        public BaseResponse(T data)
        {
            Success = true;
            Data = data;
            ValidationErrors = new List<string>();
        }

        public BaseResponse(string errorMessage)
        {
            Success = false;
            ErrorMessage = errorMessage;
            ValidationErrors = new List<string>();
        }

        public BaseResponse(List<string> validationErrors)
        {
            Success = false;
            ValidationErrors = validationErrors;
        }
    }

}
