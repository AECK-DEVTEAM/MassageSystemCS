using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using MessageSysDataManagementLib;
using System.Threading;
using System.IO;

namespace MessageSystemCSServer
{
    class Server
    {
        const int PORT = 6666;
        private static TcpListener listener;
        private static List<ClientData> clients = new List<ClientData>();
        private static List<UserData> users = new List<UserData>(); 
        private static List<GroupData> groups = new List<GroupData>();

        static void Main(string[] args)
        {
            clients = new List<ClientData>();
            users = UserData.LoadListUsers();
            groups = GroupData.LoadListGroup();

            Console.Title = "MessageSystemCS | Server";

            StartServer();
        }

        private static void StartServer()
        {
            Console.WriteLine("Starting server on " + Packet.GetThisIPv4Adress());

            listener = new TcpListener(new IPEndPoint(IPAddress.Parse(Packet.GetThisIPv4Adress()), PORT));

            Console.WriteLine("Server started. Waiting for new Client connections...\n");

            Thread listenForNewClients = new Thread(ListenForNewClients);
            listenForNewClients.Start();
        }

        private static void ListenForNewClients()
        {
            listener.Start();

            while (true)
            {
                //Sobald ein Client sich verbinden will ClientData erstellen und in ClientListe schieben
                clients.Add(new ClientData(listener.AcceptTcpClient()));
                Console.WriteLine("New Client connected");
            }            
        }

