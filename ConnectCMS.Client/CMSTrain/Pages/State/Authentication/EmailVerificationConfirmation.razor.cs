using CMSTrain.Client.Layout.Application;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Requests.Identity;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CMSTrain.Client.Pages.State.Authentication;

public partial class EmailVerificationConfirmation : ComponentBase
{
    [Parameter] public string UserId { get; set; } = "";
    
    [Parameter] public string Token { get; set; } = "";

    private bool IsVerified { get; set; }

    protected override async Task OnInitializedAsync()
    {
        SetPageTitle();

        var emailVerification = new EmailVerificationRequestDto()
        {
            UserId = UserId,
            VerificationCode = Token
        };

        try
        {
            var result = await AuthenticationService.VerifyEmailConfirmation(emailVerification);
            
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
                    IsVerified = true;
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

    #region Page Title
    [CascadingParameter] public MainLayout Layout { get; set; } = new();

    private void SetPageTitle()
    {
        Layout.PageTitle = PageTitle.EmailVerificationConfirmation;
    }
    #endregion
}