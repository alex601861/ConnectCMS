using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Requests.Identity;
using MudBlazor;

namespace CMSTrain.Client.Pages.State.Profile;

public partial class ChangePassword
{
    private bool IsBusySubmitting { get; set; }

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

    #region Handle Change Password
    private ChangePasswordRequestDto ChangePasswordDto { get; set; } = new();

    private bool ValidatePassword()
    {
        if (string.IsNullOrWhiteSpace(ChangePasswordDto.CurrentPassword) ||
            string.IsNullOrWhiteSpace(ChangePasswordDto.NewPassword) ||
            string.IsNullOrWhiteSpace(ChangePasswordDto.ConfirmPassword))
        {
            return false;
        }

        if (ChangePasswordDto.NewPassword != ChangePasswordDto.ConfirmPassword || ChangePasswordDto.NewPassword.Length < 8)
        {
            return false;
        }

        return true;    
    }
    
    private async Task ChangePasswordAsync()
    {
        IsBusySubmitting = true;

        try
        {
            var result = await ProfileService.ChangePassword(ChangePasswordDto);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error,
                    Variant.Outlined);
                IsBusySubmitting = false;
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    SnackbarService.ShowSnackbar(result.Message, Severity.Success, Variant.Outlined);
                    await AuthenticationService.LogOut();
                    NavigationManager.NavigateTo("/login", true);
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

        IsBusySubmitting = false;
    }
    #endregion
}