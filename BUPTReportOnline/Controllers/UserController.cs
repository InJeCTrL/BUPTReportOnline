using AutoMapper;
using BUPTReportOnline.DTOs;
using BUPTReportOnline.IServices;
using BUPTReportOnline.Jobs;
using BUPTReportOnline.Models;
using BUPTReportOnline.Utils;
using FluentScheduler;
using Microsoft.AspNetCore.Mvc;

namespace BUPTReportOnline.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly BROContext context;
        private readonly IMapper mapper;
        private readonly IUserManagement userManagement;

        public UserController(BROContext _context, IMapper _mapper, IUserManagement _userManagement)
        {
            context = _context;
            mapper = _mapper;
            userManagement = _userManagement;
        }

        [HttpGet("{GUID}")]
        public ActionResult<ResultWrapper> GetUserInfo(string GUID)
        {
#nullable enable
            UserDTO? user = userManagement.GetUser(GUID);
#nullable disable
            if (user == null)
            {
                return new ResultWrapper
                {
                    Code = 1,
                    Count = 0,
                    Message = "用户不存在",
                    Data = null
                };
            }
            else
            {
                if (user.IsAdmin)
                {
                    var users = userManagement.GetUsers();
                    return new ResultWrapper
                    {
                        Code = 0,
                        Count = users.Count,
                        Message = "",
                        Data = users
                    };
                }
                else
                {
                    return new ResultWrapper
                    {
                        Code = 0,
                        Count = 1,
                        Message = "",
                        Data = user
                    };
                }
            }
        }

        [HttpGet("AdminChk/{GUID}")]
        public ActionResult<ResultWrapper> GetAdminStatus(string GUID)
        {
#nullable enable
            UserDTO? user = userManagement.GetUser(GUID);
#nullable disable
            if (user == null)
            {
                return new ResultWrapper
                {
                    Code = 1,
                    Count = 0,
                    Message = "用户不存在",
                    Data = null
                };
            }
            else
            {
                return new ResultWrapper
                {
                    Code = 0,
                    Count = 1,
                    Message = "",
                    Data = user.IsAdmin
                };
            }
        }

        [HttpGet("RegChk/{GUID}")]
        public ActionResult<ResultWrapper> GetRegStatus(string GUID)
        {
#nullable enable
            UserDTO? user = userManagement.GetUser(GUID);
#nullable disable
            if (user == null)
            {
                return new ResultWrapper
                {
                    Code = 1,
                    Count = 0,
                    Message = "用户不存在",
                    Data = null
                };
            }
            else
            {
                return new ResultWrapper
                {
                    Code = 0,
                    Count = 1,
                    Message = "",
                    Data = user.Registered
                };
            }
        }

        [HttpPost("{GUID}")]
        public ActionResult<ResultWrapper> AddUser(string GUID, [FromForm] string AddGUID, [FromForm] bool IsAdmin)
        {
#nullable enable
            UserDTO? opr = userManagement.GetUser(GUID);
#nullable disable
            if (opr != null && opr.IsAdmin)
            {
                var addResult = userManagement.AddUser(AddGUID, IsAdmin);
                return new ResultWrapper
                {
                    Code = addResult.Success ? 0 : 1,
                    Count = 0,
                    Data = null,
                    Message = addResult.Message
                };
            }
            else
            {
                return new ResultWrapper
                {
                    Code = 1,
                    Count = 0,
                    Data = null,
                    Message = "非法访问"
                };
            }
        }

        [HttpPost("Reg")]
        public ActionResult<ResultWrapper> RegUser(
            [FromForm] string TargetGUID, [FromForm] int StartHour, [FromForm] int StartMinute,
            [FromForm] int EndHour, [FromForm] int EndMinute, [FromForm] string Email,
            [FromForm] bool SendInform, [FromForm] string UserName,
            [FromForm] string Password, [FromForm] bool UsePWD, [FromForm] string Cookie)
        {
            var regResult = userManagement.RegUser(TargetGUID, StartHour, StartMinute, EndHour, EndMinute,
                Email, SendInform, UserName, Password, UsePWD, Cookie);
            if (regResult.Success)
            {
                JobManager.AddJob(new SaveJob(Startup.serviceScope, TargetGUID), t =>
                {
                    t.WithName(TargetGUID).ToRunEvery(1).Days().At(StartHour, StartMinute);
                });
            }
            return new ResultWrapper
            {
                Code = regResult.Success ? 0 : 1,
                Count = 0,
                Data = null,
                Message = regResult.Message
            };
        }

        [HttpPut("{GUID}")]
        public ActionResult<ResultWrapper> UpdateUser(
            string GUID,
            [FromForm] string TargetGUID, [FromForm] string Email,
            [FromForm] bool SendInform, [FromForm] bool IsAdmin)
        {
#nullable enable
            UserDTO? opr = userManagement.GetUser(GUID);
#nullable disable
            if (opr == null || (!opr.IsAdmin && IsAdmin))
            {
                return new ResultWrapper
                {
                    Code = 1,
                    Count = 0,
                    Data = null,
                    Message = "非法访问"
                };
            }
            else
            {
                var updateResult = userManagement.UpdateUser(TargetGUID, Email, SendInform, IsAdmin);
                return new ResultWrapper
                {
                    Code = updateResult.Success ? 0 : 1,
                    Count = 0,
                    Data = null,
                    Message = updateResult.Message
                };
            }
        }

        [HttpDelete("{GUID}/{TargetGUID}")]
        public ActionResult<ResultWrapper> DeleteUser(string GUID, string TargetGUID)
        {
#nullable enable
            UserDTO? opr= userManagement.GetUser(GUID);
#nullable disable
            if (opr == null || !opr.IsAdmin)
            {
                return new ResultWrapper
                {
                    Code = 1,
                    Count = 0,
                    Data = null,
                    Message = "非法访问"
                };
            }
            else
            {
                var deleteResult = userManagement.RemoveUser(TargetGUID);
                return new ResultWrapper
                {
                    Code = deleteResult.Success ? 0 : 1,
                    Count = 0,
                    Data = null,
                    Message = deleteResult.Message
                };
            }
        }
    }
}
