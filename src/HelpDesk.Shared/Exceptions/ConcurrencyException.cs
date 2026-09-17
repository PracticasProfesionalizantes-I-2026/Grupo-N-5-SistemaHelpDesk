namespace HelpDesk.Shared.Exceptions;

public class ConcurrencyException : Exception
{
    public ConcurrencyException(string message) : base(message) { }
    public ConcurrencyException() : base("El registro fue modificado por otro usuario. Por favor, recargue e intente nuevamente") { }
}