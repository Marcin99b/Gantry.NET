namespace Gantry.NET;

public record GantryError(int Code, string Message)
{
    public static bool TryParse(string response, out GantryError? gantryError)
    {
        if (!response.StartsWith("ERROR: "))
        {
            gantryError = null;
            return false;
        }

        response = response.Replace("ERROR: ", "");
        var splitted = response.Split(";");
        var code = int.Parse(splitted[0].Trim());
        var message = splitted[1].Trim();

        gantryError = new GantryError(code, message);
        return true;
    }
}