namespace BUPTReportOnline.DTOs
{
    public class UserDTO
    {
        public string GUID { get; set; }
        public int StartHour { get; set; }
        public int StartMinute { get; set; }
        public int EndHour { get; set; }
        public int EndMinute { get; set; }
        public string Email { get; set; }
        public bool SendInform { get; set; }
        public string LastTime { get; set; }
        public bool LastResult { get; set; }
        public string LastMessage { get; set; }
        public bool Registered { get; set; }
        public bool IsAdmin { get; set; }
    }
}
