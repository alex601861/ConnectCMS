using CMSTrain.Domain.Entities;
using CMSTrain.Application.DTOs.Menu;
using CMSTrain.Application.Common.User;
using CMSTrain.Domain.Entities.Identity;
using CMSTrain.Application.Interfaces.Services;
using CMSTrain.Application.Interfaces.Repositories.Base;

namespace CMSTrain.Infrastructure.Implementation.Services;

public class MenuService(IGenericRepository genericRepository, ICurrentUserService currentUserService) : IMenuService
{
    public void AssignMenus(RoleMenuRequestDto roleMenu)
    {
        var roleId = roleMenu.RoleId;

        var role = genericRepository.GetById<Role>(roleId)!;

        genericRepository.DeleteMultipleEntity<RoleRights>(x => x.RoleId == role.Id);

        var menusToAdd = new List<Guid>();

        foreach (var menuId in roleMenu.MenuIds)
        {
            AddMenuWithAncestors(menuId, menusToAdd);
            AddMenuWithDescendants(menuId, menusToAdd);
        }

        foreach (var menuId in menusToAdd)
        {
            var menu = genericRepository.GetById<Menu>(menuId);

            if (menu == null) continue;

            var existingMenu = genericRepository.Get<RoleRights>(rr => rr.RoleId == role.Id && rr.MenuId == menu.Id);

            if (existingMenu.Any()) continue;

            var roleRight = new RoleRights()
            {
                Id = Guid.NewGuid(),
                RoleId = role.Id,
                MenuId = menu.Id,
            };

            genericRepository.Insert(roleRight);
        }
    }

    public List<RoleMenuResponseDto> GetAllRoleMenus(Guid roleId)
    {
        var role = genericRepository.GetById<Role>(roleId)!;

        var rootMenus = genericRepository.Get<Menu>(m => m.ParentMenuId == null, includeProperties: "ChildMenus").OrderBy(x => x.Sequence).ToList();

        foreach (var rootMenu in rootMenus)
        {
            LoadChildMenus(rootMenu);
        }

        var roleRights = genericRepository.Get<RoleRights>(rr => rr.RoleId == role.Id);

        var result = rootMenus.Select(menu => MapRoleRightToRoleRightDto(menu, roleRights)).ToList();

        return result;
    }

    public List<RoleMenuResponseDto> GetAllAssignedMenus()
    {
        var userInRole = currentUserService.GetUserRole;

        var role = genericRepository.GetFirstOrDefault<Role>(r => r.Name == userInRole); 

        var roleRights = genericRepository.Get<RoleRights>(rr => rr.RoleId == role!.Id); 

        var rootMenus = genericRepository.Get<Menu>(m => m.ParentMenuId == null , includeProperties: "ChildMenus").OrderBy(x => x.Sequence).ToList();

        foreach (var rootMenu in rootMenus)
        {
            LoadChildMenus(rootMenu);
        }

        var result = rootMenus
               .Where(menu => roleRights.Any(rr => rr.MenuId == menu.Id))
               .Select(menu => MapRoleRightToRoleRightDto(menu, roleRights))
               .Select(FilterMenu)
               .ToList();

        return result;
    }
    
    private void LoadChildMenus(Menu menu)
    {
        var childMenus = genericRepository.Get<Menu>(m => m.ParentMenuId == menu.Id, includeProperties: "ChildMenus").OrderBy(x => x.Sequence).ToList();
        
        menu.ChildMenus = childMenus;

        foreach (var childMenu in childMenus)
        {
            LoadChildMenus(childMenu);
        }
    }

    private void AddMenuWithAncestors(Guid menuId, List<Guid> menuSet)
    {
        var menu = genericRepository.Get<Menu>(m => m.Id == menuId, includeProperties: "ParentMenuModule").FirstOrDefault();

        if (menu == null) return;

        menuSet.Add(menuId);

        if (menu.ParentMenuId.HasValue)
        {
            AddMenuWithAncestors(menu.ParentMenuId.Value, menuSet);
        }
    }

    private void AddMenuWithDescendants(Guid menuId, List<Guid> menuSet)
    {
        var menu = genericRepository.Get<Menu>(m => m.Id == menuId, includeProperties: "ChildMenus").FirstOrDefault();

        if (menu == null) return;

        menuSet.Add(menuId);

        if (menu.ChildMenus == null || !menu.ChildMenus.Any()) return;

        foreach (var childMenu in menu.ChildMenus)
        {
            AddMenuWithDescendants(childMenu.Id, menuSet);
        }
    }

    private static RoleMenuResponseDto FilterMenu(RoleMenuResponseDto menu)
    {
        var assignedSubMenus = menu.SubMenus
            .Select(FilterMenu)
            .Where(subMenu => subMenu != null!)
            .ToList();

        if (!menu.IsAssigned && assignedSubMenus.Count == 0) return null!;
        
        menu.SubMenus = assignedSubMenus;
        
        return menu;
    }
    
    private static RoleMenuResponseDto MapRoleRightToRoleRightDto(Menu menu, IQueryable<RoleRights> roleRights)
    {
        var isAssigned = roleRights.Any(rr => rr.MenuId == menu.Id);

        return new RoleMenuResponseDto
        {
            Id = menu.Id,
            Title = menu.Title,
            Description = menu.Description,
            Sequence = menu.Sequence,
            Url = menu.Url,
            IsAssigned = isAssigned,
            SubMenus = menu.ChildMenus?.OrderBy(x => x.Sequence).Select(child => MapRoleRightToRoleRightDto(child, roleRights)).ToList() ?? []
        };
    }
}
