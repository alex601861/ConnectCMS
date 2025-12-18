using CMSTrain.Application.Common.Service;

namespace CMSTrain.Infrastructure.Persistence.Seed;

public interface IDbInitializer : IScopedService
{
    Task InitializeIdentityData(CancellationToken cancellationToken = default);

    Task InitializeCountryDataSets(CancellationToken cancellationToken = default);

    Task InitializeDesignationDataSets(CancellationToken cancellationToken = default);

    Task InitializeMenuData(CancellationToken cancellationToken = default);

    Task InitializeHeadingData(CancellationToken cancellationToken = default);

    Task InitializeStrategicData(CancellationToken cancellationToken = default);
    
    Task InitializeTraitData(CancellationToken cancellationToken = default);
}