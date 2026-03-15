using MudBlazor;
using CMSTrain.Client.Models.Base;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Requests.Strategy;
using CMSTrain.Client.Models.Responses.Strategy;

namespace CMSTrain.Client.Pages.Administrator.Analysis.SWOT;

public partial class Traits
{
    [Parameter] public StrategicType StrategicType { get; set; }

    [Parameter] public EventCallback OnTraitCountUpdate { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await GetAllStrategicTraits();
    }
    
    #region Search
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
        
        await GetAllStrategicTraits();
    }
    #endregion
    
    #region Strategic Trait Details
    private CollectionDto<GetStrategyDto>? StrategicTraitDetails { get; set; }
    
    private int PageNumber { get; set; } = Constants.Pagination.Page;
    
    private int PageSize { get; set; } = Constants.Pagination.Size; 

    private async Task UpdatePageNumber(int pageNumber)
    {
        PageNumber = pageNumber;
        
        StrategicTraitDetails = null;
        
        await GetAllStrategicTraits();
    }

    private async Task UpdatePageSize(int pageSize)
    {
        PageNumber = 1;
        
        PageSize = pageSize;
        
        StrategicTraitDetails = null;
        
        await GetAllStrategicTraits();
    }
    
    private async Task GetAllStrategicTraits()
    {
        try
        {
            var response = await StrategicTraitService.GetAllStrategies(StrategicType, PageNumber, PageSize, Search);

            if (response?.Result is null)
            {
                SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            StrategicTraitDetails = response;
            
            StateHasChanged();
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion
    
    #region Upload Strategic Trait (Any Strategies)
    private bool IsStrategicTraitUploadModalOpen { get; set; }
    
    private InsertStrategyDto UploadStrategy { get; set; } = new();
    
    private bool _isCreateButtonDisabled;

    private bool IsStrategicTraitCreateButtonDisabled
    {
        get => _isCreateButtonDisabled ||
               string.IsNullOrEmpty(UploadStrategy.Name) ||
               string.IsNullOrEmpty(UploadStrategy.Description) ||
               UploadStrategy.Type == StrategicType.None;
        set => _isCreateButtonDisabled = value;
    }
    
    private void HandleStrategicTraitBusySubmit(bool isBusySubmitting)
    {
        IsStrategicTraitCreateButtonDisabled = isBusySubmitting;
        
        StateHasChanged();
    }
    
    private void OpenStrategicTraitUploadModal()
    {
        OpenCloseStrategicTraitUploadModal();

        UploadStrategy = new InsertStrategyDto()
        {
            Type = StrategicType
        };
    }

    private async Task UploadStrategicTrait(bool isClosed)
    {
        if (isClosed)
        {
            OpenCloseStrategicTraitUploadModal();
            
            return;
        }

        try
        {
            HandleStrategicTraitBusySubmit(true);

            var result = await StrategicTraitService.InsertStrategy(UploadStrategy);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error,
                    Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    await GetAllStrategicTraits();
                    OpenCloseStrategicTraitUploadModal();
                    SnackbarService.ShowSnackbar(result.Message, Severity.Success, Variant.Outlined);
                    await OnTraitCountUpdate.InvokeAsync();
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
        finally
        {
            HandleStrategicTraitBusySubmit(false);
        }
    }

    private void OpenCloseStrategicTraitUploadModal()
    {
        IsStrategicTraitUploadModalOpen = !IsStrategicTraitUploadModalOpen;
        
        StateHasChanged();
    }
    #endregion

    #region Update Strategic Trait (Parent and Child Traits)
    private bool IsStrategicTraitUpdateModalOpen { get; set; }
    
    private UpdateStrategyDto UpdateStrategyTrait { get; set; } = new();
    
    private bool _isUpdateButtonDisabled;
    
    private bool IsStrategicTraitUpdateButtonDisabled
    {
        get => _isUpdateButtonDisabled || 
               string.IsNullOrEmpty(UpdateStrategyTrait.Name) ||
               string.IsNullOrEmpty(UpdateStrategyTrait.Description) ||
               UpdateStrategyTrait.Type == StrategicType.None;
        set => _isUpdateButtonDisabled = value;
    }
    
    private void HandleStrategicTraitUpdateBusySubmit(bool isBusySubmitting)
    {
        IsStrategicTraitUpdateButtonDisabled = isBusySubmitting;
        
        StateHasChanged();
    }

    private async Task OpenStrategicTraitUpdateModal(Guid strategicTraitId)
    {
        var result = await StrategicTraitService.GetStrategyById(strategicTraitId);

        if (result?.Result is null)
        {
            SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
            return;
        }
        
        OpenCloseStrategicTraitUpdateModal();

        var strategicTrait = result.Result;
        
        UpdateStrategyTrait = new ()
        {
            Id = strategicTrait.Id,
            Name = strategicTrait.Name,
            Description = strategicTrait.Description,
            Type = (StrategicType)Enum.Parse(typeof(StrategicType), strategicTrait.Type)
        };
    }

    private async Task UpdateStrategicTrait(bool isClosed)
    {
        if (isClosed)
        {
            OpenCloseStrategicTraitUpdateModal();
            
            return;
        }

        try
        {
            HandleStrategicTraitUpdateBusySubmit(true);

            var result = await StrategicTraitService.UpdateStrategy(UpdateStrategyTrait);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error,
                    Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    await GetAllStrategicTraits();
                    OpenCloseStrategicTraitUpdateModal();
                    SnackbarService.ShowSnackbar(result.Message, Severity.Success, Variant.Outlined);
                    await OnTraitCountUpdate.InvokeAsync();
                    break;
                case StatusCode.Status202Accepted:
                    await GetAllStrategicTraits();
                    OpenCloseStrategicTraitUpdateModal();
                    SnackbarService.ShowSnackbar(result.Message, Severity.Warning, Variant.Outlined);
                    await OnTraitCountUpdate.InvokeAsync();
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
        finally
        {
            HandleStrategicTraitUpdateBusySubmit(false);
        }
    }

    private void OpenCloseStrategicTraitUpdateModal()
    {
        IsStrategicTraitUpdateModalOpen = !IsStrategicTraitUpdateModalOpen;
        
        StateHasChanged();
    }
    #endregion
    
    #region Strategic Trait Details
    private IEnumerable<Guid> SelectedOpportunityIds { get; set; } = [];

    private IEnumerable<Guid> SelectedThreatIds { get; set; } = [];

    private GetStrategyDto StrategyDetails { get; set; } = new();
    
    private bool IsStrategicTraitDetailsModalOpen { get; set; }
    
    private async Task OpenStrategicTraitDetailsModal(Guid strategyId)
    {
        await GetStrategicTraitDetails();

        var response = await StrategicTraitService.GetStrategyById(strategyId);
            
        if (response?.Result is null)
        {
            SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
            
            return;
        }
            
        StrategyDetails = response.Result;   
        
        SelectedOpportunityIds = StrategyDetails.Opportunities.Select(x => x.Id);
            
        SelectedThreatIds = StrategyDetails.Threats.Select(x => x.Id);
        
        OpenCloseStrategicTraitDetailsModal();
    }

    private async Task UploadStrategicTraitDetails(bool isClosed)
    {
        if (isClosed)
        {
            OpenCloseStrategicTraitDetailsModal();
            return;
        }

        try
        {
            var strategyDetails = new UploadStrategyDetailsDto()
            {
                StrategyId = StrategyDetails.Id,
                Opportunities = SelectedOpportunityIds.ToList(),
                Threats = SelectedThreatIds.ToList()
            };
            
            var result = await StrategicTraitService.UploadStrategyDetails(strategyDetails);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    await GetAllStrategicTraits();
                    OpenCloseStrategicTraitDetailsModal();
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
    
    private void OpenCloseStrategicTraitDetailsModal()
    {
        IsStrategicTraitDetailsModalOpen = !IsStrategicTraitDetailsModalOpen;
        
        StateHasChanged();
    }
    #endregion

    #region Trait Details
    private GetStrategyDetailsDto TraitDetails { get; set; } = new();

    private async Task GetStrategicTraitDetails()
    {
        try
        {
            var response = await StrategicTraitService.GetStrategyDetails();
            
            if (response?.Result is null)
            {
                SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
            
                return;
            }
            
            TraitDetails = response.Result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    
    private bool IsAllStrategicTraitDetailsModalOpen { get; set; }

    private GetStrategyDto OpenedStrategicDetails { get; set; } = new();

    private string OpenedStrategy { get; set; } = string.Empty;
    
    private void OpenCloseAllStrategicTraitDetailsModal()
    {
        IsAllStrategicTraitDetailsModalOpen = !IsAllStrategicTraitDetailsModalOpen;

        if (!IsAllStrategicTraitDetailsModalOpen)
        {
            OpenedStrategicDetails = new();
            OpenedStrategy = string.Empty;
        }
        
        StateHasChanged();
    }
    
    private void OpenAllStrategicTraitDetailsModal(GetStrategyDto strategy, StrategicType strategicType)
    {
        OpenedStrategicDetails = strategy;

        OpenedStrategy = strategicType.ToString();
        
        OpenCloseAllStrategicTraitDetailsModal();
    }
    #endregion
}