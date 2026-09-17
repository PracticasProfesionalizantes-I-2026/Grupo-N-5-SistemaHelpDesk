namespace HelpDesk.Shared.Exceptions;

public class UnauthorizedActionException : Exception
{
    public UnauthorizedActionException(string message) : base(message) { }
    public UnauthorizedActionException() : base("No tiene permisos para realizar esta acción") { }
}