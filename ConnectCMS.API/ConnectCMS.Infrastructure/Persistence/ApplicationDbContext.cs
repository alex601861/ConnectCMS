using System.Data;
using CMSTrain.Helper;
using Newtonsoft.Json;
using System.Reflection;
using CMSTrain.Domain.Common;
using CMSTrain.Domain.Entities;
using CMSTrain.Domain.Common.Base;
using CMSTrain.Application.Settings;
using Microsoft.EntityFrameworkCore;
using CMSTrain.Domain.Common.Property;
using CMSTrain.Application.Exceptions;
using CMSTrain.Application.Common.User;
using CMSTrain.Domain.Entities.Identity;
using Microsoft.Extensions.Configuration;
using CMSTrain.Application.Interfaces.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace CMSTrain.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentUserService? currentUserService = null) :
    IdentityDbContext<User, Role, Guid, UserClaims, UserRoles, UserLogin, RoleClaims, UserToken>(options),
    IApplicationDbContext
{
    #region Identity Tables
    public override DbSet<User> Users { get; set; }

    public override DbSet<Role> Roles { get; set; }

    public override DbSet<UserRoles> UserRoles { get; set; }

    public override DbSet<UserClaims> UserClaims { get; set; }

    public override DbSet<RoleClaims> RoleClaims { get; set; }
    
    public DbSet<UserToken> UserToken { get; set; }

    public DbSet<UserLogin> UserLogin { get; set; }
    #endregion

    #region Other Tables
    public DbSet<Answer> Answers { get; set; }

    public DbSet<Attendance> Attendances { get; set; }

    public DbSet<Certificate> Certificates { get; set; }

    public DbSet<Class> Classes { get; set; }

    public DbSet<ClassConfiguration> ClassConfigurations { get; set; }

    public DbSet<ClassResources> ClassResources { get; set; }

    public DbSet<ClassTrainer> ClassTrainers { get; set; }

    public DbSet<Country> Countries { get; set; }

    public DbSet<Designation> Designations { get; set; }

    public DbSet<Heading> Headings { get; set; }

    public DbSet<Inspection> Inspections { get; set; }

    public DbSet<InspectionQuestionnaires> InspectionQuestions { get; set; }

    public DbSet<Menu> Menus { get; set; }

    public DbSet<Organization> Organizations { get; set; }

    public DbSet<PersonalityTrait> PersonalityTraits { get; set; }
    
    public DbSet<Questionnaire> Questionnaires { get; set; }

    public DbSet<QuestionnaireDetails> QuestionDetails { get; set; }

    public DbSet<QuestionnaireTraits> QuestionTraits { get; set; }
    
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public DbSet<Resource> Resources { get; set; }

    public DbSet<RoleRights> RoleRights { get; set; }

    public DbSet<StrategicTrait> StrategicTrait { get; set; }

    public DbSet<StrategicTraitDetails> StrategicTraitDetails { get; set; }

    public DbSet<StrategicTraitResponse> StrategicTraitResponse { get; set; }
    
    public DbSet<StrategicTraitResponseDetails> StrategicTraitResponseDetails { get; set; }
    
    public DbSet<Subordinate> Subordinates { get; set; }

    public DbSet<Training> Trainings { get; set; }

    public DbSet<TrainingCandidate> TrainingCandidates { get; set; }

    public DbSet<TrainingConfiguration> TrainingConfigurations { get; set; }

    public DbSet<TrainingFormat> TrainingFormats { get; set; }

    public DbSet<TrainingInspection> TrainingInspections { get; set; }

    public DbSet<TrainingInspectionConfiguration> TrainingInspectionConfigurations { get; set; }

    public DbSet<TrainingResources> TrainingResources { get; set; }

    public DbSet<UserResponse> UserResponses { get; set; }

    public DbSet<UserResponseAnalysis> UserResponseAnalyses { get; set; }

    public DbSet<UserResponseDetails> UserResponseDetails { get; set; }
    #endregion

    public override int SaveChanges()
    {
        UpdateLogs();

        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateLogs();

        return await base.SaveChangesAsync(cancellationToken);
    }

    public IDbConnection Connection => Database.GetDbConnection();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var basePath = AppContext.BaseDirectory;

        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        if (!Directory.Exists(basePath))
        {
            throw new DirectoryNotFoundException($"The directory '{basePath}' does not exist.");
        }
            
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
            .Build();

        var databaseSettings = new DatabaseSettings();

        configuration.GetSection("DatabaseSettings").Bind(databaseSettings);

        var connectionString = databaseSettings.DbProvider == Constants.DbProviderKeys.Npgsql
            ? databaseSettings.NpgSqlConnectionString
            : databaseSettings.SqlServerConnectionString;

        // optionsBuilder.UseLazyLoadingProxies();
        
        optionsBuilder = optionsBuilder.UseDatabase(databaseSettings.DbProvider, connectionString!);

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);

        #region Identity Entities Naming Conventions
        builder.Entity<User>().ToTable("Users");
        builder.Entity<Role>().ToTable("Roles");
        builder.Entity<UserToken>().ToTable("Tokens");
        builder.Entity<UserRoles>().ToTable("UserRoles");
        builder.Entity<RoleClaims>().ToTable("RoleClaims");
        builder.Entity<UserClaims>().ToTable("UserClaims");
        builder.Entity<UserLogin>().ToTable("LoginAttempts");
        #endregion

        #region User Model Configurations
        builder.Entity<Training>()
            .HasOne(t => t.CreatedUser)
            .WithMany(u => u.Trainings)
            .HasForeignKey(t => t.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<TrainingCandidate>()
            .HasOne(tc => tc.CreatedUser)
            .WithMany()
            .HasForeignKey(tc => tc.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<TrainingCandidate>()
            .HasOne(t => t.DeletedUser)      
            .WithMany()
            .HasForeignKey(t => t.DeletedBy)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<TrainingCandidate>()
            .HasOne(t => t.LastModifiedUser)      
            .WithMany()
            .HasForeignKey(t => t.LastModifiedBy)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<Training>()
            .HasOne(t => t.DeletedUser)      
            .WithMany()
            .HasForeignKey(t => t.DeletedBy)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<TrainingInspection>()
            .HasOne(ti => ti.CreatedUser)
            .WithMany()
            .HasForeignKey(ti => ti.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<TrainingResources>()
            .HasOne(ti => ti.CreatedUser)
            .WithMany()
            .HasForeignKey(ti => ti.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<Subordinate>()
            .HasOne(s => s.CreatedUser)
            .WithMany()
            .HasForeignKey(s => s.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<Questionnaire>()
            .HasOne(q => q.CreatedUser)
            .WithMany()
            .HasForeignKey(q => q.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<QuestionnaireDetails>()
            .HasOne(q => q.CreatedUser)
            .WithMany()
            .HasForeignKey(q => q.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<Answer>()
            .HasOne(q => q.CreatedUser)
            .WithMany()
            .HasForeignKey(q => q.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<UserResponse>()
            .HasOne(q => q.CreatedUser)
            .WithMany()
            .HasForeignKey(q => q.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<UserResponseDetails>()
            .HasOne(q => q.CreatedUser)
            .WithMany()
            .HasForeignKey(q => q.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<QuestionnaireTraits>()
            .HasOne(q => q.CreatedUser)
            .WithMany()
            .HasForeignKey(q => q.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<UserResponseAnalysis>()
            .HasOne(q => q.CreatedUser)
            .WithMany()
            .HasForeignKey(q => q.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<Attendance>()
            .HasOne(q => q.CreatedUser)
            .WithMany()
            .HasForeignKey(q => q.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<Certificate>()
            .HasOne(q => q.CreatedUser)
            .WithMany()
            .HasForeignKey(q => q.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<Class>()
            .HasOne(q => q.CreatedUser)
            .WithMany()
            .HasForeignKey(q => q.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<ClassResources>()
            .HasOne(q => q.CreatedUser)
            .WithMany()
            .HasForeignKey(q => q.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<ClassTrainer>()
            .HasOne(q => q.CreatedUser)
            .WithMany()
            .HasForeignKey(q => q.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<Inspection>()
            .HasOne(q => q.CreatedUser)
            .WithMany()
            .HasForeignKey(q => q.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<Menu>()
            .HasOne(q => q.CreatedUser)
            .WithMany()
            .HasForeignKey(q => q.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<Organization>()
            .HasOne(q => q.CreatedUser)
            .WithMany()
            .HasForeignKey(q => q.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<Resource>()
            .HasOne(q => q.CreatedUser)
            .WithMany()
            .HasForeignKey(q => q.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<RoleRights>()
            .HasOne(q => q.CreatedUser)
            .WithMany()
            .HasForeignKey(q => q.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
        #endregion

        #region JSON Configuration
        builder.Entity<TrainingConfiguration>()
            .Property(e => e.PropertyPair)
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<KeyValueProperty>(v)!)
            .HasColumnType("jsonb");
        
        builder.Entity<ClassConfiguration>()
            .Property(e => e.PropertyPair)
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<KeyValueProperty>(v)!)
            .HasColumnType("jsonb");
        
        builder.Entity<TrainingInspectionConfiguration>()
            .Property(e => e.PropertyPair)
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<KeyValueProperty>(v)!)
            .HasColumnType("jsonb");
        
        builder.Entity<UserResponseAnalysis>()
            .Property(e => e.Description)
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<KeyValueProperty>(v)!)
            .HasColumnType("jsonb");
        
        builder.Entity<UserResponseAnalysis>()
            .Property(e => e.Scores)
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<KeyValueProperty>(v)!)
            .HasColumnType("jsonb");
        
        builder.Entity<Certificate>()
            .Property(e => e.Description)
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<KeyValueProperty>(v)!)
            .HasColumnType("jsonb");
        
        builder.Entity<Certificate>()
            .Property(e => e.Score)
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<KeyValueProperty>(v)!)
            .HasColumnType("jsonb");
        #endregion
        
        #region Relationship Mapping Configurations
        builder.Entity<User>()
            .HasOne(u => u.Organization)
            .WithMany() 
            .HasForeignKey(u => u.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict); 

        builder.Entity<User>()
            .HasOne(u => u.Country)
            .WithMany() 
            .HasForeignKey(u => u.CountryId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<TrainingCandidate>()
            .HasOne(tc => tc.Candidate)
            .WithMany(u => u.TrainingCandidates)
            .HasForeignKey(tc => tc.CandidateId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<TrainingResources>()
            .HasOne(tr => tr.Training)
            .WithMany(t => t.TrainingResources)
            .HasForeignKey(tr => tr.TrainingId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<UserResponse>()
            .HasOne(q => q.Candidate)
            .WithMany()
            .HasForeignKey(q => q.CandidateId)
            .OnDelete(DeleteBehavior.Restrict);
                
        builder.Entity<Answer>()
            .HasOne(a => a.QuestionDetail)
            .WithMany(q => q.Answers)
            .HasForeignKey(a => a.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<Class>()
            .HasOne(c => c.Training)
            .WithMany(t => t.Classes)
            .HasForeignKey(c => c.TrainingId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<ClassResources>()
            .HasOne(tr => tr.Class)
            .WithMany(t => t.ClassResources)
            .HasForeignKey(tr => tr.ClassId)
            .OnDelete(DeleteBehavior.Restrict);
        #endregion
    }

    private void UpdateLogs()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e is { Entity: BaseEntity<Guid> or BaseEntity<string>, State: EntityState.Added or EntityState.Modified or EntityState.Deleted });

        if (currentUserService == null)
        {
            throw new NotFoundException("The following user has not been registered to our system.");
        }
        
        var dateTime = ExtensionMethod.GetUtcDate();
        
        var userId = currentUserService.GetUserId;

        foreach (var entry in entries)
        {
            UpdateEntityLog(entry.Entity, entry.State, userId, dateTime);
        }
    }

    private static void UpdateEntityLog<TEntity>(TEntity entity, EntityState state, Guid userId, DateTime dateTime)
        where TEntity : class
    {
        switch (state)
        {
            case EntityState.Added:
            {
                switch (entity)
                {
                    case BaseEntity<Guid> guidEntity:
                        guidEntity.CreatedAt = dateTime;
                        if (guidEntity.CreatedBy == Guid.Empty) guidEntity.CreatedBy = userId;
                        break;
                    case BaseEntity<string> stringEntity:
                        stringEntity.CreatedAt = dateTime;
                        if (stringEntity.CreatedBy == Guid.Empty) stringEntity.CreatedBy = userId;
                        break;
                }
                break;
            }

            case EntityState.Modified:
            {
                switch (entity)
                {
                    case BaseEntity<Guid> guidEntity:
                        guidEntity.LastModifiedAt = dateTime;
                        if (guidEntity.LastModifiedBy == Guid.Empty) guidEntity.LastModifiedBy = userId;
                        break;
                    case BaseEntity<string> stringEntity:
                        stringEntity.LastModifiedAt = dateTime;
                        if (stringEntity.LastModifiedBy == Guid.Empty) stringEntity.LastModifiedBy = userId;
                        break;
                }
                break;
            }

            case EntityState.Deleted:
            {
                switch (entity)
                {
                    case BaseEntity<Guid> guidEntity:
                        guidEntity.DeletedAt = dateTime;
                        if (guidEntity.DeletedBy == Guid.Empty) guidEntity.DeletedBy = userId;
                        break;
                    case BaseEntity<string> stringEntity:
                        stringEntity.DeletedAt = dateTime;
                        if (stringEntity.DeletedBy == Guid.Empty) stringEntity.DeletedBy = userId;
                        break;
                }
                break;
            }

            case EntityState.Detached:
            case EntityState.Unchanged:
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }
}
