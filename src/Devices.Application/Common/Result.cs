using System.Net;

namespace Devices.Application.Common;

public class Result<T>
{
    public bool IsSuccess { get; }
    public HttpStatusCode StatusCode { get; }
    public string Message { get; }
    public T Data { get; }
    public IEnumerable<string> Errors { get; }

    private Result(bool isSuccess, T data, string message, HttpStatusCode statusCode, IEnumerable<string> errors)
    {
        IsSuccess = isSuccess;
        Data = data;
        Message = message;
        StatusCode = statusCode;
        Errors = errors ?? Enumerable.Empty<string>();
    }

    public int StatusCodeValue => (int)StatusCode;

    public static Result<T> Success(
        T data,
        string message = "Success",
        HttpStatusCode statusCode = HttpStatusCode.OK)
        => new(true, data, message, statusCode, null);

    public static Result<T> Failure(
        string message,
        HttpStatusCode statusCode = HttpStatusCode.BadRequest,
        IEnumerable<string> errors = null)
        => new(false, default, message, statusCode, errors);
}