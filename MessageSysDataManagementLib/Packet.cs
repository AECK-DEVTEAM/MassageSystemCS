using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace MessageSysDataManagementLib
{
    [Serializable]
    public class Packet
    {
        public PacketType type;
        public string uid;
        public string destinationUID;
        public string publicKey;
        public string singleStringData;
        public string messType;
        public byte[] messageData;
        public DateTime messageTimeStamp;
        public List<object> data;

        public Packet(PacketType type)
        {
            this.type = type;
            this.data = new List<object>();
        }

        public Packet(PacketType type, string singleStringData)
        {
            this.type = type;
            this.singleStringData = singleStringData;
        }

        public Packet(PacketType type, List<object> dataList)
        {
            this.type = type;
            this.data = dataList;
        }

        public Packet(PacketType type, string uid, string publicKey)
        {
            this.type = type;
            this.uid = uid;
            this.publicKey = publicKey;
        }      

        public Packet(PacketType type, DateTime messageTimeStamp, string uid, string destinationUID, byte[] messageData)
        {
            this.type = type;
            this.messageTimeStamp = messageTimeStamp;
            this.uid = uid;
            this.destinationUID = destinationUID;
            this.messageData = messageData;
            
        }

        /// <summary>
        /// Deserialisiert den gegebenen ByteArray in ein Packet-Objekt (von JSON zurück zu einem Objekt).
        /// </summary>
        /// <param name="packetType">Packet-Objekt als ByteArray</param>
        public Packet(byte[] packetBytes)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream(packetBytes);

            Packet p = (Packet)bf.Deserialize(ms); //Deserialisiert den ByteArray in ein Objekt (von JSON zu Objekt)

            ms.Close();

            type = p.type;
            uid = p.uid;
            destinationUID = p.destinationUID;
            publicKey = p.publicKey;
            messageData = p.messageData;
            data = p.data;
            singleStringData = p.singleStringData;
            messType = p.messType;
            messageTimeStamp = p.messageTimeStamp;
        }

        /// <summary>
        /// Gibt die eigene IPv4 Adresse zurück. Wenn keine Adresse vergeben ist wird 127.0.0.1 zurückgegeben
        /// </summary>
        /// <returns>IPv4-Adresse als string</returns>
        public static string GetThisIPv4Adress()
        {
            IPAddress[] ips = Dns.GetHostAddresses(Dns.GetHostName());

            foreach (IPAddress ip in ips)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }

            return "127.0.0.1";
        }

        /// <summary>
        /// Serialisiert dieses Object in einen ByteArray (ähnlich wie bei PHP wenn ein Objekt zu einem JSON serialisiert wird).
        /// </summary>
        /// <returns>Serialisierter Byte-Array von diesem Objekt</returns>
        public byte[] ConvertToBytes()
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();

            bf.Serialize(ms, this); //Serialisiert dieses Objekt in einen ByteArray (funktioniert ählich wie JSON)
            byte[] serializedObject = ms.ToArray();
            Console.WriteLine(ms.Length);
            ms.Close();

            return serializedObject;
        }

        // Convert an object to a byte array
        public static byte[] ObjectToByteArray(Object obj)
        {
            if (obj == null)
                return null;

            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);

            return ms.ToArray();
        }

        // Convert a byte array to an Object
        public static Object ByteArrayToObject(byte[] arrBytes)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            Object obj = (Object)binForm.Deserialize(memStream);

            return obj;
        }

        public enum PacketType
        {
            Login,
            LoginSuccess,
            LoginFail,
            Message,
            ClientDisconnected,
            ClientConnected,
            GetClientList,
            ClientList,

            //extend
            Registration,
            RegistrationSuccess,
            RegistrationFail,
            CreateGroup,
            CreateGroupSuccess,
            CreateGroupFail,
            JoinGroup,
            JoinGroupSuccess,
            OutGroup,
            MessageGroup,
            GetGroupList,
            GroupList,
            NewGroupCreated,
            NewClientJoinedGroup,
            ClientOutedGroup,
        }
    }
}
