using AutoMapper;
using BUPTReportOnline.DTOs;
using BUPTReportOnline.IServices;
using BUPTReportOnline.Models;
using BUPTReportOnline.Utils;
using System.Collections.Generic;
using System.Linq;

namespace BUPTReportOnline.Services
{
    public class UserManagement : IUserManagement
    {
        private readonly BROContext context;
        private readonly IMapper mapper;
        public UserManagement(BROContext _context, IMapper _mapper)
        {
            context = _context;
            mapper = _mapper;
        }
        public EventResult AddUser(string GUID, bool IsAdmin)
        {
            if (GUID == null || GUID.Length == 0)
            {
                return new EventResult
                {
                    Success = false,
                    Message = "非法GUID"
                };
            }
            if (GetUser(GUID) == null)
            {
                context.User.Add(new User
                {
                    IsAdmin = IsAdmin,
                    GUID = GUID
                });
                context.SaveChanges();
                return new EventResult
                {
                    Success = true,
                    Message = "添加成功"
                };
            }
            else
            {
                return new EventResult
                {
                    Success = false,
                    Message = "已有重复用户"
                };
            }
        }

        public EventResult RegUser(string GUID, int StartHour, int StartMinute, int EndHour, int EndMinute,
            string Email, bool SendInform, string UserName, string Password, bool UsePWD, string Cookie)
        {
            if (StartHour < 0 || StartHour >= 24 || EndHour < 0 || EndHour >= 24 ||
                StartMinute < 0 || StartMinute >= 60 || EndMinute < 0 || EndMinute >= 60)
            {
                return new EventResult
                {
                    Success = false,
                    Message = "非法的时间格式"
                };
            }
            if ((StartHour == EndHour && StartMinute > EndMinute) ||
                (StartHour > EndHour))
            {
                return new EventResult
                {
                    Success = false,
                    Message = "时间范围右边界不可早于左边界"
                };
            }
            var target = context.User.FirstOrDefault(u => u.GUID == GUID);
            if (target == null)
            {
                return new EventResult
                {
                    Success = false,
                    Message = "用户不存在"
                };
            }
            else if (target.Registered)
            {
                return new EventResult
                {
                    Success = false,
                    Message = "用户已注册"
                };
            }
            if (UsePWD)
            {
#nullable enable
                string? cookie = WebHelper.Verify(UserName, Password);
#nullable disable
                if (cookie != null)
                {
                    target.StartHour = StartHour;
                    target.StartMinute = StartMinute;
                    target.EndHour = EndHour;
                    target.EndMinute = EndMinute;
                    target.Email = Email;
                    target.SendInform = SendInform;
                    target.Registered = true;
                    target.Cookie = cookie;
                    context.User.Update(target);
                    context.SaveChanges();
                    return new EventResult
                    {
                        Success = true,
                        Message = "注册成功"
                    };
                }
                else
                {
                    return new EventResult
                    {
                        Success = false,
                        Message = "身份验证失败"
                    };
                }
            }
            else
            {
#nullable enable
                var valid = WebHelper.Verify(Cookie);
#nullable disable
                if (valid)
                {
                    target.StartHour = StartHour;
                    target.StartMinute = StartMinute;
                    target.EndHour = EndHour;
                    target.EndMinute = EndMinute;
                    target.Email = Email;
                    target.SendInform = SendInform;
                    target.Registered = true;
                    target.Cookie = Cookie;
                    context.User.Update(target);
                    context.SaveChanges();
                    return new EventResult
                    {
                        Success = true,
                        Message = "注册成功"
                    };
                }
                else
                {
                    return new EventResult
                    {
                        Success = false,
                        Message = "身份验证失败"
                    };
                }
            }
        }

        public EventResult UpdateUser(string GUID, string Email, bool SendInform, bool IsAdmin)
        {
            var target = context.User.FirstOrDefault(u => u.GUID == GUID);
            if (target == null)
            {
                return new EventResult
                {
                    Success = false,
                    Message = "用户不存在"
                };
            }
            target.Email = Email;
            target.SendInform = SendInform;
            target.IsAdmin = IsAdmin;
            context.User.Update(target);
            context.SaveChanges();
            return new EventResult
            {
                Success = true,
                Message = "修改完成"
            };
        }

#nullable enable
        public UserDTO? GetUser(string GUID)
        {
            return mapper.Map<UserDTO?>(context.User.FirstOrDefault(u => u.GUID == GUID));
        }
#nullable disable

        public List<UserDTO> GetUsers()
        {
            return mapper.Map<List<UserDTO>>(context.User.ToList());
        }

        public EventResult RemoveUser(string GUID)
        {
            var user = context.User.FirstOrDefault(u => u.GUID == GUID);
            if (user != null)
            {
                context.User.Remove(user);
                context.SaveChanges();
                return new EventResult
                {
                    Success = true,
                    Message = "删除成功"
                };
            }
            else
            {
                return new EventResult
                {
                    Success = false,
                    Message = "没有这个用户"
                };
            }
        }
    }
}
