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
using static System.ComponentModel.Design.ObjectSelectorEditor;
using static System.Windows.Forms.DataFormats;

namespace ClientGameApp
{
    public partial class PlayingRoomForm : MaterialSkin.Controls.MaterialForm
    {
        NetworkStream Stream;
        BinaryWriter Bw;
        BinaryReader Br;

        string RoomId;
        string Word;
        string trueName;
        string roomCategory;
        string PlayerOneName;
        string PlayerTwoName;
        string PlayerStatus;
        Thread PlayerListener;
        Thread GetMsgRequest;
        bool flagListen;
        bool flagRequest;
        StringBuilder HiddenWordBuilder = new StringBuilder();
        List<string> selected;
        string WatcherName;
        public PlayingRoomForm(NetworkStream stream, string roomId, string word, string p1, string catRoom, string p2, string seen, string Watcher = "")
        {
            InitializeComponent();
            Stream = stream;
            RoomId = roomId;
            WatcherName = Watcher;
            welcomeNamedLabel.Text = "Welcome , " + (p2 == "" ? p1 : p2);

            Word = word.ToLower();
            HiddenWordBuilder.Append(string.Empty.PadLeft(Word.Length, '-'));
            lbl_word.Text = HiddenWordBuilder.ToString();

            PlayerOneName = p1;
            PlayerTwoName = p2;
            roomCategory = catRoom;
            p1NameLabel.Text = p1;
            p2NameLabel.Text = p2;
            categoryRoomLabel.Text = catRoom;
            exit.Visible = false;
            PlayerListener = new Thread(listen);
            GetMsgRequest = new Thread(Request);

            Bw = new BinaryWriter(Stream);
            Br = new BinaryReader(Stream);


            selected = new List<string>();

            flagListen = true;
            flagRequest = true;

            // o => only One In Room
            if (seen == "o")
            {
                trueName = p1;
                p1StatusLabel.Text = "Playing";
                p2StatusLabel.Text = "";
                PlayerListener.Start();
                groupBox_Key.Enabled = true;
                PlayerStatus = "owner";
            }

            else
            {
                // Case r Mean No Player 2 will will exec when i will play and i created Room
                if (p2 == null || p2 == "")
                {
                    p1NameLabel.Text = p1;
                    GetMsgRequest.Start();
                    trueName = p1;
                    p1StatusLabel.Text = "Wating";
                    groupBox_Key.Enabled = false;
                    PlayerStatus = "owner";
                }
                // Case player two join to play
                else
                {
                    p1StatusLabel.Text = "Playing";
                    trueName = p2;
                    p2NameLabel.Text = p2;
                    PlayerStatus = "opponent";
                    p2StatusLabel.Text = "Waiting";
                    PlayerListener.Start();
                    GroupBoxReverse(0);
                }
            }

            // case => watcher join to room
            if (seen == "w")
            {
                PlayerStatus = "watcher";
                exit.Visible = true;
                welcomeNamedLabel.Text = "Welcome , " + Watcher;
                groupBox_Key.Visible = false;
            }
        }



