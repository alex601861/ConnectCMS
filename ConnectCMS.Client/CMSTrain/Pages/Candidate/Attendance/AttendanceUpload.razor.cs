using MudBlazor;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Service.Extensions;
using CMSTrain.Client.Models.Responses.Training;
using CMSTrain.Client.Models.Requests.Attendance;
using CMSTrain.Client.Models.Requests.Configuration.Class;
using CMSTrain.Client.Models.Responses.Location;

namespace CMSTrain.Client.Pages.Candidate.Attendance;

public partial class AttendanceUpload : ComponentBase
{
    [Parameter] public Guid ClassId { get; set; }
    
    [Parameter] public Guid TrainingId { get; set; }
    
    [Parameter] public EventCallback OnAttendanceMarkup { get; set; }

    private bool IsLoading { get; set; } = true;
    
    protected override async Task OnInitializedAsync()
    {
        await GetTrainingDetails();
        
        await GetAttendanceAvailability();
        
        IsLoading = false;
    }
    
    #region Training Details
    private GetTrainingDto TrainingDetails { get; set; } = new();

    private async Task GetTrainingDetails()
    {
        var result = await TrainingService.GetTrainingById(TrainingId);
        
        if (result?.Result is null)
        {
            SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
            return;
        }
        
        TrainingDetails = result.Result;
    }
    #endregion
    
    #region Upload Attendance
    private byte[] Signature { get; set; } = [];
    
    private AttendanceRequestDto AttendanceRequest { get; set; } = new();

    private async Task OnAttendanceUpload()
    {
        try
        {
            AttendanceRequest = new AttendanceRequestDto()
            {
                ClassId = ClassId,
                Attendance = System.Text.Encoding.UTF8.GetString(Signature)
            };
            
            var result = await AttendanceService.UploadAttendance(AttendanceRequest);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    await OnAttendanceMarkup.InvokeAsync();
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
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion
    
    #region Attendance Availability
    private bool IsAttendanceAvailable { get; set; }
    
    private AbstractClassAttendanceConfigurationDto ClassAttendanceConfiguration { get; set; } = new();
    
    private async Task GetAttendanceAvailability()
    {
        try
        {
            var result = await ConfigurationService.GetClassAttendanceConfigurationByKey(ClassId, ClassConfiguration.ATTENDANCE_PERIOD.ToString());

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }
            
            ClassAttendanceConfiguration = result.Result.Accessibility;

            if (DateTime.Now.Date == ClassAttendanceConfiguration.Date && DateTime.Now.TimeOfDay >= ClassAttendanceConfiguration.AccessPeriod && DateTime.Now.TimeOfDay <= ClassAttendanceConfiguration.ExpirePeriod)
            {
                IsAttendanceAvailable = true;
            }
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion

    #region Geolocation & Fencing
    private bool IsPositionSetup { get; set; }
    
    private string MapUrl { get; set; } = string.Empty;

    private bool IsLocationValidForAttendance { get; set; }
    
    private double LocationDistanceValidity { get; set; }

    private string LocationDistanceValidityMessage { get; set; } = string.Empty;

    private async Task GetGeolocationDetails()
    {
        IsLoading = true;

        IsPositionSetup = false;

        if (ClassAttendanceConfiguration.IsLocationEnabled)
        {
            var locationStatus = await JsRuntime.InvokeAsync<string>("blazorGetLocationPermission.requestLocationStatus", []).ConfigureAwait(true);

            var isLocationPromptEnabled = locationStatus switch
            {
                "granted" => true,
                _ => false
            };

            if (!isLocationPromptEnabled)
            {
                isLocationPromptEnabled = await JsRuntime.InvokeAsync<bool>("triggerLocationPrompt"); 
            }
            
            if (isLocationPromptEnabled)
            {
                var locationDetails = await JsRuntime.InvokeAsync<LocationDetailsDto>("blazorGetLocationInformation.requestCurrentLocation").ConfigureAwait(true);

                var latitude = locationDetails.Latitude;
                var longitude = locationDetails.Longitude;
                
                var trainingLatitude = TrainingDetails.Latitude;
                var trainingLongitude = TrainingDetails.Longitude;
                
                var distance = ExtensionMethods.IsWithinActualRadius(latitude, longitude, (double) trainingLatitude, (double) trainingLongitude, ClassAttendanceConfiguration.Radius ?? 1);
                
                LocationDistanceValidity = distance.Distance;
                IsLocationValidForAttendance = distance.IsWithinRadius;
            
                if (!IsLocationValidForAttendance)
                {
                    LocationDistanceValidityMessage = $"You are {LocationDistanceValidity} km away from the training location. Please move closer, approx. {ClassAttendanceConfiguration.Radius} km to the training location to mark your attendance.";
                }
            }
            else
            {
                LocationDistanceValidityMessage = "Oops! It looks like location access is disabled in your browser. Please enable it to help us calculate attendance locations accurately.";
            }
            
        }
        else
        {
            IsLocationValidForAttendance = true;
        }

        if (IsLocationValidForAttendance)
        {
            SnackbarService.ShowSnackbar("You are within the attendance location. You can now mark your attendance.", Severity.Success, Variant.Outlined);    
        }
        
        IsPositionSetup = true;
            
        IsLoading = false;
        
        StateHasChanged();
    }
    #endregion
}