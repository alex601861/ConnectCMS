using CMSTrain.Application.DTOs.TrainingFormat;
using CMSTrain.Application.Exceptions;
using CMSTrain.Application.Interfaces.Repositories.Base;
using CMSTrain.Application.Interfaces.Services;
using CMSTrain.Domain.Entities;

namespace CMSTrain.Infrastructure.Implementation.Services;

public class TrainingFormatService(IGenericRepository genericRepository) : ITrainingFormatService
{
    public List<GetTrainingFormatDto> GetAllTrainingFormats(int pageNumber, int pageSize, out int rowCount, string? search, bool? isActive)
    {
        var trainingFormat = genericRepository.GetPagedResult<TrainingFormat>(
            pageNumber, pageSize, out rowCount, 
            x => 
                (string.IsNullOrEmpty(search) || x.Name.ToLower().Contains(search.ToLower())) && 
                (isActive == null || x.IsActive == isActive)).ToList();

        var result = trainingFormat.Select(x => new GetTrainingFormatDto()
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            IsActive = x.IsActive
        }).ToList();

        return result;
    }
    
    public List<GetTrainingFormatDto> GetAllTrainingFormats(string? search, bool? isActive)
    {
        var result = new List<GetTrainingFormatDto>();

        var trainingFormats = genericRepository.Get<TrainingFormat>(x => 
            (string.IsNullOrEmpty(search) || x.Name.ToLower().Contains(search.ToLower())) && 
            (isActive == null || x.IsActive == isActive));

        foreach (var trainingFormat in trainingFormats)
        {
            var trainingFormatDto = new GetTrainingFormatDto()
            {
                Id = trainingFormat.Id,
                Name = trainingFormat.Name,
                Description = trainingFormat.Description,
                IsActive = trainingFormat.IsActive, 
            };

            result.Add(trainingFormatDto);
        }

        return result;
    }

    public GetTrainingFormatDto GetTrainingFormatById(Guid id)
    {
        var trainingFormat = genericRepository.GetById<TrainingFormat>(id)
            ?? throw new NotFoundException("The following training format with the specified Id was not found.");

        var result = new GetTrainingFormatDto()
        {
            Id = trainingFormat.Id,
            Name = trainingFormat.Name,
            Description = trainingFormat.Description,
            IsActive = trainingFormat.IsActive
        };

        return result;
    }

    public void InsertTrainingFormat(CreateTrainingFormatDto trainingFormat)
    {
        var existingTrainingFormat = genericRepository.GetFirstOrDefault<TrainingFormat>(x => x.Name == trainingFormat.Name);

        if (existingTrainingFormat != null)
        {
            var classException = new[]
            {
                "The training format with the specified name already exists.",
            };
            
            throw new BadRequestException("The training format is not valid", classException);
        }

        var trainingFormatModel = new TrainingFormat()
        {
            Name = trainingFormat.Name,
            Description = trainingFormat.Description,
        };

        genericRepository.Insert(trainingFormatModel);
    }

    public void UpdateTrainingFormat(UpdateTrainingFormatDto trainingFormat)
    {
        var trainingFormatModel = genericRepository.GetById<TrainingFormat>(trainingFormat.Id)
            ?? throw new NotFoundException("The following training format was not found.");

        trainingFormatModel.Name = trainingFormat.Name;
        trainingFormatModel.Description = trainingFormat.Description;

        genericRepository.Update(trainingFormatModel);
    }

    public void ActivateDeactivateTrainingFormat(Guid id)
    {
        var trainingFormatModel = genericRepository.GetById<TrainingFormat>(id)
            ?? throw new NotFoundException("The following training format was not found.");

        trainingFormatModel.IsActive = !trainingFormatModel.IsActive;

        genericRepository.Update(trainingFormatModel);
    }
}
