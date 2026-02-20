using CMSTrain.Domain.Common;
using CMSTrain.Domain.Entities;
using CMSTrain.Domain.Common.Enum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CMSTrain.Application.Settings;
using CMSTrain.Application.Exceptions;
using CMSTrain.Domain.Entities.Identity;
using CMSTrain.Application.Interfaces.Data;
using CMSTrain.Helper;

namespace CMSTrain.Infrastructure.Persistence.Seed;

public class DbInitializer(
    UserManager<User> userManager,
    RoleManager<Role> roleManager,
    IApplicationDbContext dbContext,
    IOptions<MenuSettings> menuSettings,
    IOptions<TraitSettings> traitSettings,
    IWebHostEnvironment webHostEnvironment,
    IOptions<HeadingSettings> headingSettings,
    IOptions<StrategicSettings> strategySettings) : IDbInitializer
{
    private readonly MenuSettings _menuSettings = menuSettings.Value;

    private readonly HeadingSettings _headingSettings = headingSettings.Value;

    private readonly StrategicSettings _strategicSettings = strategySettings.Value;

    private readonly TraitSettings _traitSettings = traitSettings.Value;

    public async Task InitializeIdentityData(CancellationToken cancellationToken = default)
    {
        var user = await InitializeSuperAdmin(cancellationToken);

        await InitializeRoles();

        if (!await userManager.IsInRoleAsync(user, Constants.Roles.Superadmin))
        {
            await userManager.AddToRoleAsync(user, Constants.Roles.Superadmin);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task InitializeRoles()
    {
        var roles = new List<string>()
        {
            Constants.Roles.Superadmin,
            Constants.Roles.Admin,
            Constants.Roles.Trainer,
            Constants.Roles.Client,
            Constants.Roles.Candidate
        };

        var roleIds = new List<Guid>()
        {
            new("def10000-e86b-52c2-2a9a-08dca196874f"),
            new("def10000-e86b-52c2-3a9a-08dca196874f"),
            new("def10000-e86b-52c2-42d2-08dca196874f"),
            new("def10000-e86b-52c2-4389-08dca196874f"),
            new("def10000-e86b-52c2-43de-08dca196874f")
        };

        for (var i = 0; i < roles.Count; i++)
        {
            var role = roles[i];

            var roleExists = await roleManager.RoleExistsAsync(role);

            if (roleExists) continue;

            var newRole = new Role
            {
                Id = roleIds[i],
                Name = role,
                NormalizedName = role.ToUpper()
            };

            var result = await roleManager.CreateAsync(newRole);

            if (result.Succeeded) continue;

            var exceptions = result.Errors.Select(x => x.Description);

            throw new BadRequestException("Failed to create super admin role", exceptions.ToArray());
        }
    }

    private async Task<User> InitializeSuperAdmin(CancellationToken cancellationToken = default)
    {
        var isProduction = webHostEnvironment.IsProduction();
        
        var user = await userManager.FindByIdAsync(Constants.SuperAdmin.Identifier);

        if (user is null)
        {
            var country = await dbContext.Countries.FirstOrDefaultAsync(cancellationToken: cancellationToken);

            var superAdminUser = new User
            {
                Id = new Guid(Constants.SuperAdmin.Identifier),
                Name = isProduction ? Constants.SuperAdmin.Production.Name : Constants.SuperAdmin.Development.Name,
                UserName = isProduction ? Constants.SuperAdmin.Production.EmailAddress : Constants.SuperAdmin.Development.EmailAddress,
                Email = isProduction ? Constants.SuperAdmin.Production.EmailAddress : Constants.SuperAdmin.Development.EmailAddress,
                Gender = GenderType.Male,
                PhoneNumber = "+977-9800000000",
                CountryId = country!.Id,
                NormalizedUserName = isProduction ? Constants.SuperAdmin.Production.EmailAddress.ToUpper() : Constants.SuperAdmin.Development.EmailAddress.ToUpper(),
                NormalizedEmail = isProduction ? Constants.SuperAdmin.Production.EmailAddress.ToUpper() : Constants.SuperAdmin.Development.EmailAddress.ToUpper(),
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D"),
                IsActive = true,
            };

            var result = await userManager.CreateAsync(superAdminUser, isProduction ? Constants.SuperAdmin.Production.DecryptedPassword : Constants.SuperAdmin.Development.DecryptedPassword);

            if (result.Succeeded) return superAdminUser;

            var exceptions = result.Errors.Select(x => x.Description);

            throw new BadRequestException("Failed to create super admin user", exceptions.ToArray());
        }

        user.Name = isProduction ? Constants.SuperAdmin.Production.Name : Constants.SuperAdmin.Development.Name;
        user.UserName = isProduction ? Constants.SuperAdmin.Production.EmailAddress : Constants.SuperAdmin.Development.EmailAddress;
        user.Email = isProduction ? Constants.SuperAdmin.Production.EmailAddress : Constants.SuperAdmin.Development.EmailAddress;
        user.NormalizedEmail = isProduction ? Constants.SuperAdmin.Production.EmailAddress.ToUpper() : Constants.SuperAdmin.Development.EmailAddress.ToUpper();
        user.NormalizedUserName = isProduction ? Constants.SuperAdmin.Production.EmailAddress.ToUpper() : Constants.SuperAdmin.Development.EmailAddress.ToUpper();
        user.PasswordHash = isProduction ? Constants.SuperAdmin.Production.DecryptedPassword.HashPassword() : Constants.SuperAdmin.Development.DecryptedPassword.HashPassword();
        
        user.EmailConfirmed = true;
        user.PhoneNumberConfirmed = true;
        
        await userManager.UpdateAsync(user);
        
        return user;
    }

    public async Task InitializeCountryDataSets(CancellationToken cancellationToken = default)
    {
        var countries = dbContext.Countries.Count();

        if (countries > 0) return;

        var nepal = new Country()
        {
            Name = "Nepal",
            Code = "NP",
            PhoneCode = "+977",
            Icon = "https://cdn.jsdelivr.net/npm/country-flag-emoji-json@2.0.0/dist/images/NP.svg",
            IsActive = true
        };

        var india = new Country()
        {
            Name = "India",
            Code = "IN",
            PhoneCode = "+91",
            Icon = "https://cdn.jsdelivr.net/npm/country-flag-emoji-json@2.0.0/dist/images/IN.svg",
            IsActive = true
        };

        await dbContext.Countries.AddAsync(nepal, cancellationToken);
        await dbContext.Countries.AddAsync(india, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task InitializeDesignationDataSets(CancellationToken cancellationToken = default)
    {
        var designations = dbContext.Designations.Count();

        if (designations > 0) return;

        var salesAssistant = new Designation()
        {
            Title = "Sales Assistant",
            Description = string.Empty,
            IsActive = true
        };
        
        var juniorAssistant = new Designation()
        {
            Title = "Junior Assistant",
            Description = string.Empty,
            IsActive = true
        };
        
        var salesOfficer = new Designation()
        {
            Title = "Sales Officer",
            Description = string.Empty,
            IsActive = true
        };

        await dbContext.Designations.AddAsync(salesAssistant, cancellationToken);
        await dbContext.Designations.AddAsync(juniorAssistant, cancellationToken);
        await dbContext.Designations.AddAsync(salesOfficer, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
    
    public async Task InitializeMenuData(CancellationToken cancellationToken = default)
    {
        await OnInitializeMenuAndRoleAssignment(cancellationToken);
    }

    public async Task InitializeHeadingData(CancellationToken cancellationToken = default)
    {
        await OnInitializeHeadingAndSubHeadingAssignment(cancellationToken);
    }

    public async Task InitializeStrategicData(CancellationToken cancellationToken = default)
    {
        await OnInitializeStrategyAndTraitAssignment(cancellationToken);
    }
    
    public async Task InitializeTraitData(CancellationToken cancellationToken = default)
    {
        await OnInitializePersonalityTraitAssignment(cancellationToken);
    }
    
    private async Task OnInitializeMenuAndRoleAssignment(CancellationToken cancellationToken = default)
    {
        var menus = _menuSettings.Menus;

        foreach (var menu in menus)
        {
            await ProcessMenu(menu, null, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task ProcessMenu(MenuConfiguration menuConfiguration, Guid? parentMenuId, CancellationToken cancellationToken)
    {
        var admin = await userManager.FindByIdAsync(Constants.SuperAdmin.Identifier)
                    ?? throw new NotFoundException("An admin user has not been registered to the system.");

        var existingMenu = await dbContext.Menus.FirstOrDefaultAsync(
            m => m.Id == new Guid(menuConfiguration.Id),
            cancellationToken);

        if (existingMenu != null)
        {
            existingMenu.Title = menuConfiguration.Title;
            existingMenu.Description = menuConfiguration.Description;
            existingMenu.Sequence = menuConfiguration.Sequence;
            existingMenu.Url = menuConfiguration.Url;
            existingMenu.IsActive = menuConfiguration.IsActive;
            existingMenu.ParentMenuId = parentMenuId;

            dbContext.Menus.Update(existingMenu);
        }
        else
        {
            var newMenu = new Menu
            {
                Id = new Guid(menuConfiguration.Id),
                Title = menuConfiguration.Title,
                Description = menuConfiguration.Description,
                Sequence = menuConfiguration.Sequence,
                Url = menuConfiguration.Url,
                IsActive = menuConfiguration.IsActive,
                CreatedBy = admin.Id,
                CreatedAt = DateTime.UtcNow,
                ParentMenuId = parentMenuId
            };

            await dbContext.Menus.AddAsync(newMenu, cancellationToken);

            existingMenu = newMenu;
        }

        if (menuConfiguration.ChildMenus != null)
        {
            foreach (var childMenu in menuConfiguration.ChildMenus)
            {
                await ProcessMenu(childMenu, existingMenu.Id, cancellationToken);
            }
        }
    }
    
    private async Task OnInitializeHeadingAndSubHeadingAssignment(CancellationToken cancellationToken = default)
    {
        var admin = await dbContext.Users.FirstOrDefaultAsync(cancellationToken: cancellationToken)
                    ?? throw new NotFoundException("An admin user has not been registered to the system.");

        var headings = _headingSettings.Headings;

        var existingHeadings = await dbContext.Headings.ToListAsync(cancellationToken: cancellationToken);

        if (existingHeadings.Count > 0)
        {
            return;
        }

        foreach (var heading in headings)
        {
            var headingId = Guid.NewGuid();
            
            var headingModel = new Heading
            {
                Id = headingId,
                Title = heading.Title,
                Description = "",
                CreatedBy = admin.Id,
                CreatedAt = DateTime.UtcNow,
                Type = HeadingType.Heading,
                Facet = Enum.TryParse<FacetType>(heading.Facet, out var facet) ? facet : FacetType.None,
                Inspection = Enum.TryParse<InspectionType>(heading.Inspection, out var inspection) ? inspection : InspectionType.Feedback
            };

            await dbContext.Headings.AddAsync(headingModel, cancellationToken);

            if (heading.SubHeadings == null) continue;

            foreach (var subHeading in heading.SubHeadings)
            {
                var subHeadingModel = new Heading
                {
                    Title = subHeading.Title,
                    ParentHeadingId = headingId,
                    Description = "",
                    CreatedBy = admin.Id,
                    CreatedAt = DateTime.UtcNow,
                    Type = HeadingType.Subheading,
                    Facet = headingModel.Facet,
                    Inspection = headingModel.Inspection
                };

                await dbContext.Headings.AddAsync(subHeadingModel, cancellationToken);
            }
        }
        
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task OnInitializeStrategyAndTraitAssignment(CancellationToken cancellationToken = default)
    {
        var admin = await dbContext.Users.FirstOrDefaultAsync(cancellationToken: cancellationToken)
                    ?? throw new NotFoundException("An admin user has not been registered to the system.");

        var strategies = _strategicSettings.StrategicTraits;

        var existingStrategies = await dbContext.StrategicTrait.ToListAsync(cancellationToken: cancellationToken);

        if (existingStrategies.Count > 0)
        {
            return;
        }

        foreach (var strategy in strategies)
        {
            var strategyId = Guid.NewGuid();

            var strategyType = Enum.TryParse<StrategicType>(strategy.Type, out var strategicType) ? strategicType : StrategicType.Strength;

            var strategicTraitModel = new StrategicTrait()
            {
                Id = strategyId,
                Name = strategy.Title,
                Description = "",
                Type = strategyType,
                CreatedBy = admin.Id,
                CreatedAt = DateTime.UtcNow,
            };
            
            await dbContext.StrategicTrait.AddAsync(strategicTraitModel, cancellationToken);

            if (strategy.Opportunities is { Count: > 0 })
            {
                foreach (var opportunity in strategy.Opportunities)
                {
                    var opportunityId = Guid.NewGuid();
                    
                    var opportunityModel = new StrategicTrait()
                    {
                        Id = opportunityId,
                        Name = opportunity.Title,
                        Description = "",
                        Type = StrategicType.Opportunity,
                        CreatedBy = admin.Id,
                        CreatedAt = DateTime.UtcNow,
                    };
                    
                    await dbContext.StrategicTrait.AddAsync(opportunityModel, cancellationToken);
                    
                    var strategicTraitOpportunity = new StrategicTraitDetails()
                    {
                        TraitId = strategicTraitModel.Id,
                        DetailId = opportunityModel.Id,
                        CreatedBy = admin.Id,
                        CreatedAt = DateTime.UtcNow,
                    };
                    
                    await dbContext.StrategicTraitDetails.AddAsync(strategicTraitOpportunity, cancellationToken);
                }
            }
            
            if (strategy.Threats is { Count: > 0 })
            {
                foreach (var threat in strategy.Threats)
                {
                    var threatId = Guid.NewGuid();
                    
                    var threatModel = new StrategicTrait()
                    {
                        Id = threatId,
                        Name = threat.Title,
                        Description = "",
                        Type = StrategicType.Threat,
                        CreatedBy = admin.Id,
                        CreatedAt = DateTime.UtcNow,
                    };
                    
                    await dbContext.StrategicTrait.AddAsync(threatModel, cancellationToken);
                    
                    var strategicTraitOpportunity = new StrategicTraitDetails()
                    {
                        TraitId = strategicTraitModel.Id,
                        DetailId = threatModel.Id,
                        CreatedBy = admin.Id,
                        CreatedAt = DateTime.UtcNow,
                    };
                    
                    await dbContext.StrategicTraitDetails.AddAsync(strategicTraitOpportunity, cancellationToken);
                }
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
    
    private async Task OnInitializePersonalityTraitAssignment(CancellationToken cancellationToken = default)
    {
        var admin = await dbContext.Users.FirstOrDefaultAsync(cancellationToken: cancellationToken)
                    ?? throw new NotFoundException("An admin user has not been registered to the system.");

        var personalityTraits = _traitSettings.PersonalityTraits;

        var existingPersonalityTraits = await dbContext.PersonalityTraits.ToListAsync(cancellationToken: cancellationToken);

        if (existingPersonalityTraits.Count > 0)
        {
            return;
        }

        foreach (var personalityTrait in personalityTraits)
        {
            var personalityTraitModel = new PersonalityTrait()
            {
                Title = personalityTrait.Title,
                Description = "",
                Type = Enum.TryParse<TraitType>(personalityTrait.Type, out var personalityTraitType) ? personalityTraitType : TraitType.Openness,
                CreatedBy = admin.Id,
                CreatedAt = DateTime.UtcNow,
            };
            
            await dbContext.PersonalityTraits.AddAsync(personalityTraitModel, cancellationToken);
        }
        
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

