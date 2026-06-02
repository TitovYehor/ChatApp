namespace ChatApp.Application.Exceptions;

public class UserNotAuthenticatedException : Exception
{
    public UserNotAuthenticatedException()
        : base("User is not authenticated")
    {
    }
}