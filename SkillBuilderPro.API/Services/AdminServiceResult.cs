namespace SkillBuilderPro.API.Services;

public enum AdminServiceStatus
{
    Success,
    Created,
    ValidationError,
    NotFound,
    Conflict,
    Forbidden
}

public sealed record AdminServiceResult<T>(
    AdminServiceStatus Status,
    T? Value = default,
    string? Error = null)
{
    public static AdminServiceResult<T> Success(T value) =>
        new(AdminServiceStatus.Success, value);

    public static AdminServiceResult<T> Created(T value) =>
        new(AdminServiceStatus.Created, value);

    public static AdminServiceResult<T> Validation(string error) =>
        new(AdminServiceStatus.ValidationError, default, error);

    public static AdminServiceResult<T> NotFound(string error) =>
        new(AdminServiceStatus.NotFound, default, error);

    public static AdminServiceResult<T> Conflict(string error) =>
        new(AdminServiceStatus.Conflict, default, error);

    public static AdminServiceResult<T> Forbidden() =>
        new(AdminServiceStatus.Forbidden, default, "Administrator access is required.");
}
