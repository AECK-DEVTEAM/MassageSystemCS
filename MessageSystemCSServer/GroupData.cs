using MessageSysDataManagementLib;
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
    class GroupData
    {
        private string _gid;
        public string GID { get => _gid; set => _gid = value; }

        [JsonIgnore]
        public List<ClientData> clientsJoined = new List<ClientData>();

        private static string SAVE_PATH = "\\groups_data.json";

        public GroupData(string gid)
        {
            this._gid = gid;
        }

        public void Save()
        {
            //todo: Save group to file
            var list = LoadListGroup();
            list.Add(this);
            try
            {
                var userRaw = JsonConvert.SerializeObject(list);
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                path = path + SAVE_PATH;
                File.WriteAllText(path, userRaw);
            }
            catch
            {
                Console.WriteLine("Save group data fail");
            }
        }

        public static List<GroupData> LoadListGroup()
        {
            var list = new List<GroupData>();
            try
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                path = path + SAVE_PATH;
                string usersRaw = File.ReadAllText(path);
                list = JsonConvert.DeserializeObject<List<GroupData>>(usersRaw);
            }
            catch
            {
                Console.WriteLine("Load group data fail");
            }
            return list;
        }

        public void SendDataPacketToGroup(Packet packet)
        {
            //foreach(var client in this.clientsJoined)
            //{
            //    if (client.is)
            //}
        }
    }
}
