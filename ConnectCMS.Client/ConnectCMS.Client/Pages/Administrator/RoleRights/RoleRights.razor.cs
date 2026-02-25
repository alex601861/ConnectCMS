using MudBlazor;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Layout.Application;
using CMSTrain.Client.Models.Requests.Menu;
using CMSTrain.Client.Models.Responses.Menu;
using CMSTrain.Client.Models.Responses.User;
using CMSTrain.Client.Models.Requests.Identity;

namespace CMSTrain.Client.Pages.Administrator.RoleRights;

public partial class RoleRights
{
    private Guid RoleId { get; set; }
    
    private List<RolesDto> Roles { get; set; } = [];
    
    public IReadOnlyCollection<Guid> SelectedValues = [];
    
    private RoleMenuRequestDto? MenuRightsRequest { get; set; }
    
    private List<RoleMenuResponseDto>? MenuRightsResponse { get; set; }

    private List<UserResponseDto> GetRoleUsers { get; set; } = [];

    protected override async Task OnInitializedAsync()
    {
        SetPageTitle();

        await GetAllRoles();
    }

    #region Page Title
    [CascadingParameter] public MainLayout Layout { get; set; } = new();

    private void SetPageTitle()
    {
        Layout.PageTitle = PageTitle.RoleRights;
    }
    #endregion

    private async Task GetAllRoles()
    {
        var response = await RoleService.GetAllRoles();

        if (response?.Result is null)
        {
            SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
            
            return;
        }

        Roles = response.Result;
    }

    private async Task HandleRoleMenuSearch()
    {
        try
        {
            var result = await MenuService.GetAllRoleMenus(RoleId);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                
                return;
            }

            var menuRights = result.Result;
            
            SelectedValues = CollectAssignedIds(menuRights);
            
            MenuRightsResponse = menuRights;

            var userRoles = await UserModuleService.GetUsersByRole(roleId: RoleId);
            
            if (userRoles?.Result is null)
            {
                SnackbarService.ShowSnackbar(Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                
                return;
            }

            GetRoleUsers = userRoles.Result;

            foreach (var user in GetRoleUsers)
            {
                user.ImageUrl = string.IsNullOrEmpty(user.ImageUrl)
                    ? null
                    : FileManager.FetchFileUrl(user.ImageUrl, Constants.FilePath.UsersImagesFilePath);
            }
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }

    private async Task SubmitSelectedValues()
    {
        try
        {
            MenuRightsRequest = new RoleMenuRequestDto
            {
                RoleId = RoleId,
                MenuIds = SelectedValues.ToList()
            };

            var result = await MenuService.AssignRoleMenus(MenuRightsRequest);
            
            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }
            
            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
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
        catch (Exception e)
        {
            SnackbarService.ShowSnackbar($"Warning {e.Message}", Severity.Warning, Variant.Outlined);
        }
    }
    
    private HashSet<Guid> CollectAssignedIds(List<RoleMenuResponseDto> menuRights)
    {
        var assignedIds = new HashSet<Guid>();

        CollectIds(menuRights);

        return assignedIds;

        void CollectIds(List<RoleMenuResponseDto> menus)
        {
            foreach (var menu in menus)
            {
                if (menu.IsAssigned)
                {
                    assignedIds.Add(menu.Id);
                }

                if (menu.SubMenus is { Count: > 0 })
                {
                    CollectIds(menu.SubMenus);
                }
            }
        }
    }
}