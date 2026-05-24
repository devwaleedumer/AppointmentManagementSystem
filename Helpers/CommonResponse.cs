namespace AppointmentManagementSystem.Helpers
{
    public class CommonResponse<T>
    {
        public int Status { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }
    }
}
