namespace ChatApp.Contracts.Common;

/// <summary>
/// Represents the error message returned when an API request fails
/// </summary>
public class ErrorResponse
{
    public string Message { get; set; } = string.Empty;
}