using BUPTReportOnline.Models;
using BUPTReportOnline.Utils;
using FluentScheduler;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;

namespace BUPTReportOnline.Jobs
{
    public class SaveJob : IJob
    {
        private static readonly Random random = new Random();
        private readonly BROContext context;
        private readonly string GUID;
        public SaveJob(IServiceScope _serviceScope, string _GUID)
        {
            context = _serviceScope.ServiceProvider.GetService<BROContext>();
            GUID = _GUID;
        }
        public void Execute()
        {
#nullable enable
            User? targetUser = context.User.FirstOrDefault(u => u.GUID == GUID);
#nullable disable
            if (targetUser == null)
            {
                JobManager.RemoveJob(GUID);
            }
            else
            {
                #region 延迟随机时间
                var StartHour = targetUser.StartHour;
                var StartMinute = targetUser.StartMinute;
                var EndHour = targetUser.EndHour;
                var EndMinute = targetUser.EndMinute;
                var StartTotalSecond = (StartHour * 60 + StartMinute) * 60;
                var EndTotalSecond = (EndHour * 60 + EndMinute) * 60;
                var RandomTotalMinute = random.Next(EndTotalSecond - StartTotalSecond + 1);
                Thread.Sleep(RandomTotalMinute * 1000);
                #endregion
                #region 自动填报并保存结果
                var saveResult = WebHelper.Save(targetUser.Cookie, out string Tip);
                targetUser.LastResult = saveResult;
                targetUser.LastMessage = Tip;
                targetUser.LastTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                context.SaveChanges();
                #endregion
                #region 发送邮件通知
                if (targetUser.SendInform)
                {
                    MailHelper.SendEMail(targetUser.GUID, targetUser.Email,
                        targetUser.LastResult, targetUser.LastMessage, targetUser.LastTime);
                }
                #endregion
            }
        }
    }
}
