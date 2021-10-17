using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Web;

namespace BUPTReportOnline.Utils
{
    public static class WebHelper
    {
        private static readonly HttpClient loginClient = new HttpClient(new HttpClientHandler
        {
            UseCookies = false
        });
        private static readonly HttpClient innerClient = new HttpClient(new HttpClientHandler
        {
            UseCookies = false
        });
        private static readonly HttpClient saveClient = new HttpClient(new HttpClientHandler
        {
            UseCookies = false
        });
        private const string loginURL = "https://app.bupt.edu.cn/uc/wap/login/check";
        private const string innerURL = "https://app.bupt.edu.cn/ncov/wap/default/index";
        private const string saveURL = "https://app.bupt.edu.cn/ncov/wap/default/save";
        static WebHelper()
        {
            loginClient.Timeout = new TimeSpan(0, 0, 10);
            loginClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:81.0) Gecko/20100101 Firefox/81.0");
            innerClient.Timeout = new TimeSpan(0, 0, 10);
            innerClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:81.0) Gecko/20100101 Firefox/81.0");
            saveClient.Timeout = new TimeSpan(0, 0, 10);
            saveClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:81.0) Gecko/20100101 Firefox/81.0");
        }
#nullable enable
        /// <summary>
        /// 验证用户名密码，并返回Cookie
        /// </summary>
        /// <param name="Username"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public static string? Verify(string Username, string Password)
        {
            var content = new StringContent($"username={Username}&password={Password}", Encoding.UTF8, "application/x-www-form-urlencoded");
            try
            {
                var loginResponse = loginClient.PostAsync(loginURL, content).Result;
                if (loginResponse.IsSuccessStatusCode &&
                    loginResponse.Content.ReadAsStringAsync().Result.Contains("\"e\":0"))
                {
                    var cookies = loginResponse.Headers.GetValues("Set-Cookie");
                    var cookiesBuilder = new StringBuilder();
                    foreach (var cookie in cookies)
                    {
                        cookiesBuilder.Append($"{cookie};");
                    }
                    return cookiesBuilder.ToString();
                }
            }
            catch
            {
                return null;
            }
            return null;
        }
#nullable disable
        /// <summary>
        /// 验证Cookie
        /// </summary>
        /// <param name="Cookie"></param>
        /// <returns></returns>
        public static bool Verify(string Cookie)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, innerURL);
                request.Headers.Add("Cookie", Cookie);
                var innerResponse = innerClient.SendAsync(request).Result;
                if (innerResponse.IsSuccessStatusCode &&
                    innerResponse.Content.ReadAsStringAsync().Result.Contains("<title>登录</title>"))
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 执行填报
        /// </summary>
        /// <param name="Cookie"></param>
        /// <param name="Tip"></param>
        /// <returns></returns>
        public static bool Save(string Cookie, out string Tip)
        {
            try
            {
                var innerRequest = new HttpRequestMessage(HttpMethod.Get, innerURL);
                innerRequest.Headers.Add("Cookie", Cookie);
                var innerResponse = innerClient.SendAsync(innerRequest).Result;
                if (innerResponse.IsSuccessStatusCode &&
                    innerResponse.Content.ReadAsStringAsync().Result.Contains("<title>登录</title>"))
                {
                    Tip = "登录失败";
                    return false;
                }
                else
                {
                    var content = innerResponse.Content.ReadAsStringAsync().Result;
                    if (content.Contains("hasFlag: '1'"))
                    {
                        Tip = "今天已经填报了";
                        return true;
                    }
                    else
                    {
                        var startPattern = "oldInfo: ";
                        var oldInfo = content[(content.IndexOf(startPattern) + startPattern.Length)..];
                        oldInfo = oldInfo[..oldInfo.IndexOf("tipMsg: ''")].Trim();
                        oldInfo = oldInfo[..^1];
                        var json = JsonSerializer.Deserialize<Dictionary<string, object>>(oldInfo);
                        var sendInfo = "";
                        foreach (var part in json)
                        {
                            sendInfo += $"{part.Key}={HttpUtility.UrlEncode(part.Value.ToString())}&";
                        }
                        sendInfo = sendInfo[..^1];
                        var sendInfoContent = new StringContent(sendInfo, Encoding.UTF8, "application/x-www-form-urlencoded");
                        var saveRequest = new HttpRequestMessage(HttpMethod.Post, saveURL)
                        {
                            Content = sendInfoContent
                        };
                        saveRequest.Headers.Add("Cookie", Cookie);
                        var saveResponse = saveClient.SendAsync(saveRequest).Result;
                        if (!saveResponse.IsSuccessStatusCode)
                        {
                            Tip = "请求失败";
                            return false;
                        }
                        else
                        {
                            var saveResult = saveResponse.Content.ReadAsStringAsync().Result;
                            var saveData = JsonSerializer.Deserialize<Dictionary<string, object>>(saveResult);
                            Tip = saveData["m"].ToString();
                            return saveData["e"].ToString() == "0";
                        }
                    }
                }
            }
            catch
            {
                Tip = "网络错误";
                return false;
            }
        }
    }
}
