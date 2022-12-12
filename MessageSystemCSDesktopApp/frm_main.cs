using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MessageSysDataManagementLib;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;

namespace MessageSystemCSDesktopApp
{
    public partial class frm_main : Form
    {
        const int PORT = 6666;
        private TcpClient tcpClient;
        private NetworkStream clientStream;
        private string uid;
        private string publicKey;
        private string privateKey;

        public frm_main()
        {            
            InitializeComponent();

            Tuple<string, string> keyPair = KeyManagement.CreateKeyPair();
            privateKey = keyPair.Item1;
            publicKey = keyPair.Item2;
            MessageBox.Show("Private-Key:\n\n" + privateKey);

            //tc_conversations.HandleCreated += tc_conversations_HandleCreated;
            tc_conversations.Padding = new Point(15, 4);
            tc_conversations.DrawMode = TabDrawMode.OwnerDrawFixed;

            tb_uid.Text = Properties.Settings.Default.UID;
            tb_ip.Text = Properties.Settings.Default.ServerIP;

            if (tb_uid.Text == "")
            {
                tb_uid.Text = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            }
        }

        private void btn_newGroup_Click(object sender, EventArgs e)
        {
            if(tb_gid.Text == String.Empty)
                return;

            var packet = new Packet(Packet.PacketType.CreateGroup, tb_gid.Text);
            SendDataToServer(packet);
            Log("Create-Packet sent.\n");
        }

        private void btn_connect_Click(object sender, EventArgs e)
        {            
            if(tb_uid.Text == String.Empty || tb_ip.Text == String.Empty)
            {
                return;
            }

            ConnectToServer(tb_ip.Text, PORT);

            Properties.Settings.Default.ServerIP = tb_ip.Text;
            Properties.Settings.Default.Save();            

            //Starte Thread zum Empfangen von Daten
            Thread receiveDataThread = new Thread(DataIn);
            receiveDataThread.Start();

            btn_connect.Enabled = false;
            tb_ip.Enabled = false;
            lb_clients.Enabled = false;
            btn_login.Enabled = true;
            btn_register.Enabled = true;
            
        }

        private void btn_login_Click(object sender, EventArgs e)
        {
            var uid = tb_uid.Text;
            var password = tb_password.Text;
            if (String.IsNullOrEmpty(uid)) uid = "empty";
            if (String.IsNullOrEmpty(password)) password = "empty";

            this.uid = uid;
            Properties.Settings.Default.UID = uid;
            Properties.Settings.Default.Save();

            //Login            
            var packet = new Packet(Packet.PacketType.Login, uid, publicKey);
            packet.singleStringData = password;
            SendDataToServer(packet);
            Log("Login-Packet sent.\n");
        }

        private void btn_register_Click(object sender, EventArgs e)
        {
            //Register
            var uid = tb_uid2.Text;
            var password = tb_password2.Text;
            if (String.IsNullOrEmpty(uid)) uid = "empty";
            if (String.IsNullOrEmpty(password)) password = "empty";

            var packet = new Packet(Packet.PacketType.Registration, uid, publicKey);
            packet.singleStringData = password;
            SendDataToServer(packet);
            Log("Registration-Packet sent.\n");
        }

        public void SendDataToServer(Packet packet)
        {
            byte[] packetBytes = packet.ConvertToBytes();

            var length = packetBytes.Length;
            var lengthBytes = BitConverter.GetBytes(length);
            clientStream.Write(lengthBytes, 0, 4); //Senden der Länge/Größe des Textes
            clientStream.Write(packetBytes, 0, packetBytes.Length); //Senden der eingentlichen Daten/des Textes    
        }

