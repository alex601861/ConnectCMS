using System.Data;
using CMSTrain.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using CMSTrain.Domain.Entities.Identity;
using CMSTrain.Application.Common.Service;

namespace CMSTrain.Application.Interfaces.Data;

public interface IApplicationDbContext : IScopedService
{
    #region Identity
    DbSet<User> Users { get; set; }

    DbSet<Role> Roles { get; set; }

    DbSet<UserRoles> UserRoles { get; set; }

    DbSet<UserClaims> UserClaims { get; set; }

    DbSet<RoleClaims> RoleClaims { get; set; }

    DbSet<UserToken> UserToken { get; set; }

    DbSet<UserLogin> UserLogin { get; set; }
    #endregion

    #region Modules
    DbSet<Answer> Answers { get; set; }

    DbSet<Attendance> Attendances { get; set; }

    DbSet<Certificate> Certificates { get; set; }

    DbSet<Class> Classes { get; set; }

    DbSet<ClassResources> ClassResources { get; set; }

    DbSet<ClassTrainer> ClassTrainers { get; set; }

    DbSet<Country> Countries { get; set; }

    DbSet<Designation> Designations { get; set; }

    DbSet<Heading> Headings { get; set; }

    DbSet<Inspection> Inspections { get; set; }

    DbSet<Menu> Menus { get; set; }

    DbSet<Organization> Organizations { get; set; }

    DbSet<PersonalityTrait> PersonalityTraits { get; set; }

    DbSet<Questionnaire> Questionnaires { get; set; }

    DbSet<QuestionnaireDetails> QuestionDetails { get; set; }
    
    DbSet<RefreshToken> RefreshTokens { get; set; }

    DbSet<Resource> Resources { get; set; }

    DbSet<RoleRights> RoleRights { get; set; }

    DbSet<StrategicTrait> StrategicTrait { get; set; }

    DbSet<StrategicTraitDetails> StrategicTraitDetails { get; set; }

    DbSet<StrategicTraitResponse> StrategicTraitResponse { get; set; }
    
    DbSet<StrategicTraitResponseDetails> StrategicTraitResponseDetails { get; set; }
    
    DbSet<Subordinate> Subordinates { get; set; }

    DbSet<Training> Trainings { get; set; }

    DbSet<TrainingCandidate> TrainingCandidates { get; set; }

    DbSet<TrainingFormat> TrainingFormats { get; set; }

    DbSet<TrainingInspection> TrainingInspections { get; set; }

    DbSet<TrainingResources> TrainingResources { get; set; }

    DbSet<UserResponse> UserResponses { get; set; }

    DbSet<UserResponseAnalysis> UserResponseAnalyses { get; set; }

    DbSet<UserResponseDetails> UserResponseDetails { get; set; }
    #endregion

    #region Functions
    int SaveChanges();

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    #endregion

    #region Properties
    IDbConnection Connection { get; }
    #endregion
}