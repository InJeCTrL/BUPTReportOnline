using BUPTReportOnline.DTOs;
using BUPTReportOnline.Models;
using System.Collections.Generic;

namespace BUPTReportOnline.IServices
{
    public interface IUserManagement
    {
        public EventResult AddUser(string GUID, bool IsAdmin);
        public EventResult RegUser(string GUID, int StartHour, int StartMinute, int EndHour, int EndMinute,
            string Email, bool SendInform, string UserName, string Password, bool UsePWD, string Cookie);
        public EventResult UpdateUser(string GUID, string Email, bool SendInform, bool IsAdmin);
#nullable enable
        public UserDTO? GetUser(string GUID);
#nullable disable
        public List<UserDTO> GetUsers();
        public EventResult RemoveUser(string GUID);
    }
}
