namespace AppointmentManagementSystem.Helpers
{
    /// <summary>
    /// Standard envelope for JSON API responses consumed by FullCalendar and other AJAX clients.
    /// </summary>
    /// <typeparam name="T">Payload type (appointment list, status code, etc.).</typeparam>
    public class CommonResponse<T>
    {
        /// <summary>1 = success, 0 = failure. See <see cref="ConstHelper.SucceededCode"/>.</summary>
        public int Status { get; set; }

        public T Data { get; set; }

        /// <summary>Human-readable message for UI notifications.</summary>
        public string Message { get; set; }
    }
}
