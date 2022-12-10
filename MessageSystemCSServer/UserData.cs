using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MessageSystemCSServer
{
    class UserData
    {
        private string _uid;
        private string _password;

        private static string SAVE_PATH = "users_data.json";

        public UserData(string uid, string password)
        {
            _uid = uid;
            _password = password;
        }

        public void Save()
        {
            //todo: Save list user to file
            var listUser = LoadListUsers();
            listUser.Add(this);
            try
            {
                var userRaw = JsonConvert.SerializeObject(listUser);
                File.WriteAllText(SAVE_PATH, userRaw);
            }
            catch
            {
                Console.WriteLine("Save users data fail");
            }
        }

        public static List<UserData> LoadListUsers()
        {
            var list = new List<UserData>();
            try
            {
                string usersRaw = File.ReadAllText(SAVE_PATH);
                list = JsonConvert.DeserializeObject<List<UserData>>(usersRaw);
            }
            catch
            {
                Console.WriteLine("Load users data fail");
            }
            return list;
        }
    }


}
