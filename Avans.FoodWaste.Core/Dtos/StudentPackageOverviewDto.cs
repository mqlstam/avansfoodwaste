using System.Collections.Generic;

namespace Avans.FoodWaste.Core.Dtos
{
    public class StudentPackageOverviewDto
    {
        public IEnumerable<PackageDto> AvailablePackages { get; set; } = Enumerable.Empty<PackageDto>();
        public IEnumerable<ReservationDetailsDto> ReservedPackages { get; set; } = Enumerable.Empty<ReservationDetailsDto>();
    }
}