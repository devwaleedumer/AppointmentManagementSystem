
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AppointmentManagementSystem.Helpers
{
    public static class ConstHelper
    {
        public static  string AdminRole = "Admin";
        public static string DoctorRole = "Doctor";
        public static  string PatientRole = "Patient";

        // success messages
        public static string AppointmentAdded = "Appointment Added Successfully";
        public static string AppointmentUpdated = "Appointment Updated Successfully";
        public static string AppointmentDeleted = "Appointment Deleted Successfully";
        public static string AppointmentExisted = "Appointment Already Exists";

        // error messages
        public static string AppointmentNotFound = "Appointment Not Found";
        public static string AppointmentAddError = "Error occurred while adding appointment";
        public static string AppointmentUpdateError = "Error occurred while updating appointment";
        public static string AppointmentDeleteError = "Error occurred while deleting appointment";

        public static string SomeThingWentWrong = "Something went wrong";

        public static int SucceededCode = 1;
        public static int FailedCode = 0;


        public static IEnumerable<SelectListItem> GetDropdownRoles()
        {
            return new[] {
               new SelectListItem { Text = AdminRole, Value = AdminRole },
               new SelectListItem { Text = DoctorRole, Value = DoctorRole },
               new SelectListItem { Text = PatientRole, Value = PatientRole }
            };
        }

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
