using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Requests.Identity;
using CMSTrain.Client.Models.Responses.Country;
using CMSTrain.Client.Models.Responses.Designation;
using CMSTrain.Client.Models.Responses.Identity;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CMSTrain.Client.Pages.State.Profile;

public partial class PersonalDetails
{
    [Parameter] public EventCallback OnProfileDetailsUpdate { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        await GetUserDetails();
        
        await GetAllCountries();

        await GetAllDesignations();
    }
    
    #region Meta Data Details
    private List<GetCountryDto> Countries { get; set; } = [];

    private async Task GetAllCountries()
    {
        var response = await CountryService.GetAllCountries(isActive: true);

        if (response?.Result is null)
        {
            SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
            
            return;
        }
        
        Countries = response.Result;
    }
    
    private List<GetDesignationDto> Designations { get; set; } = [];

    private async Task GetAllDesignations()
    {
        var response = await DesignationService.GetAllDesignations(isActive: true);

        if (response?.Result is null)
        {
            SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
            
            return;
        }
        
        Designations = response.Result;
    }
    #endregion
    
    #region User Details
    private UserDetail UserProfile { get; set; } = new();

    private async Task GetUserDetails()
    {
        var response = await ProfileService.GetUserProfile();

        if (response?.Result is null)
        {
            SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);

            return;
        }

        UserProfile = response.Result;

        if (UserProfile.Id != Guid.Empty)
        {
            ProfileUpdate = new ProfileRequestDto
            {
                Name = UserProfile.Name,
                PhoneNumber = UserProfile.PhoneNumber,
                Email = UserProfile.Email,
                CountryId = UserProfile.CountryId,
                Gender = Enum.TryParse(UserProfile.Gender, out GenderType gender) ? gender : GenderType.Other,
                Address = UserProfile.Address,
                DesignationId = UserProfile.DesignationId,
            };
        }
    }
    #endregion
    
    #region Update User Profile
    private bool BusySubmitting { get; set; }

    private ProfileRequestDto ProfileUpdate { get; set; } = new();

    private bool ValidateProfileDetails()
    {
        return !string.IsNullOrWhiteSpace(ProfileUpdate.Email) &&
               !string.IsNullOrWhiteSpace(ProfileUpdate.Name) &&
               !string.IsNullOrWhiteSpace(ProfileUpdate.PhoneNumber) &&
               ProfileUpdate.CountryId != Guid.Empty;
    }
    
    private async Task HandleValidSubmit()
    {
        BusySubmitting = true;

        try
        {
            var result = await ProfileService.UpdateProfile(ProfileUpdate);
            
            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                BusySubmitting = false;
                return;
            }
            
            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    await GetUserDetails();
                    await OnProfileDetailsUpdate.InvokeAsync();
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
        
        BusySubmitting = false;
    }
    #endregion
}