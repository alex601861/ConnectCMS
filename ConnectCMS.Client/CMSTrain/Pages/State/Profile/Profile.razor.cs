using MudBlazor;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Layout.Application;
using CMSTrain.Client.Models.Requests.File;
using CMSTrain.Client.Models.Requests.Identity;
using CMSTrain.Client.Models.Responses.Country;
using CMSTrain.Client.Models.Responses.Identity;
using Microsoft.AspNetCore.Components.Forms;

namespace CMSTrain.Client.Pages.State.Profile;

public partial class Profile : ComponentBase
{
    protected override async Task OnInitializedAsync()
    {
        SetPageTitle();
        
        await GetUserDetails();
    }

    #region Page Title
    [CascadingParameter] public MainLayout Layout { get; set; } = new();

    private void SetPageTitle()
    {
        Layout.PageTitle = PageTitle.Profile;
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

        if (Layout.GlobalState != null)
        {
            Layout.GlobalState.Name = UserProfile.Name;
            Layout.GlobalState.ImageUrl = UserProfile.ImageUrl;
            Layout.GlobalState.EmailAddress = UserProfile.Email;
        }
    }
    #endregion

    #region Update User Image
    private bool OpenImageDrawerToggle { get; set; } 
    
    private ProfileImageRequestDto ProfileImageUpdate { get; set; } = new();

    private void ToggleImageEdit()
    {
        OpenImageDrawerToggle = !OpenImageDrawerToggle;
    }
    
    private async Task HandleValidImageSubmit(FileUploadResultDto fileUpload)
    {
        try
        {
            ProfileImageUpdate.ImageUrl = fileUpload.File;
            
            var result = await ProfileService.UpdateProfileImage(ProfileImageUpdate);
            
            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }
            
            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    ToggleImageEdit();
                    await GetUserDetails();
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

    #region Tab Active Index
    private int ActivePanelIndex { get; set; }
    #endregion
    
    #region Profile Update
    public async Task UpdateProfileDetails()
    {
        await GetUserDetails();
    }
    #endregion
}