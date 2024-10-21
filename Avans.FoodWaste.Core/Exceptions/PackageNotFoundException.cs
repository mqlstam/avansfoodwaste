namespace Avans.FoodWaste.Core.Exceptions;

public class PackageNotFoundException : Exception
{
    public PackageNotFoundException(int packageId)
        : base($"Package with ID {packageId} not found.") { }
}