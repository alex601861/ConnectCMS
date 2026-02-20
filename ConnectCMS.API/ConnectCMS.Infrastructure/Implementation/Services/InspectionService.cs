using CMSTrain.Domain.Common;
using CMSTrain.Domain.Entities;
using CMSTrain.Domain.Common.Enum;
using CMSTrain.Application.Exceptions;
using CMSTrain.Application.DTOs.Inspection;
using CMSTrain.Application.DTOs.Questionnaires;
using CMSTrain.Application.Interfaces.Services;
using CMSTrain.Application.Interfaces.Repositories.Base;
using CMSTrain.Helper;

namespace CMSTrain.Infrastructure.Implementation.Services;

public class InspectionService(IGenericRepository genericRepository, IFileService fileService) : IInspectionService
{
    private const string InspectionFilePath = Constants.FilePath.InspectionsImagesFilePath;

    public List<GetInspectionDto> GetAllInspections(int pageNumber, int pageSize, out int rowCount, string? search = null, bool? isActive = null)
    {
        var inspections = genericRepository.GetPagedResult<Inspection>(pageNumber, pageSize, out rowCount,
            x => (string.IsNullOrEmpty(search) || x.Name.ToLower().Contains(search)) && 
                 (isActive == null || x.IsActive == isActive)).ToList();

        return inspections.Select(x => new GetInspectionDto
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            ImageUrl = x.ImageUrl,
            Type = x.InspectionType.ToInspectionType(),
            IsActive = x.IsActive,
            HasAssignedQuestionnaire = x.InspectionType == InspectionType.SwotAnalysis || 
                                       genericRepository.GetCount<InspectionQuestionnaires>(z => z.InspectionId == x.Id) != 0
        }).ToList();
    }
    
    public List<GetInspectionDto> GetAllInspections(string? search = null, bool? isActive = null)
    {
        var inspections = genericRepository.Get<Inspection>(x => 
            (string.IsNullOrEmpty(search) || x.Name.ToLower().Contains(search)) && 
            (isActive == null || x.IsActive == isActive)).ToList();

        return inspections.Select(x => new GetInspectionDto()
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            ImageUrl = x.ImageUrl,
            Type = x.InspectionType.ToInspectionType(),
            IsActive = x.IsActive,
            HasAssignedQuestionnaire = x.InspectionType == InspectionType.SwotAnalysis || 
                                       genericRepository.GetCount<InspectionQuestionnaires>(z => z.InspectionId == x.Id) != 0
        }).ToList();
    }

    public GetInspectionDto GetInspectionById(Guid inspectionId)
    {
        var inspection = genericRepository.GetById<Inspection>(inspectionId)
            ?? throw new NotFoundException("The following inspection could not be found.");

        return new GetInspectionDto()
        {
            Id = inspection.Id,
            Name = inspection.Name,
            Description = inspection.Description,
            ImageUrl = inspection.ImageUrl,
            Type = inspection.InspectionType.ToInspectionType(),
            IsActive = inspection.IsActive,
            HasAssignedQuestionnaire = inspection.InspectionType == InspectionType.SwotAnalysis || 
                                       genericRepository.GetCount<InspectionQuestionnaires>(z => z.InspectionId == inspection.Id) != 0
        };
    }

    public GetInspectionDto GetInspectionByType(InspectionType inspectionType)
    {
        var inspection = genericRepository.GetFirstOrDefault<Inspection>(x => x.InspectionType == inspectionType)
                         ?? throw new NotFoundException("The following inspection could not be found.");

        return new GetInspectionDto()
        {
            Id = inspection.Id,
            Name = inspection.Name,
            Description = inspection.Description,
            ImageUrl = inspection.ImageUrl,
            Type = inspection.InspectionType.ToInspectionType(),
            IsActive = inspection.IsActive,
            HasAssignedQuestionnaire = inspection.InspectionType == InspectionType.SwotAnalysis || 
                                       genericRepository.GetCount<InspectionQuestionnaires>(z => z.InspectionId == inspection.Id) != 0
        };
    }
    
    public List<GetInspectionDto> GetAllAvailableTrainingInspections()
    {
        var inspections = genericRepository.Get<Inspection>(x => x.IsActive).ToList();

        return inspections.Select(x => new GetInspectionDto()
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            ImageUrl = x.ImageUrl,
            Type = x.InspectionType.ToInspectionType(),
            IsActive = x.IsActive,
            HasAssignedQuestionnaire = x.InspectionType == InspectionType.SwotAnalysis || 
                                       genericRepository.GetCount<InspectionQuestionnaires>(z => z.InspectionId == x.Id) != 0
        }).ToList();
    }

    public List<GetInspectionDto> GetAllAvailableTrainingInspections(int pageNumber, int pageSize, out int rowCount)
    {
        var inspections = genericRepository.GetPagedResult<Inspection>(pageNumber, pageSize, out rowCount, x => x.IsActive).ToList();

        return inspections.Select(x => new GetInspectionDto()
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            ImageUrl = x.ImageUrl,
            Type = x.InspectionType.ToInspectionType(),
            IsActive = x.IsActive,
            HasAssignedQuestionnaire = x.InspectionType == InspectionType.SwotAnalysis || 
                                       genericRepository.GetCount<InspectionQuestionnaires>(z => z.InspectionId == x.Id) != 0
        }).ToList();
    }
    
    public List<GetInspectionDto> GetAllAssignedTrainingInspections(Guid trainingId, string? search = null)
    {
        var training = genericRepository.GetById<Training>(trainingId) 
                       ?? throw new NotFoundException("The respective training could not be found.");

        var trainingInspections = genericRepository.Get<TrainingInspection>(x => x.TrainingId == training.Id).ToList();

        var result = new List<GetInspectionDto>();
        
        foreach (var trainingInspection in trainingInspections)
        {
            var inspection = genericRepository.GetById<Inspection>(trainingInspection.InspectionId)
                             ?? throw new NotFoundException("The respective inspection could not be found.");
            
            if (!string.IsNullOrEmpty(search) && !inspection.Name.ToLower().Contains(search)) continue;
            
            result.Add(new GetInspectionDto()
            {
                Id = inspection.Id,
                Name = inspection.Name,
                Description = inspection.Description,
                ImageUrl = inspection.ImageUrl,
                Type = inspection.InspectionType.ToInspectionType(),
                IsActive = inspection.IsActive,
                HasAssignedQuestionnaire = inspection.InspectionType == InspectionType.SwotAnalysis || 
                                           genericRepository.GetCount<InspectionQuestionnaires>(z => z.InspectionId == inspection.Id) != 0
            });
        }

        return result;
    }

    public List<GetInspectionDto> GetAllAssignedTrainingInspections(Guid trainingId, int pageNumber, int pageSize, out int rowCount, string? search = null)
    {
        var training = genericRepository.GetById<Training>(trainingId) 
                       ?? throw new NotFoundException("The respective training could not be found.");

        var trainingInspections = genericRepository.GetPagedResult<TrainingInspection>(pageNumber, pageSize, out rowCount, x => x.TrainingId == training.Id).ToList();

        var result = new List<GetInspectionDto>();
        
        foreach (var trainingInspection in trainingInspections)
        {
            var inspection = genericRepository.GetById<Inspection>(trainingInspection.InspectionId)
                             ?? throw new NotFoundException("The respective inspection could not be found.");
            
            if (!string.IsNullOrEmpty(search) && !inspection.Name.ToLower().Contains(search)) continue;

            result.Add(new GetInspectionDto()
            {
                Id = inspection.Id,
                Name = inspection.Name,
                Description = inspection.Description,
                ImageUrl = inspection.ImageUrl,
                Type = inspection.InspectionType.ToInspectionType(),
                IsActive = inspection.IsActive,
                HasAssignedQuestionnaire = inspection.InspectionType == InspectionType.SwotAnalysis || 
                                           genericRepository.GetCount<InspectionQuestionnaires>(z => z.InspectionId == inspection.Id) != 0
            });
        }

        return result;
    }
    
    public void InsertInspection(CreateInspectionDto inspection)
    {
        var existingInspection = genericRepository.GetFirstOrDefault<Inspection>(x => x.Name == inspection.Name && x.InspectionType != InspectionType.Others);

        if (existingInspection != null)
        {
            var exception = new[]
            {
                "An existing inspection with the same name or type already exists in the system"
            };
            
            throw new BadRequestException("The following inspection could not be inserted", exception);
        }

        var inspectionModel = new Inspection
        {
            Name = inspection.Name,
            Description = inspection.Description,
            ImageUrl = inspection.Image != null 
                ? fileService.UploadDocument(inspection.Image, InspectionFilePath)
                : string.Empty,
            InspectionType = inspection.InspectionType,
        };

        genericRepository.Insert(inspectionModel);
    }

    public void UpdateInspection(UpdateInspectionDto inspection)
    {
        var inspectionModel = genericRepository.GetById<Inspection>(inspection.Id)
            ?? throw new NotFoundException("The following inspection could not be found.");

        inspectionModel.Name = inspection.Name;
        inspectionModel.Description = inspection.Description;

        if (inspection.Image != null)
        {
            var inspectionPath = Path.Combine(InspectionFilePath, inspectionModel.ImageUrl);

            fileService.DeleteFile(inspectionPath);
            
            var imageUrl = fileService.UploadDocument(inspection.Image, InspectionFilePath);
            
            inspectionModel.ImageUrl = imageUrl;
        }
        
        genericRepository.Update(inspectionModel);
    }

    public void ActivateDeactivateInspection(Guid inspectionId)
    {
        var inspection = genericRepository.GetById<Inspection>(inspectionId)
            ?? throw new NotFoundException("The following inspection could not be found.");

        inspection.IsActive = !inspection.IsActive;
        
        genericRepository.Update(inspection);
    }

    // TODO: Removal of unnecessary questionnaire traits.
    public void UploadInspectionQuestionnaires(UploadInspectionQuestionnaireDto inspectionQuestionnaires)
    {
        var inspectionModel = genericRepository.GetById<Inspection>(inspectionQuestionnaires.InspectionId)
                              ?? throw new NotFoundException("The following inspection could not be found.");

        var inspectionQuestionnaire = genericRepository.GetFirstOrDefault<InspectionQuestionnaires>(x => x.InspectionId == inspectionModel.Id);

        if (inspectionQuestionnaire != null)
        {
            var questionnaireModel = genericRepository.GetById<Questionnaire>(inspectionQuestionnaire.QuestionnaireId);

            if (questionnaireModel != null)
            {
                var questionnaireDetails = genericRepository.Get<QuestionnaireDetails>(x => x.QuestionnaireId == questionnaireModel.Id).ToList();
                
                var answers = genericRepository.Get<Answer>(x => (x.QuestionId != null && questionnaireDetails.Select(z => z.Id).Contains(x.QuestionId.Value)) || x.IsAnswerForInspection && x.InspectionId == inspectionModel.Id).ToList();
                
                if (answers.Count > 0)
                {
                    genericRepository.RemoveMultipleEntity(answers);
                }
                
                if (questionnaireDetails.Count > 0)
                {
                    genericRepository.RemoveMultipleEntity(questionnaireDetails);
                }
                
                genericRepository.Delete(inspectionQuestionnaire);

                genericRepository.Delete(questionnaireModel);
            }
        }
        
        var questionnaire = new Questionnaire
        {
            IsQuestionnaireForTraining = false,
            QuestionDetails = inspectionQuestionnaires.QuestionnaireDetails.Select((x, index) => new QuestionnaireDetails()
            {
                Title = x.Title,
                QuestionType = x.Type,
                HeadingId = x.HeadingId,
                IsParentHeading = x.IsParentHeading,
                Order = index + 1,
                QuestionnaireTraits = inspectionModel.InspectionType == InspectionType.PersonalityTest 
                    ? x.TraitTypes?.Select(z => new QuestionnaireTraits() 
                    {
                        TraitType = z
                    }).ToList() 
                    : [],
                HasUniqueAnswers = inspectionQuestionnaires.Answers != null && !HasPredefinedAnswers(x.Answers, inspectionQuestionnaires.Answers.Where(z => z.QuestionType == x.Type.ToString()).ToList()),
                Answers = inspectionQuestionnaires.Answers != null 
                    ? !HasPredefinedAnswers(x.Answers, inspectionQuestionnaires.Answers.Where(z => z.QuestionType == x.Type.ToString()).ToList()) 
                        ? x.Answers.Select((z, order) => new Answer() 
                        {
                            IsAnswerForInspection = false,
                            IsAnswerForQuestion = true,
                            AnswerType = x.Type, 
                            Title = z.Title,
                            Order = order + 1,
                            IsSelectable = true
                        }).ToList() 
                        : [] 
                    : x.Answers.Select(z => new Answer()
                    {
                        IsAnswerForInspection = false,
                        IsAnswerForQuestion = true,
                        AnswerType = x.Type, 
                        Title = z.Title,
                        IsSelectable = true
                    }).ToList()
            }).ToList()
        };

        if (inspectionQuestionnaires.RequiresPredefinedAnswers)
        {
            var answers = inspectionQuestionnaires.Answers?.Select((z, order) => new Answer
            {
                IsAnswerForInspection = true,
                IsAnswerForQuestion = false,
                InspectionId = inspectionModel.Id,
                Order = order + 1,
                AnswerType = z.QuestionType != null ? Enum.Parse<QuestionType>(z.QuestionType) : QuestionType.SingleSelectMcq,
                QuestionId = null,
                Title = z.Title,
                IsSelectable = true
            }).ToList();
     
            if (answers != null && answers.Count != 0)
            {
                genericRepository.AddMultipleEntity(answers);
            }
        }
        
        var questionnaireId = genericRepository.Insert(questionnaire);
        
        genericRepository.Insert(new InspectionQuestionnaires()
        {
            InspectionId = inspectionModel.Id,
            QuestionnaireId = questionnaireId
        });
    }

    private static bool HasPredefinedAnswers(List<McqAnswerDetailsDto> uploadedAnswers, List<McqAnswerDetailsDto> predefinedAnswers)
    {
        return uploadedAnswers.Count == predefinedAnswers.Count && 
               uploadedAnswers.All(answer => 
                   predefinedAnswers.Any(x => 
                       x.Title == answer.Title));
    }
}