namespace SecurityPortal.Application.Common.Models;

public class Result
{
    public bool IsSuccess { get; }
    public string Error { get; }
    public List<string> Errors { get; }

    protected Result(bool isSuccess, string error, List<string> errors)
    {
        IsSuccess = isSuccess;
        Error = error ?? string.Empty;
        Errors = errors ?? new List<string>();
    }

    public static Result Success() => new(true, string.Empty, new List<string>());
    public static Result Failure(string error) => new(false, error, new List<string>());
    public static Result Failure(List<string> errors) => new(false, string.Empty, errors);
}

public class Result<T> : Result
{
    public T? Value { get; }

    protected Result(T? value, bool isSuccess, string error, List<string> errors)
        : base(isSuccess, error, errors)
    {
        Value = value;
    }

    public static Result<T> Success(T value) => new(value, true, string.Empty, new List<string>());
    public static new Result<T> Failure(string error) => new(default, false, error, new List<string>());
    public static new Result<T> Failure(List<string> errors) => new(default, false, string.Empty, errors);
}