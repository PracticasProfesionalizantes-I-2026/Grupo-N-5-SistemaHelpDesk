namespace HelpDesk.Shared.Exceptions;

public class DuplicateException : Exception
{
    public DuplicateException(string message) : base(message) { }
    public DuplicateException(string entityName, string field, object value) : base($"{entityName} con {field} '{value}' ya existe") { }
}