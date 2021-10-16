using System.ComponentModel.DataAnnotations.Schema;

namespace BUPTReportOnline.Models
{
    public class User
    {
        public int Id { get; set; }
        /// <summary>
        /// 用户唯一ID
        /// </summary>
        public string GUID { get; set; }
        /// <summary>
        /// 用户Cookie
        /// </summary>
        public string Cookie { get; set; } = "";
        /// <summary>
        /// 小时时间范围左边界
        /// </summary>
        public int StartHour { get; set; }
        /// <summary>
        /// 分钟时间范围左边界
        /// </summary>
        public int StartMinute { get; set; }
        /// <summary>
        /// 小时时间范围右边界
        /// </summary>
        public int EndHour { get; set; }
        /// <summary>
        /// 分钟时间范围右边界
        /// </summary>
        public int EndMinute { get; set; }
        /// <summary>
        /// 邮箱号
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 是否发送通知邮件
        /// </summary>
        public bool SendInform { get; set; }
        /// <summary>
        /// 最近一次填报时间
        /// </summary>
        public string LastTime { get; set; }
        /// <summary>
        /// 最近一次执行结果
        /// </summary>
        public bool LastResult { get; set; } = false;
        /// <summary>
        /// 最近一次执行结果消息
        /// </summary>
        public string LastMessage { get; set; }
        /// <summary>
        /// 是否已注册
        /// </summary>
        public bool Registered { get; set; } = false;
        /// <summary>
        /// 是否是管理员
        /// </summary>
        public bool IsAdmin { get; set; }
    }
}
