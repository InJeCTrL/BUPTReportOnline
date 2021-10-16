using Microsoft.Extensions.Configuration;
using System.IO;

namespace BUPTReportOnline
{
    public static class GlobalConfig
    {
        public static string FrontEndPrefix { get; set; }
        public static string SrcEMail { get; set; }
        public static string SrcEMailPWD { get; set; }
        public static string EDMContent { get; set; }
        public static string EMailSubject { get; set; }
        public static void InitConfig(IConfigurationSection configurationSection)
        {
            FrontEndPrefix = configurationSection.GetValue<string>("FrontEndPrefix");
            SrcEMail = configurationSection.GetValue<string>("SrcEMail");
            SrcEMailPWD = configurationSection.GetValue<string>("SrcEMailPWD");
            EMailSubject = configurationSection.GetValue<string>("EMailSubject");
            var EDMPath = configurationSection.GetValue<string>("EDMPath");
            #region 读取EDM模板内容
            EDMContent = File.ReadAllText(EDMPath);
            #endregion
        }
    }
}
