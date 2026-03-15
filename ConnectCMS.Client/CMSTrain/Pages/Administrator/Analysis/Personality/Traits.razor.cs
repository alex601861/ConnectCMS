using MudBlazor;
using CMSTrain.Client.Models.Base;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Layout.Application;
using CMSTrain.Client.Models.Requests.PersonalityTrait;
using CMSTrain.Client.Models.Responses.PersonalityTrait;

namespace CMSTrain.Client.Pages.Administrator.Analysis.Personality;

public partial class Traits
{
    private int ActivePanelIndex { get; set; }

    private bool IsEditModalOpen { get; set; }

    [Parameter] public TraitType TraitType { get; set; }

    protected override async Task OnInitializedAsync()
    {
        SetPageTitle();

        await GetAllPersonalityTraits();
    }

    #region Page Title

    [CascadingParameter] public MainLayout Layout { get; set; } = new();

    private void SetPageTitle()
    {
        Layout.PageTitle = PageTitle.Facet;
    }

    #endregion

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

        await GetAllPersonalityTraits();
    }

    #endregion

    #region Update Traits

    private GetPersonalityTraitDto GetPersonalityTrait { get; set; } = new();
    private UpdatePersonalityTraitDto UpdatePersonalityTraitDto { get; set; } = new();

    private bool _isUpdateButtonDisabled;

    private bool IsUpdateTraitButtonDisabled
    {
        get => _isUpdateButtonDisabled ||
               string.IsNullOrEmpty(UpdatePersonalityTraitDto.Description);
        set => _isUpdateButtonDisabled = value;
    }
    
    private void HandleTraitUpdateBusySubmit(bool isBusySubmitting)
    {
        IsUpdateTraitButtonDisabled = isBusySubmitting;

        StateHasChanged();
    }

    private void OpenCloseEditModal()
    {
        IsEditModalOpen = !IsEditModalOpen;

        StateHasChanged();
    }

    private async Task OpenTraitUpdateModal(Guid traitId)
    {
        var response = await PersonalityTraitService.GetPersonalityTraitById(traitId);

        if (response?.Result is null)
        {
            SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error,
                Variant.Outlined);
            return;
        }

        GetPersonalityTrait = response.Result;

        UpdatePersonalityTraitDto = new UpdatePersonalityTraitDto()
        {
            Id = GetPersonalityTrait.Id,
            Description = GetPersonalityTrait.Description,
        };

        OpenCloseEditModal();
    }

    private async Task UpdateTrait(bool isClosed)
    {
        if (isClosed)
        {
            OpenCloseEditModal();

            return;
        }

        try
        {
            HandleTraitUpdateBusySubmit(true);

            var result = await PersonalityTraitService.UpdatePersonalityTrait(UpdatePersonalityTraitDto);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error,
                    Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    OpenCloseEditModal();
                    await GetAllPersonalityTraits();
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
        finally
        {
            HandleTraitUpdateBusySubmit(false);
        }
    }
    #endregion

    #region Traits

    private CollectionDto<GetPersonalityTraitDto>? PersonalityTraits { get; set; }

    private async Task UpdatePageNumber(int pageNumber)
    {
        await GetAllPersonalityTraits(pageNumber: pageNumber);
    }

    private async Task UpdatePageSize(int pageSize)
    {
        await GetAllPersonalityTraits(pageSize: pageSize);
    }

    private async Task GetAllPersonalityTraits(int pageNumber = Constants.Pagination.Page,
        int pageSize = Constants.Pagination.Size)
    {
        try
        {
            var result = await PersonalityTraitService.GetAllPersonalityTraits(pageNumber, pageSize, Search);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error,
                    Variant.Outlined);
                return;
            }

            PersonalityTraits = result;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }

    #endregion
}