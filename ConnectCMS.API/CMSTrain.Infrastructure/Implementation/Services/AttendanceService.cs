using ClosedXML.Excel;
using CMSTrain.Helper;
using CMSTrain.Domain.Entities;
using CMSTrain.Application.Exceptions;
using CMSTrain.Application.Common.User;
using CMSTrain.Domain.Entities.Identity;
using CMSTrain.Application.DTOs.Candidate;
using CMSTrain.Application.DTOs.Attendance;
using CMSTrain.Application.DTOs.Certification;
using static CMSTrain.Domain.Common.Constants;
using CMSTrain.Application.Interfaces.Services;
using CMSTrain.Application.Interfaces.Repositories.Base;

namespace CMSTrain.Infrastructure.Implementation.Services;

public class AttendanceService(IGenericRepository genericRepository, 
    ICertificationService certificationService,
    ICurrentUserService userService,
    IFileService fileService) : IAttendanceService
{
    private const string AttendanceImagePath = FilePath.AttendanceImageFilePath;

    public List<GetAttendanceResponseDto> GetAllAttendanceRequests(Guid classId, int pageNumber, int pageSize, out int rowCount, string? search = null, bool? isApproved = null)
    {
        var @class = genericRepository.GetById<Class>(classId)
                     ?? throw new NotFoundException("The respective class could not be found.");

        var training = genericRepository.GetById<Training>(@class.TrainingId)
                       ?? throw new NotFoundException("The respective training could not be found.");

        var trainingCandidates = genericRepository.Get<TrainingCandidate>(x => 
            x.TrainingId == training.Id && x.IsApproved).ToList();

        var candidates = genericRepository.GetPagedResult<User>(pageNumber, pageSize, out rowCount, x => 
            trainingCandidates.Select(z => z.CandidateId).Contains(x.Id) && 
            (string.IsNullOrEmpty(search) || x.Name.ToLower().Contains(search.ToLower()) || x.UserName!.ToLower().Contains(search.ToLower()))).ToList();

        var result = new List<GetAttendanceResponseDto>();
        
        foreach (var candidate in candidates)
        {
            var attendance = genericRepository.GetFirstOrDefault<Attendance>(a => 
                a.CandidateId == candidate.Id && a.ClassId == @class.Id);

            if (isApproved != null && attendance?.IsApproved != isApproved) continue;
            
            var approvalDetails = GetApprovedUserAndRole(attendance);
            
            result.Add(new GetAttendanceResponseDto()
            {
                Id = attendance?.Id,
                AttendedAt = attendance?.CreatedAt.ToFormattedDateTime(),
                ActionDate = attendance?.LastModifiedAt?.ToFormattedDateTime(),
                IsActionCompleted = attendance?.IsActionCompleted,
                IsApproved = attendance?.IsApproved,
                Remarks = attendance?.Remarks,
                ImageUrl = attendance?.AttendanceImageUrl,
                ApprovedBy = approvalDetails.ApprovedUser?.Name,
                ApprovalRole = approvalDetails.ApprovedRole?.Name,
                CandidateDetails = new GetCandidateDetailsDto()
                {
                    Id = candidate.Id,
                    Name = candidate.Name,
                    EmailAddress = candidate.Email ?? "",
                    PhoneNumber = candidate.PhoneNumber ?? "",
                    ImageUrl = candidate.ImageURL,
                    Gender = candidate.Gender.ToString(),
                    DesignationId = candidate.DesignationId,
                    Designation = candidate.DesignationId != null ? genericRepository.GetById<Designation>(candidate.DesignationId)?.Title : null
                }
            });
        }

        return result;
    }
    
    public List<GetAttendanceResponseDto> GetAllAttendanceRequests(Guid classId, string? search = null, bool? isApproved = null)
    {
        var @class = genericRepository.GetById<Class>(classId)
            ?? throw new NotFoundException("The respective class could not be found.");

        var training = genericRepository.GetById<Training>(@class.TrainingId)
                       ?? throw new NotFoundException("The respective training could not be found.");

        var trainingCandidates = genericRepository.Get<TrainingCandidate>(x => 
            x.TrainingId == training.Id && x.IsApproved).ToList();

        var candidates = genericRepository.Get<User>(x => 
                trainingCandidates.Select(z => z.CandidateId).Contains(x.Id) && 
                (string.IsNullOrEmpty(search) || x.Name.ToLower().Contains(search.ToLower()) || x.UserName!.ToLower().Contains(search.ToLower()))).ToList();

        var result = new List<GetAttendanceResponseDto>();
        
        foreach (var candidate in candidates)
        {
            var attendance = genericRepository.GetFirstOrDefault<Attendance>(a => 
                a.CandidateId == candidate.Id && a.ClassId == @class.Id);

            if (isApproved != null && attendance?.IsApproved != isApproved) continue;

            var approvalDetails = GetApprovedUserAndRole(attendance);

            result.Add(new GetAttendanceResponseDto
            {
                Id = attendance?.Id,
                AttendedAt = attendance?.CreatedAt.ToFormattedDateTime(),
                ActionDate = attendance?.LastModifiedAt?.ToFormattedDateTime(),
                IsActionCompleted = attendance?.IsActionCompleted,
                IsApproved = attendance?.IsApproved,
                Remarks = attendance?.Remarks,
                ImageUrl = attendance?.AttendanceImageUrl,
                ApprovedBy = approvalDetails.ApprovedUser?.Name,
                ApprovalRole = approvalDetails.ApprovedRole?.Name,
                CandidateDetails = new GetCandidateDetailsDto()
                {
                    Id = candidate.Id,
                    Name = candidate.Name,
                    EmailAddress = candidate.Email ?? "",
                    PhoneNumber = candidate.PhoneNumber ?? "",
                    ImageUrl = candidate.ImageURL,
                    Gender = candidate.Gender.ToString(),
                    DesignationId = candidate.DesignationId,
                    Designation = candidate.DesignationId != null ? genericRepository.GetById<Designation>(candidate.DesignationId)?.Title : null
                }
            });
        }

        return result;
    }
    
    public GetAttendanceResponseDto? GetAttendanceRequestForCandidate(Guid classId)
    {
        var @class = genericRepository.GetById<Class>(classId)
                     ?? throw new NotFoundException("The respective class could not be found.");

        var candidateId = userService.GetUserId;

        var candidate = genericRepository.GetById<User>(candidateId)
                        ?? throw new NotFoundException("The following user has not been registered to our system.");

        var attendance = genericRepository.GetFirstOrDefault<Attendance>(x =>
            x.ClassId == @class.Id && x.CandidateId == candidate.Id);

        var approvalDetails = GetApprovedUserAndRole(attendance);
        
        return attendance == null
            ? null
            : new GetAttendanceResponseDto()
            {
                Id = attendance.Id,
                AttendedAt = attendance.CreatedAt.ToFormattedDateTime(),
                ActionDate = attendance.LastModifiedAt?.ToFormattedDateTime(),
                IsActionCompleted = attendance.IsActionCompleted,
                IsApproved = attendance.IsApproved,
                Remarks = attendance.Remarks,
                ImageUrl = attendance.AttendanceImageUrl,
                ApprovedBy = approvalDetails.ApprovedUser?.Name,
                ApprovalRole = approvalDetails.ApprovedRole?.Name,
                CandidateDetails = new GetCandidateDetailsDto()
                {
                    Id = candidate.Id,
                    Name = candidate.Name,
                    EmailAddress = candidate.Email ?? "",
                    PhoneNumber = candidate.PhoneNumber ?? "",
                    ImageUrl = candidate.ImageURL,
                    Gender = candidate.Gender.ToString(),
                    DesignationId = candidate.DesignationId,
                    Designation = candidate.DesignationId != null ? genericRepository.GetById<Designation>(candidate.DesignationId)?.Title : null
                }
            };
    }
    
    public List<GetAttendanceResponseDto> GetAttendanceRequestForClient(Guid classId, int pageNumber, int pageSize, out int rowCount, string? search = null, bool? isApproved = null)
    {
        var clientId = userService.GetUserId;

        var client = genericRepository.GetById<User>(clientId) 
                     ?? throw new NotFoundException("The following client has not been registered to our system.");

        var candidates = genericRepository.GetPagedResult<User>(pageNumber, pageSize, out rowCount, x => x.OrganizationId == client.OrganizationId && 
            (string.IsNullOrEmpty(search) || x.Name.ToLower().Contains(search.ToLower()) || x.UserName!.ToLower().Contains(search.ToLower()))).ToList();

        var @class = genericRepository.GetById<Class>(classId)
                     ?? throw new NotFoundException("The respective class could not be found.");

        var training = genericRepository.GetById<Training>(@class.TrainingId)
                       ?? throw new NotFoundException("The following training could not be found.");
        
        var result = new List<GetAttendanceResponseDto>();
        
        foreach (var candidate in candidates)
        {
            var trainingCandidate = genericRepository.GetFirstOrDefault<TrainingCandidate>(x =>
                x.TrainingId == training.Id && x.CandidateId == candidate.Id && x.IsApproved);
            
            if (trainingCandidate is null) continue;

            var attendance = genericRepository.GetFirstOrDefault<Attendance>(x =>
                    x.CandidateId == candidate.Id && x.ClassId == @class.Id);

            if (isApproved != null && attendance?.IsApproved != isApproved) continue;

            var approvalDetails = GetApprovedUserAndRole(attendance);
            
            result.Add(new GetAttendanceResponseDto()
            {
                Id = attendance?.Id,
                AttendedAt = attendance?.CreatedAt.ToFormattedDateTime(),
                ActionDate = attendance?.LastModifiedAt?.ToFormattedDateTime(),
                IsActionCompleted = attendance?.IsActionCompleted,
                IsApproved = attendance?.IsApproved,
                Remarks = attendance?.Remarks,
                ImageUrl = attendance?.AttendanceImageUrl,
                ApprovedBy = approvalDetails.ApprovedUser?.Name,
                ApprovalRole = approvalDetails.ApprovedRole?.Name,
                CandidateDetails = new GetCandidateDetailsDto()
                {
                    Id = candidate.Id,
                    Name = candidate.Name,
                    EmailAddress = candidate.Email ?? "",
                    PhoneNumber = candidate.PhoneNumber ?? "",
                    ImageUrl = candidate.ImageURL,
                    Gender = candidate.Gender.ToString(),
                    DesignationId = candidate.DesignationId,
                    Designation = candidate.DesignationId != null ? genericRepository.GetById<Designation>(candidate.DesignationId)?.Title : null
                }
            });
        }

        return result;
    }
    
    public List<GetAttendanceResponseDto> GetAttendanceRequestForClient(Guid classId, string? search = null, bool? isApproved = null)
    {
        var clientId = userService.GetUserId;

        var client = genericRepository.GetById<User>(clientId) 
                     ?? throw new NotFoundException("The following client has not been registered to our system.");

        var candidates = genericRepository.Get<User>(x => x.OrganizationId == client.OrganizationId && 
                                                          (string.IsNullOrEmpty(search) || x.Name.ToLower().Contains(search.ToLower()) || x.UserName!.ToLower().Contains(search.ToLower()))).ToList();

        var @class = genericRepository.GetById<Class>(classId)
                     ?? throw new NotFoundException("The respective class could not be found.");

        var training = genericRepository.GetById<Training>(@class.TrainingId)
                       ?? throw new NotFoundException("The following training could not be found.");
        
        var result = new List<GetAttendanceResponseDto>();
        
        foreach (var candidate in candidates)
        {
            var trainingCandidate = genericRepository.GetFirstOrDefault<TrainingCandidate>(x =>
                x.TrainingId == training.Id && x.CandidateId == candidate.Id && x.IsApproved);
            
            if (trainingCandidate is null) continue;

            var attendance = genericRepository.GetFirstOrDefault<Attendance>(x =>
                    x.CandidateId == candidate.Id && x.ClassId == @class.Id);
            
            if (isApproved != null && attendance?.IsApproved != isApproved) continue;

            var approvalDetails = GetApprovedUserAndRole(attendance);
            
            result.Add(new GetAttendanceResponseDto()
            {
                Id = attendance?.Id,
                AttendedAt = attendance?.CreatedAt.ToFormattedDateTime(),
                ActionDate = attendance?.LastModifiedAt?.ToFormattedDateTime(),
                IsActionCompleted = attendance?.IsActionCompleted,
                IsApproved = attendance?.IsApproved,
                Remarks = attendance?.Remarks,
                ImageUrl = attendance?.AttendanceImageUrl,
                ApprovedBy = approvalDetails.ApprovedUser?.Name,
                ApprovalRole = approvalDetails.ApprovedRole?.Name,
                CandidateDetails = new GetCandidateDetailsDto()
                {
                    Id = candidate.Id,
                    Name = candidate.Name,
                    EmailAddress = candidate.Email ?? "",
                    PhoneNumber = candidate.PhoneNumber ?? "",
                    ImageUrl = candidate.ImageURL,
                    Gender = candidate.Gender.ToString(),
                    DesignationId = candidate.DesignationId,
                    Designation = candidate.DesignationId != null ? genericRepository.GetById<Designation>(candidate.DesignationId)?.Title : null
                }
            });
        }

        return result;
    }
    
    public void UploadAttendance(AttendanceRequestDto attendanceRequest)
    {
        var candidateId = userService.GetUserId;
        
        var candidate = genericRepository.GetById<User>(candidateId)
                        ?? throw new NotFoundException("The following user has not been registered to our system.");
        
        var @class = genericRepository.GetById<Class>(attendanceRequest.ClassId)
            ?? throw new NotFoundException("The respective class could not be found.");
        
        var attendance = genericRepository.GetFirstOrDefault<Attendance>(a => 
            a.ClassId == @class.Id && a.CandidateId == candidateId);
        
        if (attendance != null)
        {
            var exception = new[]
            {
                "You have already marked your attendance at the following class."
            };
            
            throw new BadRequestException("You can not mark an attendance for the respective class.", exception);
        }

        var filePath = Path.Combine(AttendanceImagePath, @class.Id.ToString());
        
        var imageUrl = fileService.UploadDocument(attendanceRequest.Attendance, filePath);

        var attendanceModel = new Attendance
        {
            ClassId = attendanceRequest.ClassId,
            CandidateId = candidate.Id,
            AttendanceImageUrl = Path.Combine(imageUrl),
            IsActionCompleted = false,
            IsApproved = false,
            Remarks = string.Empty,
        };

        genericRepository.Insert(attendanceModel);
    }

    public void ApproveRejectAttendance(AttendanceApproveRejectDto approveReject)
    {
        var attendance = genericRepository.GetById<Attendance>(approveReject.RequestId)
            ?? throw new NotFoundException("The respective attendance could not be found.");

        attendance.IsActionCompleted = true;
        attendance.Remarks = approveReject.Remarks;
        attendance.IsApproved = approveReject.IsApproved;
        attendance.LastModifiedBy = userService.GetUserId;

        genericRepository.Update(attendance);

        if (!attendance.IsApproved) return;
        
        var candidate = genericRepository.GetById<User>(attendance.CandidateId)
                        ?? throw new NotFoundException("The following user has not been registered to our system.");

        var @class = genericRepository.GetById<Class>(attendance.ClassId)
                     ?? throw new NotFoundException("The following class could not be found.");

        var training = genericRepository.GetById<Training>(@class.TrainingId)
                       ?? throw new NotFoundException("The following training could not be found.");

        var trainingCandidate = genericRepository.GetFirstOrDefault<TrainingCandidate>(x =>
                                    x.TrainingId == training.Id && x.CandidateId == candidate.Id && x.IsApproved)
                                ?? throw new NotFoundException("The following candidate has not been assigned to the respective training.");
            
        certificationService.IssueTrainingCandidateCertification(new IssueCertificationDto()
        {
            TrainingCandidateId = trainingCandidate.Id
        });
    }

    public void CancelAttendance(Guid classId)
    {
        var candidateId = userService.GetUserId;

        var attendance = genericRepository.GetFirstOrDefault<Attendance>(
                             a => a.ClassId == classId && a.CandidateId == candidateId)
                         ?? throw new NotFoundException("The respective attendance could not be found.");

        genericRepository.Delete(attendance);
    }

    public string DownloadAttendanceFile(Guid attendanceId)
    {
        var attendance = genericRepository.GetById<Attendance>(attendanceId)
            ?? throw new NotFoundException("The respective attendance could not be found.");

        var attendanceImagePath = Path.Combine(AttendanceImagePath, attendance.ClassId.ToString(), attendance.AttendanceImageUrl);

        var filePath = fileService.FileExistPath(attendanceImagePath);

        return !string.IsNullOrEmpty(filePath) ? filePath : string.Empty;
    }

    public byte[] ExportAttendanceDetails(Guid classId, Guid? organizationId)
    {
        #region Module Entity Data Representation
        var @class = genericRepository.GetById<Class>(classId)
                     ?? throw new NotFoundException("The following class could not be found.");
        
        var attendances = genericRepository.Get<Attendance>(x => 
            x.ClassId == @class.Id).ToList();
        #endregion
        
        #region Setup of ClosedXML Workbook
        var workbook = new XLWorkbook();
        #endregion

        #region Data Population
        
        #region Initialization of Worksheet
        var attendanceRow = 1;
        var attendanceSheet = workbook.Worksheets.Add("Questions");

        attendanceSheet.Column(1).Width = 10;
        attendanceSheet.Column(2).Width = 50;
        attendanceSheet.Column(3).Width = 50;
        attendanceSheet.Column(4).Width = 30;
        attendanceSheet.Column(5).Width = 45;
        attendanceSheet.Column(6).Width = 30;
        attendanceSheet.Column(7).Width = 25;
        attendanceSheet.Column(8).Width = 20;
        attendanceSheet.Column(9).Width = 20;
        attendanceSheet.Column(10).Width = 20;
        attendanceSheet.Column(11).Width = 25;
        attendanceSheet.Column(12).Width = 80;
        #endregion
        
        #region Attendance Table Header
        attendanceSheet.Cell(attendanceRow, 1).Value = "#";
        attendanceSheet.Cell(attendanceRow, 2).Value = "Name";
        attendanceSheet.Cell(attendanceRow, 3).Value = "Organization";
        attendanceSheet.Cell(attendanceRow, 4).Value = "Contact Number";
        attendanceSheet.Cell(attendanceRow, 5).Value = "Email Address";
        attendanceSheet.Cell(attendanceRow, 6).Value = "Designation";
        attendanceSheet.Cell(attendanceRow, 7).Value = "Attendance Date";
        attendanceSheet.Cell(attendanceRow, 8).Value = "Status";
        attendanceSheet.Cell(attendanceRow, 9).Value = "Remarks";
        attendanceSheet.Cell(attendanceRow, 10).Value = "Action By";
        attendanceSheet.Cell(attendanceRow, 11).Value = "Action Date";
        attendanceSheet.Cell(attendanceRow, 12).Value = "Signature";

        for (var col = 1; col <= 12; col++)
        {
            var cell = attendanceSheet.Cell(attendanceRow, col);
            
            cell.Style.Font.Bold = true;

            if (col is >= 2 and <= 12)
            {
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }
        }
        
        attendanceRow++;
        #endregion

        #region Attendance Details
        foreach (var attendance in attendances)
        {
            var candidate = genericRepository.GetById<User>(attendance.CandidateId)
                            ?? throw new NotFoundException("The following user has not been registered to our system.");

            if (organizationId != null && candidate.OrganizationId != organizationId) continue;
            
            var organization = candidate.OrganizationId != null ? 
                genericRepository.GetById<Organization>(candidate.OrganizationId) 
                    ?? throw new NotFoundException("The following organization could not be found.")
                : null;

            var designation = candidate.DesignationId != null ? 
                genericRepository.GetById<Designation>(candidate.DesignationId)
                    ?? throw new NotFoundException("The following designation could not be found.")
                : null;

            var approvalDetails = GetApprovedUserAndRole(attendance);

            attendanceSheet.Cell(attendanceRow, 1).Value = attendanceRow - 1;
            attendanceSheet.Cell(attendanceRow, 1).Value = attendanceRow - 1;
            attendanceSheet.Cell(attendanceRow, 2).Value = candidate.Name;
            attendanceSheet.Cell(attendanceRow, 3).Value = organization?.Name;
            attendanceSheet.Cell(attendanceRow, 4).Value = candidate.PhoneNumber;
            attendanceSheet.Cell(attendanceRow, 5).Value = candidate.Email;
            attendanceSheet.Cell(attendanceRow, 6).Value = designation?.Title;
            attendanceSheet.Cell(attendanceRow, 7).Value = attendance.CreatedAt.ToFormattedDateTime();
            
            var attendanceStatus = attendance switch
            {
                { IsActionCompleted: true, IsApproved: true } => "Approved",
                { IsActionCompleted: true, IsApproved: false } => "Rejected",
                _ => "Pending"
            };
            
            attendanceSheet.Cell(attendanceRow, 8).Value = attendanceStatus;
            attendanceSheet.Cell(attendanceRow, 9).Value = attendance.Remarks;
            attendanceSheet.Cell(attendanceRow, 10).Value = approvalDetails.ApprovedUser?.Name;
            attendanceSheet.Cell(attendanceRow, 11).Value = attendance.LastModifiedAt?.ToFormattedDateTime();
            
            if (!string.IsNullOrEmpty(attendance.AttendanceImageUrl))
            {
                try
                {
                    var attendanceImagePath = Path.Combine(AttendanceImagePath, attendance.ClassId.ToString(), attendance.AttendanceImageUrl);

                    var filePath = fileService.FileExistPath(attendanceImagePath);
                    
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        var imageBytes = File.ReadAllBytes(filePath);
    
                        using var stream = new MemoryStream(imageBytes);
    
                        var picture = attendanceSheet.AddPicture(stream)
                            .MoveTo(attendanceSheet.Cell(attendanceRow, 12), 5, 5);
    
                        picture.WithSize(550, 75);
    
                        attendanceSheet.Row(attendanceRow).Height = 65;
                    }
                    else
                    {
                        attendanceSheet.Cell(attendanceRow, 12).Value = attendance.AttendanceImageUrl;
                    }
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    attendanceSheet.Cell(attendanceRow, 12).Value = attendance.AttendanceImageUrl;
                }
            }
            else
            {
                attendanceSheet.Cell(attendanceRow, 12).Value = "No Signature Available";
            }

            for (var col = 1; col <= 12; col++)
            {
                var cell = attendanceSheet.Cell(attendanceRow, col);
                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            attendanceRow++;
        }
        #endregion
        
        #region Finalization of Worksheet
        attendanceSheet.Style.Font.FontName = "Aptos";

        var questionsCellRange = attendanceSheet.RangeUsed();

        if (questionsCellRange != null)
        {
            foreach (var cell in questionsCellRange.Cells())
            {
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                cell.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            }
        }
        #endregion
        
        #endregion

        #region Assignment of Workbook
        using var memoryStream = new MemoryStream();
        workbook.SaveAs(memoryStream);
        memoryStream.Seek(0, SeekOrigin.Begin);
        #endregion
        
        return memoryStream.ToArray();
    }
    
    private (User? ApprovedUser, Role? ApprovedRole) GetApprovedUserAndRole(Attendance? attendance)
    {
        if (attendance is not { LastModifiedBy: not null, IsApproved: true }) return (null, null);
        
        var approvedUser = genericRepository.GetById<User>(attendance.LastModifiedBy)
                           ?? throw new NotFoundException("The following user has not been registered to our system.");

        var userRole = genericRepository.GetFirstOrDefault<UserRoles>(x => x.UserId == approvedUser.Id);

        var approvedRole = userRole != null 
            ? genericRepository.GetById<Role>(userRole.RoleId)
              ?? throw new NotFoundException("The following role could not be found.")
            : null;

        return (approvedUser, approvedRole);
    }
}
