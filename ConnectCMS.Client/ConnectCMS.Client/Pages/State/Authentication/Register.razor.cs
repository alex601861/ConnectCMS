using MudBlazor;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Requests.Identity;
using CMSTrain.Client.Models.Responses.Identity;
using CMSTrain.Client.Layout.Application;
using CMSTrain.Client.Models.Requests.Email;
using CMSTrain.Client.Models.Requests.File;

namespace CMSTrain.Client.Pages.State.Authentication;

public partial class Register : ComponentBase
{
    protected override void OnInitialized()
    {
        SetPageTitle();
    }

    #region Page Title
    [CascadingParameter] public MainLayout Layout { get; set; } = new();

    private void SetPageTitle()
    {
        Layout.PageTitle = PageTitle.Register;
    }
    #endregion

    private bool OpenRegisterImageDrawerToggle { get; set; }
    
    private void ToggleRegisterImage()
    {
        OpenRegisterImageDrawerToggle = !OpenRegisterImageDrawerToggle;
    }

    private void HandleRegisterImageUpload(FileUploadResultDto fileUpload)
    {
        RegisterDto.ImageUrl = fileUpload.File;
        ImageUrl = fileUpload.Base64File;
        ToggleRegisterImage();
        StateHasChanged();
    }
    
    #region Password Visibility
    private bool PasswordVisibility { get; set; }

    private InputType PasswordInput { get; set; } = InputType.Password;
    
    private string PasswordInputIcon { get; set; } = Icons.Material.Filled.VisibilityOff;
    
    private void TogglePasswordVisibility()
    {
        if (PasswordVisibility)
        {
            PasswordVisibility = false;
            PasswordInputIcon = Icons.Material.Filled.VisibilityOff;
            PasswordInput = InputType.Password;
        }
        else
        {
            PasswordVisibility = true;
            PasswordInputIcon = Icons.Material.Filled.Visibility;
            PasswordInput = InputType.Text;
        }
    }
    #endregion
    
    #region Form Submission
    private bool _busySubmitting;
    
    private bool BusySubmitting
    {
        get => _busySubmitting || 
               string.IsNullOrEmpty(RegisterDto.Email) || 
               string.IsNullOrEmpty(RegisterDto.Password) || 
               string.IsNullOrEmpty(RegisterDto.ConfirmPassword) || 
               string.IsNullOrEmpty(RegisterDto.Name) || 
               string.IsNullOrEmpty(RegisterDto.PhoneNumber)
               || RegisterDto.Gender == null;
        set => _busySubmitting = value;
    }

    private string? ImageUrl { get; set; }

    private CandidateRegisterDto RegisterDto { get; set; } = new();

    private void HandleBusySubmitting(bool isBusySubmitting)
    {
        BusySubmitting = isBusySubmitting;
        
        StateHasChanged();
    }
    
    private async Task HandleValidSubmit()
    {
        HandleBusySubmitting(true);
        
        try
        {
            var result = await AuthenticationService.SelfRegister(RegisterDto);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                BusySubmitting = false;
                return;
            }
            
            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    SnackbarService.ShowSnackbar(result.Message, Severity.Success, Variant.Outlined);
                    await HandleEmailConfirmation(result.Result);
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
            HandleBusySubmitting(false);
        }
    }

    private async Task HandleEmailConfirmation(RegistrationResponseDto registration)
    {
        try
        {
            HandleBusySubmitting(true);

            var emailConfirmation = new RegistrationEmailRequestDto()
            {
                UserId = registration.UserId
            };

            var result = await EmailConfirmationService.SelfRegistration(emailConfirmation);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    SnackbarService.ShowSnackbar(result.Message, Severity.Success, Variant.Outlined);
                    NavigationManager.NavigateTo("login");
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
            HandleBusySubmitting(false);
        }
    }
    #endregion
}