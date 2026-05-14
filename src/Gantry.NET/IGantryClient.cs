namespace Gantry.NET;

public interface IGantryClient
{
    Task Put(string message);
    Task<string> Get(int offset);
    Task<int> MaxOffset();
}
