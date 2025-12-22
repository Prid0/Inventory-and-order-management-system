public class ServiceResult<T>
{
    public bool Success { get; private set; }
    public string Message { get; private set; }
    public T Data { get; private set; }

    private ServiceResult(bool success, string message, T data)
    {
        Success = success;
        Message = message;
        Data = data;
    }

    public static ServiceResult<T> Result(T data, string successMessage = null)
    {
        if (data == null)
            return new ServiceResult<T>(false, successMessage, data);

        return new ServiceResult<T>(true, "Success", data);
    }
}
