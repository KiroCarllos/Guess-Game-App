using MaterialSkin.Controls;
using System.Net.Sockets;

namespace ClientGameApp
{
    public partial class Form1 : MaterialForm
    {
        TcpClient client;
        NetworkStream Stream;
        string Name;
        Thread GoWelcome;

        public Form1()
        {
            InitializeComponent();
        }

        private void loginBtn_Click(object sender, EventArgs e)
        {
            // 1 - connect  to server
            try
            {
                client = new TcpClient();
                client.Connect("127.0.0.1", 12345);
                Stream = client.GetStream();
                Name = loginTextBox.Text;
                new BinaryWriter(Stream).Write(Name);
                GoWelcome = new Thread(openWelcome);
                Close();
                GoWelcome.Start();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void openWelcome()
        {
            Application.Run(new LobbyForm(Stream, Name));
        }
    }
}
