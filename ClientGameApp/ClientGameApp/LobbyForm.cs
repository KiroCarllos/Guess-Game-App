using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using MaterialSkin.Controls;
namespace ClientGameApp
{
    public partial class LobbyForm : MaterialForm
    {
        Dictionary<int, string> RoomList;
        BinaryReader Br;
        BinaryWriter Bw;
        string Name;
        NetworkStream Stream;
        string[] rooms;
        Thread chooseCategory;
        Thread goplay;
        string[] joinRoom;
        public LobbyForm(NetworkStream stream, string Name)
        {
            InitializeComponent();
            welcomeNamedLabel.Text = "Welcome , " + Name;
            Br = new BinaryReader(stream);
            Bw = new BinaryWriter(stream);
            Stream = stream;
            this.Name = Name;
            joinBtn.Enabled = false;
            watchBtn.Enabled = false;
            RoomList = new Dictionary<int, string>();
        }

        public void getRooms()
        {
            bool flag = true;
            roomListView.Items.Clear();
            RoomList.Clear();
            Bw.Write("r,get");
            while (flag)
            {
                if (Stream.DataAvailable)
                {
                    rooms = Br.ReadString().Split(';');
                    flag = false;
                }
            }
            for (int i = 0; i < rooms.Length; i++)
            {
                if (rooms[i].Contains(","))
                {
                    try
                    {
                        string[] roomInfo = rooms[i].Split(",");
                        int roomId = int.Parse(roomInfo[0]);
                        RoomList.Add(roomId, rooms[i]);
                        roomListView.Items.Add($"Room:{roomInfo[0]}  ,Host:{roomInfo[2]}  ,Opponent:{roomInfo[3]}  ,Status:{roomInfo[4]}  ,Category:{roomInfo[6]}  ,Watchers:{roomInfo[8]}");
                    }catch (Exception e)
                    {

                    }
                }
            }
        }

        private void LobbyForm_Load(object sender, EventArgs e)
        {
            getRooms();
        }

        private void createBtn_Click(object sender, EventArgs e)
        {
            chooseCategory = new Thread(openSelectRoomCategory);
            Close();
            chooseCategory.Start();
        }
        public void openSelectRoomCategory()
        {
            Application.Run(new SelectRoomType(Stream, Name));
        }

        private void roomListView_SelectedValueChanged(object sender, EventArgs e)
        {
            if (roomListView.SelectedItem != null)
            {
                joinBtn.Enabled = true;
                watchBtn.Enabled = true;
            }
        }

        private void joinBtn_Click(object sender, EventArgs e)
        {
            string[] m = roomListView.SelectedItem.ToString().Split(",");             //Room:10,Owner:david,Opponent:mina,Status:playing,Category:Anime,Watchers:10
            string[] cell = m[0].Split(":");
            int roomId = int.Parse(cell[1]);
            string player2 = m[2].Split(":")[1].Replace(" ","");
            if (!string.IsNullOrEmpty(player2))
            {
                MessageBox.Show("Sorry Room Is Full");
            }
            else
            {
                joinRoom = RoomList[roomId].Split(",");
                try
                {
                    Bw.Write("r,join," + roomId);
                    //wait for the request

                    bool flag = true;
                    string OwnerMsg;

                    while (flag)
                    {
                        if (Stream.DataAvailable)
                        {
                            OwnerMsg = Br.ReadString();
                            switch (OwnerMsg)
                            {
                                case "accept":
                                    goplay = new Thread(play);
                                    Close();
                                    goplay.Start();
                                    break;
                                case "reject":
                                    MessageBox.Show("can't watch");
                                    break;

                            }
                            flag = false;
                        }
                    }
                }
                catch (IOException)
                {
                    MessageBox.Show("Server is disconnected,close and try again");
                }
            }

        }

        void play()
        {
            // i will join to play else i will watch the game
            if (joinRoom[3] == "")
                Application.Run(new PlayingRoomForm(Stream, joinRoom[0], joinRoom[7], joinRoom[2], joinRoom[6], Name, "r"));
            else
                Application.Run(new PlayingRoomForm(Stream, joinRoom[0], joinRoom[7], joinRoom[2], joinRoom[6], joinRoom[3], "w",Name));

        }

        private void watchBtn_Click(object sender, EventArgs e)
        {
            string[]  m = roomListView.SelectedItem.ToString().Split(",");
            string[] cell = m[0].Split(":");
            int i = int.Parse(cell[1]);
            joinRoom = RoomList[i].Split(",");
            bool player2 = string.IsNullOrEmpty(m[2].Split(":")[1].Replace(" ", ""));
            if (player2)
            {
                MessageBox.Show("Sorry, Room Not Start Yet!");
            }
            else
            {
                try
                {
                    Bw.Write($"r,watch,{i},i");

                    bool flag = true;
                    string OwnerMsg;
                    while (flag)
                    {
                        if (Stream.DataAvailable)
                        {
                            OwnerMsg = Br.ReadString();
                            switch (OwnerMsg)
                            {
                                case "watch,accept":
                                    goplay = new Thread(play);
                                    Close();
                                    goplay.Start();
                                    break;
                                case "watch,reject":
                                    MessageBox.Show("refused");
                                    break;
                            }
                            flag = false;
                        }
                    }
                }
                catch (IOException)
                {
                    MessageBox.Show("Server is disconnected,close and try again");
                }

            }

        }
    }
}
