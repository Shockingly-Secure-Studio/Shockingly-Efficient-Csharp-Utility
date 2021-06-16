using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Org.BouncyCastle.Crypto.Tls;
using Service.Exploit;
using UnityEngine;

namespace Service
{
    public class password
    {
        private string _pathWordList;
        private List<InputWebService> _total;
        private string[] _loginListParam={"login","username","email","user"};
        private string[] _passwordListParam={"password"};

        public password(string pathWordList,List<InputWebService> total)
        {
            _pathWordList = pathWordList;
            _total = total;
        }
        public async Task _test()
        {
            for(var i=0; i<_total.Count-1;i++)
            {
                if (_loginListParam.Contains(_total[i].GetParam()))
                {
                    if (_passwordListParam.Contains(_total[i+1].GetParam()))
                    {
                        await TryPassword(_total[i], _total[i + 1]);
                    }
                }
            }
        }
        private  async Task TryPassword(InputWebService login,InputWebService password)
        {
            if (File.Exists(_pathWordList))
            {
                using var file = new System.IO.StreamReader(_pathWordList);
                string payload;
                string badResult= await login.Submit("notAnUserName", new List<(string, string)>()
                        {(password.GetParam(),"notAPassWord")});
                int badResultHash = badResult.GetHashCode();
                while((payload = await file.ReadLineAsync()) != null)
                {
                    string[] credential = payload.Split(',');
                    string result= await login.Submit(credential[0], new List<(string, string)>()
                            {(password.GetParam(),credential[1])});
                    int resultHash = result.GetHashCode();
                    if (resultHash!=badResultHash)
                    {
                        AccessPoint accessPoint = new AccessPoint(login.GetUrl(), payload, AccessPointType.Insecure_Authentication,8 );
                        login.Log(accessPoint);
                        Debug.Log($"Insecure_Authentication flaw find with {payload}");
                        return;
                    }
                }
            }

        }
    }
}