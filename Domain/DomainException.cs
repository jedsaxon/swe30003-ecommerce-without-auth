namespace Domain;

public class DomainException(
    Dictionary<string, string> exceptions,
    string message = "Errors occurred while creating the domain model")
    : Exception(message)
{
    public Dictionary<string, string> DomainExceptions => exceptions;
    
    /// <summary>
    /// Attepmts to execute the action. If a `DomainException` was thrown, it will add it to the `errors` dictionary,
    /// and return false. This does not check for any other exceptions, so it may be a better idea to manually add
    /// try/catch statements if that is the case.
    /// </summary>
    /// <param name="current">The current domain errors, which will be modified if action() throws a DomainException</param>
    /// <param name="action">The action to execute. It will catch any `DomainException`s that occur, and morph it into the current errors</param>
    /// <returns>Whether a `DomainException` was thrown during the action</returns>
    public static bool TryExecute(Dictionary<string, string> current, Action action) 
    {
        try
        {
            action();
        }
        catch (DomainException e)
        {
            foreach (var (k, v) in e.DomainExceptions)
                current[k] = v;

            return false;
        }

        return true;
    }
}