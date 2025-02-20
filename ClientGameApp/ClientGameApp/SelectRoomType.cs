using Microsoft.VisualBasic.Devices;
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

namespace ClientGameApp
{
    public partial class SelectRoomType : MaterialSkin.Controls.MaterialForm
    {
        NetworkStream Stream;
        BinaryReader Br;
        BinaryWriter Bw;
        string Name;
        string[] createResponse;
        Thread GoPlay;

        public SelectRoomType(NetworkStream stream, string name)
        {
            InitializeComponent();
            welcomeNamedLabel.Text = "Welcome , " + name;
            Stream = stream;
            Bw = new BinaryWriter(Stream);
            Br = new BinaryReader(Stream);
            Name = name;
            //suddenClose = true;
        }

        private async void SelectRoomType_Load(object sender, EventArgs e)
        {
           await readAllCategoriesFromOnlineFile();
        }

        public async Task readAllCategoriesFromOnlineFile()
        {
            string fileOnline = "https://grage.vps.kirellos.com/assets/file.txt";
            string[] lines = await ReadFileFromUrlAsync(fileOnline);
            foreach (string line in lines)
            {
                string[] parts = line.Split(':');
                if (parts.Length == 2)
                {
                    string category = parts[0].Trim();
                    if (!categoriesDropDown.Items.Contains(category))
                    {
                        categoriesDropDown.Items.Add(category);
                    }
                }
            }

        }

        static async Task<string[]> ReadFileFromUrlAsync(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string fileContent = await client.GetStringAsync(url);
                    return fileContent.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching file: {ex.Message}");
                    return new string[] { };
                }
            }
        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            if (categoriesDropDown.SelectedItem == null)
            {
                MessageBox.Show("Please select a category");
                return;
            }
            else
            {
                try
                {
                    Bw.Write("r,create," + categoriesDropDown.SelectedItem);

                    bool flag = true;
                    while (flag)
                    {
                        if (Stream.DataAvailable)
                        {
                            createResponse = Br.ReadString().Split(',');
                            flag = false;
                        }
                    }
                    GoPlay = new Thread(openPlay);
                    Close();
                    GoPlay.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            void openPlay()
            {
                Application.Run(new PlayingRoomForm(Stream, createResponse[2], createResponse[3], Name.ToString(), categoriesDropDown.SelectedItem.ToString(), "", "r"));
            }
        }
    }
}
