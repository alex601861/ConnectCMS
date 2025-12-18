using CMSTrain.Domain.Common;
using Microsoft.AspNetCore.Identity;
using CMSTrain.Domain.Entities.Identity;
using CMSTrain.Application.DTOs.Identity;
using CMSTrain.Application.Exceptions;
using CMSTrain.Application.Interfaces.Repositories.Base;
using CMSTrain.Application.Interfaces.Services.Identity;

namespace CMSTrain.Identity.Implementation.Services;

public class RoleService(RoleManager<Role> roleManager, IGenericRepository genericRepository) : IRoleService
{
    public List<RolesDto> GetAllRoles()
    {
        var roles = roleManager.Roles.ToList();

        var result = roles.Select(x => new RolesDto()
        {
            Id = x.Id,
            Name = x.Name!,
        }).ToList();

        return result;
    }

    public List<RolesDto> GetAllRoles(int pageNumber, int pageSize, out int rowCount, string? search = null)
    {
        var roles = genericRepository.GetPagedResult<Role>(pageNumber, pageSize, out int dataCount, 
            x => (string.IsNullOrEmpty(search) || x.Name.ToLower().Contains(search.ToLower()))).ToList();

        rowCount = dataCount;

        var result = roles.Select(x => new RolesDto()
        {
            Id = x.Id,
            Name = x.Name!,
        }).ToList();

        return result;
    }

    public List<RolesDto> GetPrecedingRoles()
    {
        var roles = roleManager.Roles.ToList();
        
        var result = roles
            .Where(x => x.Name is not Constants.Roles.Superadmin)
            .Select(x => new RolesDto
            {
                Id = x.Id,
                Name = x.Name!,
            })
            .ToList();

        return result;
    }

    public async Task InsertRole(RolesDto role)
    {
        var roleExists = await roleManager.RoleExistsAsync(role.Name);

        if (roleExists)
        {
            var classException = new[]
            {
                "The role with this name already exists.",
            };
            
            throw new BadRequestException("The role is not valid", classException);
        }

        var newRole = new Role()
        {
            Name = role.Name,
            NormalizedName = role.Name.ToUpper(),
        };

        await roleManager.CreateAsync(newRole);
    }

    public async Task UpdateRole(RolesDto role)
    {
        var roleExists = await roleManager.RoleExistsAsync(role.Name);

        if (!roleExists)
        {
            var classException = new[]
            {
                "The role doesn't exists.",
            };
            
            throw new BadRequestException("The role is not valid", classException);
        }

        var existingRole = await roleManager.FindByIdAsync(role.Id.ToString())
            ?? throw new NotFoundException("The role was not found.");

        existingRole.Name = role.Name;
        existingRole.NormalizedName = role.Name.ToUpper();

        await roleManager.UpdateAsync(existingRole);
    }

    public async Task DeleteRole(Guid roleId)
    {
        var existingRole = await roleManager.FindByIdAsync(roleId.ToString())
            ?? throw new NotFoundException("The role was not found.");

        await roleManager.DeleteAsync(existingRole);
    }
}