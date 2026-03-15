using MudBlazor;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Layout.Application;
using CMSTrain.Client.Models.Requests.Email;

namespace CMSTrain.Client.Pages.State.Authentication;

public partial class ForgotPassword : ComponentBase
{
    protected override void OnInitialized()
    {
        SetPageTitle();
    }

    #region Page Title
    [CascadingParameter] public MainLayout Layout { get; set; } = new();

    private void SetPageTitle()
    {
        Layout.PageTitle = PageTitle.ForgotPassword;
    }
    #endregion
    
    #region Password Entry and Form Submission
    private ForgotPasswordRequestDto ForgotPasswordDto { get; set; } = new();

    private bool BusySubmitting { get; set; }
    
    private void HandleBusySubmitting(bool isBusySubmitting)
    {
        BusySubmitting = isBusySubmitting;
    }
    
    private async Task HandleValidSubmit()
    {
        HandleBusySubmitting(true);

        try
        {
            var result = await EmailConfirmationService.ForgotPassword(ForgotPasswordDto);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error,
                    Variant.Outlined);
                BusySubmitting = false;
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