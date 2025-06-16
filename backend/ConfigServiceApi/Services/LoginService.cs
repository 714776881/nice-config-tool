using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ConfigServiceApi.Models;
using ConfigServiceApi.Repository;
using ConfigServiceApi.Utils;

namespace ConfigServiceApi
{
    public class LoginService
    {
        private readonly UserRepository userRepository;
        public LoginService() { 
            userRepository = new UserRepository();
        }
        public LoginVO GetUserInfo(LoginForm loginModel)
        {
            // 解析口令
            var parameters = DecodeLoginToken(loginModel.loginToken);
            var userid = parameters["userid"];
            var token = parameters["token"];
            var guid = parameters["guid"];
            var regionid = parameters["regionid"];
            var hospitalid = parameters["hospitalid"];
            var departmentid = parameters["departmentid"];

            // 验证口令
            if (false == VerifyToken(userid, guid, token))
            {
                throw new Exception("验证Token失败！");
            }

            // 验证用户
            var users = userRepository.GetUsers(userid);
            if (users.Count == 0)
            {
                throw new Exception("未发现该用户");
            }
            var user = users[0];
            var userInfo = new LoginVO();
            userInfo.userCode = user.UserCode;
            userInfo.userId = user.UserId;
            userInfo.userName = user.UserName;
            userInfo.userActor = user.UserActor;
            userInfo.userRole = user.UserRole;

            userInfo.regionId = regionid;
            userInfo.hospitalId = hospitalid;
            userInfo.departmentId = departmentid;

            Logger.LogError($"[登录] userid = {userid},username = {userInfo.userName}");
            Logger.LogError($"[登录] regionid = {regionid},hospitalid = {hospitalid},departmentid='{departmentid}'");
            return userInfo;
        }

        private bool VerifyToken(string userid, string guid, string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            // 判断口令有效日期
            DateTime dt = DateTime.ParseExact(guid, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);
            var ds = DateTime.Now - dt;
            //
            //if (ds.TotalMinutes > 200)
            //{
            //    return false;
            //}

            var loginKeyCode = ConfigHelper.GetSetting("LoginKeyCode", "24899");
            string expectToken = EncryptionTool.Md5_Encrypt(userid + guid + loginKeyCode);

            if (token != expectToken)
            {
                Logger.LogError("【校验口令】口令错误！");
                return false;
            }
            return true;
        }

        private Dictionary<string, string> DecodeLoginToken(string loginToken)
        {
            var result = new Dictionary<string, string>();
            // base64加密
            try
            {
                loginToken = loginToken.Replace(" ", "+");
                // 检查Base64字符串长度是否是4的倍数，如果不是，添加填充符
                while (loginToken.Length % 4 != 0)
                {
                    loginToken += "=";
                }

                byte[] param = Convert.FromBase64String(loginToken);
                loginToken = System.Text.Encoding.Default.GetString(param, 0, param.Length);
            }
            catch (Exception ex)
            {
                Logger.LogError(string.Format("【免密登录】Base64解码发生异常，错误！" + ex.ToString()));
                throw ex;
            }

            if (loginToken.Equals(""))
            {
                Logger.LogError(string.Format("【免密登录】Base64解码后长度为空，错误！"));
                throw new Exception("Base64解码后长度为空!");
            }

            // 解析后缀 & =
            char[] itemseparator1 = new char[1];
            itemseparator1[0] = '&';
            char[] itemseparator2 = new char[1];
            itemseparator2[0] = '=';

            loginToken = loginToken.ToLower();
            string[] paramsegs = loginToken.Split(itemseparator1, StringSplitOptions.RemoveEmptyEntries);
            for (int index = 0; index < paramsegs.Length; ++index)
            {
                string[] paramsubsegs = paramsegs[index].Split(itemseparator2, StringSplitOptions.RemoveEmptyEntries);
                if (paramsubsegs.Length != 2)
                {
                    continue;
                }
                string key = paramsubsegs[0];
                string value = paramsubsegs[1];
                if (!result.ContainsKey(key))
                {
                    Logger.LogError(string.Format("【解密参数】{0}={1}", key, value));
                    result[key] = value;
                }
            }
            return result;
        }
    }
}
