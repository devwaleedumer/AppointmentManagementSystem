using Microsoft.AspNetCore.Mvc.Rendering;

namespace AppointmentManagementSystem.Helpers
{
    /// <summary>
    /// Shared constants for roles, API status codes, user-facing messages, and UI dropdown helpers.
    /// </summary>
    public static class ConstHelper
    {
        // Identity role names (must match AspNetRoles.Name values).
        public const string AdminRole = "Admin";
        public const string DoctorRole = "Doctor";
        public const string PatientRole = "Patient";

        /// <summary>Comma-separated roles for [Authorize] when any app role may access.</summary>
        public const string AllRoles = "Admin,Doctor,Patient";

        public const string AdminAndDoctor = "Admin,Doctor";
        public const string AdminAndPatient = "Admin,Patient";
        public const string DoctorAndPatient = "Doctor,Patient";

        /// <summary>Appointment approval: pending doctor review.</summary>
        public const int AppointmentPending = 0;

        /// <summary>Appointment approval: accepted by doctor.</summary>
        public const int AppointmentApproved = 1;

        /// <summary>Appointment approval: cancelled (soft status, row is kept).</summary>
        public const int AppointmentCancelled = -1;

        // API / service operation messages
        public static string AppointmentAdded = "Appointment Added Successfully";
        public static string AppointmentUpdated = "Appointment Updated Successfully";
        public static string AppointmentDeleted = "Appointment Deleted Successfully";
        public static string AppointmentExisted = "Appointment Already Exists";

        public static string AppointmentNotFound = "Appointment Not Found";
        public static string AppointmentAddError = "Error occurred while adding appointment";
        public static string AppointmentUpdateError = "Error occurred while updating appointment";
        public static string AppointmentDeleteError = "Error occurred while deleting appointment";

        public static string SomeThingWentWrong = "Something went wrong";

        /// <summary>Standard success flag for JSON API responses.</summary>
        public static int SucceededCode = 1;

        /// <summary>Standard failure flag for JSON API responses.</summary>
        public static int FailedCode = 0;

        public static IEnumerable<SelectListItem> GetDropdownRoles()
        {
            return
            [
                new SelectListItem { Text = AdminRole, Value = AdminRole },
                new SelectListItem { Text = DoctorRole, Value = DoctorRole },
                new SelectListItem { Text = PatientRole, Value = PatientRole }
            ];
        }

        /// <summary>
        /// Builds duration options for booking forms. Values are total minutes; labels show hours.
        /// </summary>
        public static List<SelectListItem> GetTimeDropDown()
        {
            int minute = 60;
            List<SelectListItem> timeList = new List<SelectListItem>();
            for (int i = 1; i <= 12; i++)
            {
                timeList.Add(new(value: minute.ToString(), text: i + "Hr"));
                minute += 30;
                timeList.Add(new(value: minute.ToString(), text: i + "Hr 30 min"));
                minute += 30;
            }

            return timeList;
        }
    }
}