        private void DataManagerForIncommingServerPackets(Packet packet)
        {
            string[] data;
            switch (packet.type)
            {
                case Packet.PacketType.ClientOutedGroup:
                    data = packet.singleStringData.Split(';');
                    if (this.uid == data[1])
                    {
                        Log("You outed group " + data[0] + ".");
                    }
                    else
                    {
                        Log(data[1] + " outed group " + data[0] + ".");
                    }
                    break;
                case Packet.PacketType.JoinGroupSuccess:
                    Log("Join group success.");
                    break;
                case Packet.PacketType.NewClientJoinedGroup:
                    data = packet.singleStringData.Split(';');
                    Log("New client " + data[1] + " joined your group " + data[0] + '.');
                    break;
                case Packet.PacketType.NewGroupCreated:
                    Log("New group was created.");
                    GetGroupList();
                    break;
                case Packet.PacketType.GroupList:
                    Log("Group list received.");
                    InvokeGUIThread(() => {
                        lb_groups.Items.Clear();
                        foreach (object gid in packet.data)
                        {
                            lb_groups.Items.Add((string) gid);
                        }
                    });
                    break;
                case Packet.PacketType.CreateGroupSuccess:
                    Log("Cretate group was successfull.\n");
                    MessageBox.Show("Create group was successfull", "Successfull", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    GetGroupList();
                    break;
                case Packet.PacketType.CreateGroupFail:
                    Log("Create group fail.\n");
                    MessageBox.Show("Create group failed!\n\nDetails:\n" + packet.singleStringData, "Create group failed.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case Packet.PacketType.RegistrationSuccess:
                    Log("Registration was successfull.\n");
                    MessageBox.Show("Registration was successfull.", "Registration was successfull." , MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                case Packet.PacketType.RegistrationFail:
                    Log("Register failed.\n");
                    MessageBox.Show("Registation failed!\n\nDetails:\n" + packet.singleStringData, "Register failed.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;

                case Packet.PacketType.LoginSuccess:
                    Log("Login was successfull.\n");
                    GetClientist();
                    GetGroupList();
                    InvokeGUIThread(() =>
                    {
                        this.btn_login.Enabled = false;
                        this.lb_clients.Enabled = true;
                        this.tb_uid.Enabled = false;
                        this.tb_password.Enabled = false;
                        this.btn_newGroup.Enabled = true;
                    });

                    break;
                case Packet.PacketType.LoginFail:
                    //tcpClient.Close();
                    Log("Login failed.");
                    MessageBox.Show("Login failed!\n\nDetails:\n" + packet.singleStringData, "Login failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case Packet.PacketType.ClientList:
                    Log("ClientList received.");

                    InvokeGUIThread(() => {
                        foreach (object clientdata in packet.data)
                        {
                            string[] data1 = ((string) clientdata).Split(';');
                            lb_clients.Items.Add(new LocalClientData(data1[0], data1[1]));
                        }
                    });
                    break;
                case Packet.PacketType.ClientConnected:
                    Log("New Client connected.");

                    InvokeGUIThread(() => {                                                
                        string[] data2 = (packet.singleStringData).Split(';');
                        lb_clients.Items.Add(new LocalClientData(data2[0], data2[1]));                        
                    });
                    break;
                case Packet.PacketType.ClientDisconnected:
                    InvokeGUIThread(() => {

                        string[] data3 = (packet.singleStringData).Split(';');
                        string packetDataUID = data3[0];
                        string packetDataPublicKey = data3[1];

                        foreach (ConversationTabPage conversation in tc_conversations.TabPages)
                        {
                            if(conversation.UID == packetDataUID && conversation.PublicKey == packetDataPublicKey)
                            {
                                //tc_conversations.TabPages.Remove(conversation);
                                conversation.DisableAll("Client disconnected");
                            }
                        }

                        foreach (LocalClientData item in lb_clients.Items)
                        {
                            if(packetDataUID == item.uid && packetDataPublicKey == item.publicKey)
                            {
                                lb_clients.Items.Remove(item);
                                break;
                            }
                        }                        
                    });
                    break;
                case Packet.PacketType.Message:
                    Log("Incomming Message from " + packet.uid);
                    InvokeGUIThread(() =>
                    {
                        if (String.IsNullOrEmpty(packet.messType))
                        {
                            OnNewMessage(packet.uid, packet.messageTimeStamp, KeyManagement.Decrypt(privateKey, packet.messageData));
                        }
                        else
                        {
                            var messType = packet.messType.Split(':');
                            if (messType[0] == "file" && messType[1] == "img")
                            { 
                                OnNewImageMessage(packet.uid, packet.messageTimeStamp, packet.messageData);
                            }
                        }
                    });
                    break;
                case Packet.PacketType.MessageGroup:
                    Log("Incomming Group Message from " + packet.uid);
                    InvokeGUIThread(() => {
                        if (String.IsNullOrEmpty(packet.messType))
                        {
                            OnNewMessage(packet.uid, packet.messageTimeStamp, message: packet.singleStringData, gid: packet.destinationUID);
                        }
                        else
                        {
                            var messType = packet.messType.Split(':');
                            if (messType[0] == "file" && messType[1] == "img")
                            {
                                OnNewImageMessage(packet.uid, packet.messageTimeStamp, packet.messageData, gid: packet.destinationUID);
                            }
                        }
                    });
                    break;
            }
        }
        private byte[] turnImageToByteArray(System.Drawing.Image img)
        {
            MemoryStream ms = new MemoryStream();
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            return ms.ToArray();
        }

        private void OnNewMessage(string senderUID, DateTime timeStamp, string message, string gid = null) // code here
        {            
            //wenn neue Message kommt und Fenster hat nicht den Focus oder ist minimiert dann blink
            if (!this.Focused || this.WindowState == FormWindowState.Minimized)
            {
                FlashWindow.Start(this);               
            }

            ConversationTabPage tab = null;

            if (gid == null)
            {
                tab = TabExistsForUID(senderUID);
            }
            else
            {
                tab = TabExistsForGID(gid);
                message = senderUID + ": " + message;
            }

            if (tab != null) //Tab exists
            {
                tab.NewMessageFromOther(senderUID, timeStamp, message);


                //if (TabIsActiveForUID(senderUID) == null) //Also nicht aktiv
                //{                   
                //    //Blink
                //}                
            }
            else
            {
                if (gid == null) tc_conversations.TabPages.Add(new ConversationTabPage(this, senderUID, GetPublicKeyForUID(senderUID)));
                else tc_conversations.TabPages.Add(new ConversationTabPage(this, gid));
                ConversationTabPage lastTP = (ConversationTabPage) tc_conversations.TabPages[tc_conversations.TabPages.Count - 1];
                Application.DoEvents();
                OnNewMessage(senderUID, timeStamp, message);                
                //Blink
            }           
        }
        private void OnNewImageMessage(string senderUID, DateTime timeStamp, Byte[] message, string gid = null) // code here
        {
            Image img = (Image)Packet.ByteArrayToObject(message);
            byte[] imgBytes = turnImageToByteArray(img);
            Log("This message is a image file " + img.Size);
            string imgString = Convert.ToBase64String(imgBytes);
            //wenn neue Message kommt und Fenster hat nicht den Focus oder ist minimiert dann blink
            if (!this.Focused || this.WindowState == FormWindowState.Minimized)
            {
                FlashWindow.Start(this);
            }

            ConversationTabPage tab = null;

            if (gid == null)
            {
                tab = TabExistsForUID(senderUID);
            }
            else
            {
                tab = TabExistsForGID(gid);
            }

            if (tab != null) //Tab exists
            {
                tab.NewImageMessageFromOther(senderUID, timeStamp, imgString);
               

                //if (TabIsActiveForUID(senderUID) == null) //Also nicht aktiv
                //{                   
                //    //Blink
                //}                
            }
            else
            {
                if (gid == null) tc_conversations.TabPages.Add(new ConversationTabPage(this, senderUID, GetPublicKeyForUID(senderUID)));
                else tc_conversations.TabPages.Add(new ConversationTabPage(this, gid));
                ConversationTabPage lastTP = (ConversationTabPage)tc_conversations.TabPages[tc_conversations.TabPages.Count - 1];
                Application.DoEvents();
                OnNewImageMessage(senderUID, timeStamp, message);
                //Blink
            }
        }

        private void ConnectToServer(string ip, int port)
        {
            tcpClient = new TcpClient();

            while (!tcpClient.Connected)
            {
                try
                {
                    Log("Trying to connect at " + ip + " on port " + port + "...");
                    tcpClient.Connect(new IPEndPoint(IPAddress.Parse(ip), port));
                    clientStream = tcpClient.GetStream();
                    Log("Connected");
                    MessageBox.Show("Connected!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Message: " + ex.Message);
                }
            }           
        }

        private void DataIn()
        {            
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

                    //Daten sind im Buffer-Array gespeichert
                    DataManagerForIncommingServerPackets(new Packet(buffer));
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine("No: " + ex.ErrorCode + " Message: " + ex.Message);
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Server disconnected!");
                Console.ReadLine();
                Environment.Exit(0);
            }

        }

        private void InvokeGUIThread(Action action)
        {
            Invoke(action);
        }

        private void frm_main_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }

        public void GetClientist()
        {
            SendDataToServer(new Packet(Packet.PacketType.GetClientList));
        }


        public void GetGroupList()
        {
            SendDataToServer(new Packet(Packet.PacketType.GetGroupList));
        }

        private void lb_clients_DoubleClick(object sender, EventArgs e)
        {
            if (lb_clients.SelectedItem != null)
            {
                foreach (ConversationTabPage conversation  in tc_conversations.TabPages)
                {
                    if(conversation.UID == ((LocalClientData)lb_clients.SelectedItem).uid && conversation.PublicKey == ((LocalClientData)lb_clients.SelectedItem).publicKey)
                    {
                        Log(">> Conversation already exists.");
                        tc_conversations.SelectedTab = conversation;
                        return;
                    }
                }

                tc_conversations.TabPages.Add(new ConversationTabPage(this, ((LocalClientData)lb_clients.SelectedItem).uid, ((LocalClientData)lb_clients.SelectedItem).publicKey));
                tc_conversations.SelectedTab = tc_conversations.TabPages[tc_conversations.TabPages.Count-1];
            }
        }
        
        private void lb_groups_DoubleCLick(object sender, EventArgs e)
        {
            if (lb_groups.SelectedItem != null)
            {
                var gid = lb_groups.SelectedItem.ToString();

                foreach (ConversationTabPage conversation in tc_conversations.TabPages)
                {
                    if (conversation.GID == gid)
                    {
                        Log(">> Conversation already exists.");
                        tc_conversations.SelectedTab = conversation;
                        return;
                    }


                }

                tc_conversations.TabPages.Add(new ConversationTabPage(this, gid));
                tc_conversations.SelectedTab = tc_conversations.TabPages[tc_conversations.TabPages.Count - 1];

                // Join Group
                var packet = new Packet(Packet.PacketType.JoinGroup, this.uid, this.publicKey);
                packet.singleStringData = gid;
                SendDataToServer(packet);
            }
        }

        public void Log(string message)
        {
            InvokeGUIThread(() => { tb_log.Text += ">> " + message + "\n"; tb_log.ScrollToCaret(); });
        }       
        
        public void SendMessage(string destinationID, byte[] encrypedMessage)
        {
            SendDataToServer(new Packet(Packet.PacketType.Message, DateTime.Now, uid, destinationID, encrypedMessage));
        }

        public void SendGroupMessage(string gid, string publicMessage)
        {
            var packet = new Packet(Packet.PacketType.MessageGroup, DateTime.Now, uid, gid, null);
            packet.singleStringData = publicMessage;
            SendDataToServer(packet);
        }

        public void SendImg(string destinationID, byte[] imgData)
        {
            var packet = new Packet(Packet.PacketType.Message, DateTime.Now, uid, destinationID, imgData);
            packet.messType = "file:img";
            SendDataToServer(packet);
        }

        public void SendGroupImg(string gid, byte[] imgData)
        {
            var packet = new Packet(Packet.PacketType.MessageGroup, DateTime.Now, uid, gid, null);
            packet.messageData = imgData;
            packet.messType = "file:img";
            SendDataToServer(packet);
        }

        private void tc_conversations_DrawItem(object sender, DrawItemEventArgs e)
        {
            var tabPage = this.tc_conversations.TabPages[e.Index];
            var tabRect = this.tc_conversations.GetTabRect(e.Index);
            tabRect.Inflate(-2, -2);
            
            var closeImage = Properties.Resources.IconClose;
            e.Graphics.DrawImage(closeImage, (tabRect.Right - closeImage.Width), tabRect.Top + (tabRect.Height - closeImage.Height) / 2);
            TextRenderer.DrawText(e.Graphics, tabPage.Text, tabPage.Font, tabRect, tabPage.ForeColor, TextFormatFlags.Left);            
        }

        private void tc_conversations_MouseDown(object sender, MouseEventArgs e)
        {
            for (var i = 0; i < tc_conversations.TabPages.Count; i++)
            {
                var tabRect = tc_conversations.GetTabRect(i);
                tabRect.Inflate(-2, -2);
                var closeImage = Properties.Resources.IconClose;
                var imageRect = new Rectangle((tabRect.Right - closeImage.Width), tabRect.Top + (tabRect.Height - closeImage.Height) / 2, closeImage.Width, closeImage.Height);
                if (imageRect.Contains(e.Location))
                {
                    var tab = (ConversationTabPage)tc_conversations.TabPages[i];
                    if (tab.IsGroup)
                    {
                        Log("You was out group " + tab.GID);
                        SendDataToServer(new Packet(Packet.PacketType.OutGroup, tab.GID));

                    }
                    tc_conversations.TabPages.RemoveAt(i);
                    break;
                }
            }
        }

        private ConversationTabPage TabExistsForUID(string uid)
        {
            foreach (ConversationTabPage conversation in tc_conversations.TabPages)
            {
                if(!conversation.IsGroup && conversation.UID == uid)
                {
                    return conversation;
                }
            }

            return null;
        }

        private ConversationTabPage TabExistsForGID(string gid)
        {
            foreach (ConversationTabPage conversation in tc_conversations.TabPages)
            {
                if (conversation.IsGroup && conversation.GID == gid)
                {
                    return conversation;
                }
            }

            return null;
        }

        private bool TabIsActiveForUID(string uid)
        {
            ConversationTabPage currentTab = (ConversationTabPage) tc_conversations.SelectedTab;

            if (currentTab.UID == uid)
            {
                return true;
            }           

            return false;
        }        

        private string GetPublicKeyForUID(string uid)
        {
            foreach (LocalClientData client in lb_clients.Items)
            {
                if (client.uid == uid)
                {
                    return client.publicKey;
                }
            }

            throw new Exception("Client does not exist!");
        }       

        private void tc_conversations_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConversationTabPage currentTab = (ConversationTabPage) tc_conversations.SelectedTab;            
        }

        private void frm_main_Activated(object sender, EventArgs e)
        {
            FlashWindow.Stop(this);
        }

        private void tb_password_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void frm_main_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
