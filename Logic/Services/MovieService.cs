using Logic.Entities;

namespace Logic.Services;

public class MovieService
{
    public DateTime? GetExpirationDate(LicensingModel licensingModel)
    {
        switch (licensingModel)
        {
            case LicensingModel.TwoDays:
                return DateTime.UtcNow.AddDays(2);
            case LicensingModel.LifeLong:
                return null;
            default:
                throw new ArgumentOutOfRangeException(nameof(licensingModel), licensingModel, null);
        }
    }
}