using CMSTrain.Client.Models.Base;
using MudBlazor;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Requests.Identity;
using CMSTrain.Client.Models.Requests.Subordinate;
using CMSTrain.Client.Models.Responses.Identity;
using CMSTrain.Client.Models.Responses.Subordinate;
using CMSTrain.Client.Models.Responses.TrainingCandidate;

namespace CMSTrain.Client.Pages.State.Candidate;

public partial class CandidateSubordinateDetails : ComponentBase
{
    [Parameter] public Guid TrainingCandidateId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await GetRoles();

        await GetAllSubordinates();

        await GetClientUserDetails();
        
        await GetTrainingCandidateAssignmentDetails();
    }

    #region Search and Filter
    private string _search = string.Empty;

    private string Search
    {
        get => _search;
        set
        {
            if (_search == value) return;
            _search = value;
            _ = OnSearchInputAsync(_search);
        }
    }

    private async Task OnSearchInputAsync(string search)
    {
        Search = search;
        
        PageSize = Constants.Pagination.Size;

        PageNumber = Constants.Pagination.Page;

        await GetAllSubordinates();
    }

    private int? Type { get; set; }

    private async Task OnSubordinatesFilter()
    {
        PageSize = Constants.Pagination.Size;

        PageNumber = Constants.Pagination.Page;
        
        Subordinates = null;
        
        await GetAllSubordinates();
    }
    #endregion
    
    #region Subordinate Details
    private CollectionDto<GetSubordinateDto>? Subordinates { get; set; }
    
    private int PageNumber { get; set; } = Constants.Pagination.Page;
    
    private int PageSize { get; set; } = Constants.Pagination.Size; 
    
    private async Task UpdatePageNumber(int pageNumber)
    {
        PageNumber = pageNumber;
        
        Subordinates = null;
        
        await GetAllSubordinates();
    }

    private async Task UpdatePageSize(int pageSize)
    {
        PageNumber = 1;
        
        PageSize = pageSize;
        
        Subordinates = null;
        
        await GetAllSubordinates();
    }
    
    private async Task GetAllSubordinates()
    {
        try
        {
            var response = await SubordinateService.GetSubordinateDetailsForTrainingCandidate(TrainingCandidateId, PageNumber, PageSize);

            if (response?.Result is null)
            {
                SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            Subordinates = response;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
        
    }
    #endregion

    #region Training Candidate
    private TrainingCandidateAssignmentDetailsDto TrainingCandidateAssignment { get; set; } = new();

    private async Task GetTrainingCandidateAssignmentDetails()
    {
        try
        {
            var result = await TrainingCandidateService.GetTrainingCandidateAssignmentDetails(TrainingCandidateId);
            
            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }
            
            TrainingCandidateAssignment = result.Result;
        }   
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion
    
    #region Roles and Assignments
    private RolesDto Role { get; set; } = new();

    private async Task GetRoles()
    {
        try
        {
            var response = await ProfileService.GetUserRole();

            if (response?.Result is null)
            {
                SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            Role = response.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion
    
    #region Candidate Details
    private UserDetail ClientUserDetails { get; set; } = new();

    private async Task GetClientUserDetails()
    {
        try
        {
            var result = await ProfileService.GetUserProfile();
            
            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }
            
            ClientUserDetails = result.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion
    
    #region Create Subordinates
    private bool IsCreateModalOpen { get; set; }

    private CreateClientSubordinateDto CreateCandidateSubordinateDto { get; set; } = new();
    
    private bool IsCreateButtonDisabled =>
        string.IsNullOrEmpty(CreateCandidateSubordinateDto.SubordinateDetails.Name) ||
        string.IsNullOrEmpty(CreateCandidateSubordinateDto.SubordinateDetails.Email) ||
        string.IsNullOrEmpty(CreateCandidateSubordinateDto.SubordinateDetails.ContactNumber);

    private void OpenCloseCreateSubordinateModal()
    {
        IsCreateModalOpen = !IsCreateModalOpen;

        StateHasChanged();
    }

    private void OpenCreateSubordinateModal()
    {
        CreateCandidateSubordinateDto = new CreateClientSubordinateDto()
        {
            TrainingCandidateId = TrainingCandidateId
        };

        OpenCloseCreateSubordinateModal();
    }

    private async Task InsertSubordinate(bool isClosed)
    {
        if (isClosed)
        {
            OpenCloseCreateSubordinateModal();

            return;
        }

        try
        {
            var result = await SubordinateService.InsertSubordinateForCandidates(CreateCandidateSubordinateDto);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);

                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    await GetAllSubordinates();
                    OpenCloseCreateSubordinateModal();
                    SnackbarService.ShowSnackbar(result.Message, Severity.Success, Variant.Outlined);
                    break;
                case StatusCode.Status404NotFound:
                case StatusCode.Status400BadRequest:
                case StatusCode.Status401Unauthorized:
                    SnackbarService.ShowSnackbar(result.Message, Severity.Warning, Variant.Outlined);
                    break;
                case StatusCode.Status500InternalServerError:
                    SnackbarService.ShowSnackbar(result.Message, Severity.Error, Variant.Outlined);
                    break;
            }
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion
}