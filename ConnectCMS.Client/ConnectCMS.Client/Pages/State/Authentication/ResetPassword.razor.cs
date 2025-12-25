using MudBlazor;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Requests.Identity;
using CMSTrain.Client.Layout.Application;

namespace CMSTrain.Client.Pages.State.Authentication;

public partial class ResetPassword : ComponentBase
{
    private ForgotPasswordEmailRequestDto ResetPasswordDto { get; set; } = new();
    
    private bool BusySubmitting { get; set; }

    [Parameter] public string Token { get; set; } = "";
    
    [Parameter] public Guid UserId { get; set; }

    protected override void OnInitialized()
    {
        SetPageTitle();
        ResetPasswordDto.Token = Token;
        ResetPasswordDto.UserId = UserId;
    }

    #region Page Title
    [CascadingParameter] public MainLayout Layout { get; set; } = new();

    private void SetPageTitle()
    {
        Layout.PageTitle = PageTitle.ResetPassword;
    }
    #endregion

    private async Task HandleValidSubmit()
    {
        BusySubmitting = true;
        
        try
        {
            var result = await AuthenticationService.ResetPassword(ResetPasswordDto);
            
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
                    NavigationManager.NavigateTo("/login");
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
}