        public static void DataIn(object tcpClient)          //clientData)
        {
            TcpClient client = (TcpClient)tcpClient;
            NetworkStream clientStream = client.GetStream();
            try
            {
                while (true)
                {
                    byte[] buffer; //Daten
                    byte[] dataSize = new byte[4]; //Länge

                    int readBytes = clientStream.Read(dataSize, 0, 4);

                    while (readBytes != 4)
                    {
                        readBytes += clientStream.Read(dataSize, readBytes, 4 - readBytes);
                    }
                    var contentLength = BitConverter.ToInt32(dataSize, 0);

                    buffer = new byte[contentLength];
                    readBytes = 0;
                    while (readBytes != buffer.Length)
                    {
                        readBytes += clientStream.Read(buffer, readBytes, buffer.Length - readBytes);
                    }

                    //Daten sind im Buffer-Array
                    DataManagerForIncommingClientData(new Packet(buffer), client);
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine("No: " + ex.ErrorCode + " Message: " + ex.Message);
            }
            catch (IOException)
            {
                ClientData disconnectedClient = GetClientFromList(client);
                Console.WriteLine("Client disconnected with UID: " + GetClientFromList(client).UID);
                clients.Remove(disconnectedClient);
                foreach (var g in groups)
                {
                    bool hasClient = g.clientsJoined.Remove(disconnectedClient);
                    if (hasClient)
                    {
                        foreach (var c in g.clientsJoined)
                        {
                            c.SendDataPacketToClient(new Packet(Packet.PacketType.ClientOutedGroup, g.GID + ";" + disconnectedClient.UID + ";" + disconnectedClient.PublicKey));
                        }
                        Console.WriteLine("Group members are notified that " + disconnectedClient.UID + " has out group");
                    }
                }
                Console.WriteLine("Client removed from list.\n");

                //Notify other Clients that client has disconnected.
                foreach (ClientData c in clients)
                {
                    c.SendDataPacketToClient(new Packet(Packet.PacketType.ClientDisconnected, disconnectedClient.UID + ";" + disconnectedClient.PublicKey));
                    Console.WriteLine(c.UID + " notified that " + disconnectedClient.UID + " has disconnected");
                }
                Console.ReadLine();
                Environment.Exit(0);
            }
        }

        private static void DataManagerForIncommingClientData(Packet p, TcpClient clientSocket)
        {
            ClientData client;
            GroupData group;
            switch (p.type)
            {
                case Packet.PacketType.OutGroup:
                    client = GetClientFromList(clientSocket);
                    Console.WriteLine("Client " + client.UID + " out group " + p.singleStringData + ".");
                    group = groups.Find(g => g.GID.ToLower() == p.singleStringData.ToLower());
                    if (group != null)
                    {
                        if (group.clientsJoined.Remove(client))
                        {
                            client.SendDataPacketToClient(new Packet(Packet.PacketType.ClientOutedGroup, group.GID + ";" + client.UID + ";" + client.PublicKey));
                            Console.WriteLine("Group members [" + group.ToString() + "]");

                            //Notify group members that client out Group
                            foreach(var c in group.clientsJoined)
                            {
                                c.SendDataPacketToClient(new Packet(Packet.PacketType.ClientOutedGroup, group.GID + ";" + client.UID + ";" + client.PublicKey));
                            }
                        }
                    }
                     break;
                case Packet.PacketType.JoinGroup:
                    client = GetClientFromList(clientSocket);
                    Console.WriteLine("Client " + client.UID + "join group " + p.singleStringData + ".");
                    group = groups.Find(g => g.GID.ToLower() == p.singleStringData.ToLower());
                    if (group != null)
                    {
                        group.clientsJoined.Add(client);
                        //Notify group members that new client joined Group
                        client.SendDataPacketToClient(new Packet(Packet.PacketType.JoinGroupSuccess));
                        foreach (var c in group.clientsJoined)
                        {
                            if (c.UID.ToLower() != client.UID.ToLower())
                            {
                                c.SendDataPacketToClient(new Packet(Packet.PacketType.NewClientJoinedGroup, group.GID + ';' + client.UID + ';' + client.PublicKey));
                            }
                        }
                        Console.WriteLine("Group members ["+group.ToString()+ "]");
                    }
                    break;
                case Packet.PacketType.GetGroupList:
                    client = GetClientFromList(clientSocket);
                    Console.WriteLine("Client " + client.UID + " wants Group List. Generating...");
                    Server.LoadGroupsDataToServer();
                    List<object> gdataList = new List<object>();
                    foreach (GroupData g in groups)
                    {
                        gdataList.Add(g.GID);
                    }
                    client.SendDataPacketToClient(new Packet(Packet.PacketType.GroupList, gdataList));
                    break;
                case Packet.PacketType.CreateGroup:
                    Console.WriteLine("Client wants to create group with GID: " + p.singleStringData);
                    client = GetClientFromList(clientSocket);
                    Server.LoadGroupsDataToServer();
                    bool gidExist = false;
                    foreach(var g in groups)
                    {
                        if (g.GID.ToLower() == p.singleStringData.ToLower())
                        {
                            client.SendDataPacketToClient(new Packet(Packet.PacketType.CreateGroupFail, "Group with this gid already exists!"));
                            gidExist = true;
                        }
                    }
                    if (!gidExist)
                    {
                        group = new GroupData(p.singleStringData);
                        group.Save();
                        client.SendDataPacketToClient(new Packet(Packet.PacketType.CreateGroupSuccess));

                        //Notify clients that new Group has created
                        foreach (ClientData c in clients)
                        {
                            if (c.UID != p.uid)
                            {
                                c.SendDataPacketToClient(new Packet(Packet.PacketType.NewGroupCreated, group.GID));
                            }
                        }
                    }
                    break;
                case Packet.PacketType.Registration:
                    Console.WriteLine("Client wants to registration with UID: " + p.uid + " and Public-Key: " + p.publicKey);
                    client = GetClientFromList(clientSocket);
                    var user = GetUserFromList(p.uid);
                    if (user != null)
                    {
                        client.SendDataPacketToClient(new Packet(Packet.PacketType.RegistrationFail, "User with this uid already exists!"));
                    }
                    else
                    {
                        user = new UserData(p.uid, p.singleStringData);
                        user.Save();
                        client.SendDataPacketToClient(new Packet(Packet.PacketType.RegistrationSuccess, "User with this uid already exists!"));
                    }
                    break;
                case Packet.PacketType.Login:
                    Console.WriteLine("Client wants to login with UID: " + p.uid + " and Public-Key: " + p.publicKey);
                    client = GetClientFromList(clientSocket);

                    // Check account in db
                    if (!CheckUidAndPass(p.uid, p.singleStringData))
                    {
                        client.SendDataPacketToClient(new Packet(Packet.PacketType.LoginFail, "Uid or password increct!"));
                        break;
                    }

                    foreach (ClientData c in clients)
                    {
                        if(c.UID.ToLower() == p.uid.ToLower())
                        {
                            client.SendDataPacketToClient(new Packet(Packet.PacketType.LoginFail, "User with this uid already online!"));
                        }
                    }

                    client.UID = p.uid;
                    client.PublicKey = p.publicKey;
                    client.SendDataPacketToClient(new Packet(Packet.PacketType.LoginSuccess));

                    //Notify clients that new Client has connected
                    foreach (ClientData c in clients)
                    {
                        if (c.UID != p.uid)
                        {
                            c.SendDataPacketToClient(new Packet(Packet.PacketType.ClientConnected, p.uid + ";" + p.publicKey));
                        }
                    }
                    break;
                case Packet.PacketType.GetClientList:
                    client = GetClientFromList(clientSocket);
                    Console.WriteLine("Client " + client.UID + " wants Client List. Generating...");

                    List<object> dataList = new List<object>();
                    foreach (ClientData c in clients)
                    {
                        if (c.UID != client.UID)
                        {
                            dataList.Add(c.UID + ";" + c.PublicKey);
                        }
                    }
                    client.SendDataPacketToClient(new Packet(Packet.PacketType.ClientList, dataList));
                    break;
                case Packet.PacketType.Message:
                    Console.WriteLine("Incomming Message from " + p.uid + " at " + p.messageTimeStamp.ToString("HH:mm:ss") + " to " + p.destinationUID + " data: " + Encoding.UTF8.GetString(p.messageData));
                    foreach (ClientData c in clients)
                    {
                        if(c.UID == p.destinationUID)
                        {
                            c.SendDataPacketToClient(new Packet(Packet.PacketType.Message, p.messageTimeStamp, p.uid, p.destinationUID, p.messageData)
                            {
                                messType = p.messType
                            });
                            Console.WriteLine("Message send to " + c.UID);
                        }
                    }
                    break;
                case Packet.PacketType.MessageGroup:
                    Console.WriteLine("Incomming Group Message from " + p.uid + " at " + p.messageTimeStamp.ToString("HH:mm:ss") + " to group " + p.destinationUID);
                    client = GetClientFromList(clientSocket);
                    group = groups.Find(g => g.GID.ToLower() == p.destinationUID.ToLower());
                    foreach(var c in group.clientsJoined)
                    {
                        if (client.UID != c.UID)
                        {
                            var p2 = new Packet(Packet.PacketType.MessageGroup, p.messageTimeStamp, p.uid, p.destinationUID, p.messageData);
                            p2.singleStringData = p.singleStringData;
                            p2.messType = p.messType;
                            c.SendDataPacketToClient(p2);
                        }
                    }
                    break;
            }
        }

        private static void LoadGroupsDataToServer()
        {
            var data = GroupData.LoadListGroup();
            foreach (var group in Server.groups)
            {
                var x = data.Find(g => g.GID == group.GID);
                if (x != null)
                {
                    x.clientsJoined = group.clientsJoined;
                }
            }
            Server.groups = data;
        }

        private static UserData GetUserFromList(string uid)
        {
            users = UserData.LoadListUsers();

            for (int i = 0; i < users.Count; i++)
            {
                if (users[i].UID.ToLower().Trim() == uid.ToLower().Trim()) 
                    return users[i];
            }
            return null;
        }

        private static bool CheckUidAndPass(string uid, string password)
        {
            var user = Server.GetUserFromList(uid);
            if (user != null && user.Password == password)
                return true;
            return false;
        }

        /// <summary>
        /// Findet den passenden Client welcher über diesen Socket mit dem Server verbunden ist.
        /// </summary>
        /// <param name="clientSocket">Socket mit dem der Client mit dem Server verbunden ist</param>
        /// <returns>Gefundenen Client andernfalls null.</returns>
        private static ClientData GetClientFromList(TcpClient tcpClient)
        {
            foreach (ClientData client in clients)
            {
                if (client.TcpClient == tcpClient)
                {
                    return client;
                }
            }

            return null;
        }

        /// <summary>
        /// Findet den passenden Client welcher mit dieser UID mit dem Server verbunden ist.
        /// </summary>
        /// <param name="uid">Client UID</param>
        /// <returns>Gefundenen Client andernfalls null.</returns>
        private static ClientData GetClientFromList(string uid)
        {
            foreach (ClientData client in clients)
            {
                if (client.UID == uid)
                {
                    return client;
                }
            }

            return null;
        }
    }
}