        void listen()
        {
            try
            {
                while (flagListen)
                {
                    if (Stream.DataAvailable)
                    {
                        string[] s = Br.ReadString().Split(',');
                        // case * , a player 1 wrong answer and player two will play
                        if (s[0] == "*")
                        {
                            Invoke(() =>
                            {
                                //string tmp;
                                //tmp = p1NameLabel.Text;
                                //p1NameLabel.Text = tmp;
                                //p2NameLabel.Text = p2NameLabel.Text;
                                string tmp;
                                tmp = p1StatusLabel.Text;
                                p1StatusLabel.Text = p2StatusLabel.Text;
                                p2StatusLabel.Text = tmp;

                            });
                            if (s.Length > 1 && s[1] == "a")
                            {
                                Invoke(() =>
                                {
                                    GroupBoxReverse(1);

                                });
                            }
                            // case watcher or anther player will disable key pad
                            else if (s.Length > 1 && s[1] == "n")
                            {
                                Invoke(() => { GroupBoxReverse(0); });
                            }
                        }
                        else if (s[0].Contains(":"))
                        {
                            string[] keys = s[0].Split(":");
                            int indexesLength = keys.Length - 1;
                            selected.Add(keys[0]);
                            try
                            {
                                for (int i = 1; i <= indexesLength; i++)
                                {
                                    int j = int.Parse(keys[i]);
                                    HiddenWordBuilder.Remove(j, 1);
                                    HiddenWordBuilder.Insert(j, keys[0]);
                                    Invoke(() => { lbl_word.Text = HiddenWordBuilder.ToString(); });
                                }
                            }
                            catch (Exception ex)
                            {
                                // MessageBox.Show(ex.Message);
                            }
                        }
                        else if (s[0] == "win")
                        {
                            Invoke(() =>
                            {
                                DialogResult status = DialogResult.None;
                                status = MessageBox.Show("Winner Winner chicken dinner !!\n Do you want to play again ?!", "Congratulation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                                switch (status)
                                {
                                    case DialogResult.Yes:
                                        Bw.Write("e," + RoomId + ",yp");
                                        break;
                                    case DialogResult.No:
                                        Bw.Write("e," + RoomId + ",np");
                                        flagListen = false;
                                        Thread goWelcome = new Thread(() => Application.Run(new LobbyForm(Stream, trueName)));
                                        this.Invoke(new Action(() => Close()));
                                        goWelcome.Start();
                                        break;
                                }
                            });
                        }
                        else if (s[0] == "lose")
                        {
                            Invoke(() =>
                            {
                                DialogResult status = DialogResult.None;
                                status = MessageBox.Show("Good luck next time !!\n Do you want to play again ?!", "Try again", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                                switch (status)
                                {
                                    case DialogResult.Yes:
                                        Bw.Write("e," + RoomId + ",yp");
                                        MessageBox.Show("waiting for other player's response");
                                        break;

                                    case DialogResult.No:
                                        Bw.Write("e," + RoomId + ",np");
                                        flagListen = false;
                                        // suddenclose = false;
                                        Thread goWelcome = new Thread(() => Application.Run(new LobbyForm(Stream, trueName)));

                                        this.Invoke(new Action(() => Close()));
                                        goWelcome.Start();
                                        break;
                                }
                            });
                        }
                        else if (s[0] == "start game")
                        {
                            //will play again after finished
                            this.Invoke(new Action(() =>
                            {
                                flagListen = false;
                                Thread gotoPlay = new Thread(() => Application.Run(new PlayingRoomForm(Stream, RoomId, s[1], s[2], categoryRoomLabel.Text, s[3], s[4])));
                                Close();
                                gotoPlay.Start();
                            }));

                        }
                        else if (s[0] == "l")
                        {
                            if (s[1] == "end game")
                            {
                                flagListen = false;
                                //suddenclose = false;
                                Thread goWelcome = new Thread(() => Application.Run(new LobbyForm(Stream, string.IsNullOrEmpty(WatcherName) ? trueName : WatcherName)));

                                this.Invoke(new Action(() => Close()));
                                goWelcome.Start();


                            }
                        }
                        else if (s[0] == "watch" && s[1] == "reject")
                        {
                            if (s[2] == "end")
                            {
                                MessageBox.Show("game ended");
                            }

                            flagListen = false;
                            // suddenclose = false;
                            Thread goWelcome = new Thread(() => Application.Run(new LobbyForm(Stream, s[2])));

                            this.Invoke(new Action(() => Close()));
                            goWelcome.Start();
                        }
                        else if (s[0] == "play new")
                        {
                            this.Invoke(new Action(() =>
                            {
                                flagListen = false;
                                //   suddenclose = false;
                                Thread gotoPlay = new Thread(() => Application.Run(new PlayingRoomForm(Stream, RoomId, s[1], trueName, roomCategory, "", "r")));
                                Close();
                                gotoPlay.Start();
                            }));

                        }

                    }
                }
            }
            catch (IOException)
            {
                MessageBox.Show("server is disconnected,close and try again");
            }

        }


        void Request()
        {
            try
            {
                string Msg;
                while (flagRequest)
                {
                    if (Stream.DataAvailable)
                    {
                        Msg = Br.ReadString();
                        DialogResult Result = MessageBox.Show(Msg, "Request", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                        switch (Result)
                        {
                            case DialogResult.Yes:
                                //play
                                flagRequest = false;

                                // this code listen for player 1 whose waiting anther player join the room
                                p2NameLabel.Invoke(new Action(() =>
                                {
                                    p2NameLabel.Text = Msg.Split(':')[0];
                                    PlayerTwoName = Msg.Split(':')[0];
                                }));
                                lbl_word.Invoke(new Action(() =>
                                {
                                    groupBox_Key.Enabled = true;
                                    p1NameLabel.Text = PlayerOneName;
                                    p2NameLabel.Text = PlayerTwoName;
                                    p2StatusLabel.Text = "Waiting";
                                    p1StatusLabel.Text = "Playing";
                                    lbl_word.Show();
                                }));

                                Bw.Write("r,join," + RoomId + ",ao");
                                PlayerListener.Start();
                                break;
                            case DialogResult.No:
                                Bw.Write("r,join," + RoomId + ",ro");
                                break;
                        }
                    }
                }
            }
            catch (IOException)
            {
                MessageBox.Show("server is disconnected,close and try again");
            }
        }






        private void GroupBoxReverse(int flag)
        {
            if (flag == 1)
            {
                groupBox_Key.Enabled = true;
                foreach (Control btn in groupBox_Key.Controls)
                {
                    if (btn is Button)
                    {
                        Button button = (Button)btn;
                        string buttonText = button.Text;
                        if (selected.Contains(buttonText.ToLower()))
                            button.Enabled = false;

                    }
                }
            }
            else
            {
                groupBox_Key.Enabled = false;
            }
        }

        static List<int> FindLetterIndexes(string word, char letter)
        {
            List<int> indexes = new List<int>();
            for (int i = 0; i < word.Length; i++)
            {
                if (word[i] == letter)
                {
                    indexes.Add(i);
                }
            }
            return indexes;
        }


        private void btnA_Click(object sender, EventArgs e)
        {
            char letter = char.Parse(((Button)sender).Text.ToLower());

            ((Button)sender).Enabled = false;

            List<int> indexes = FindLetterIndexes(Word, letter);

            if (indexes.Count > 0)
            {

                Bw.Write($"g,{RoomId},{letter}:{string.Join(":", indexes)}");
                GroupBoxReverse(1);
                foreach (int i in indexes)
                {
                    HiddenWordBuilder.Remove(i, 1);
                    HiddenWordBuilder.Insert(i, letter);
                    lbl_word.Text = HiddenWordBuilder.ToString();
                }
            }
            else
            {
                Bw.Write($"g,{RoomId},*");
                //string tmp;
                //tmp = p1NameLabel.Text;
                //p1NameLabel.Text = p2NameLabel.Text;
                //p2NameLabel.Text = tmp;

                string tmp;
                tmp = p1StatusLabel.Text;
                p1StatusLabel.Text = p2StatusLabel.Text;
                p2StatusLabel.Text = tmp;
                GroupBoxReverse(0);
            }
        }

        private void PlayingRoomForm_Load(object sender, EventArgs e)
        {
            if (PlayerStatus == "watcher")
            {
                Bw.Write($"r,watch,{RoomId},get");
            }
        }

        private void exit_Click(object sender, EventArgs e)
        {
            Bw.Write("r,watch," + RoomId + ",o");
        }
    }
}
