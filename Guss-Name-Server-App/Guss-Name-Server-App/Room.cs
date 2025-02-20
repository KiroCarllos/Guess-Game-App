using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guss_Name_Server_App
{
    enum RoomStatus { playing, waiting };

    internal class Room
    {
        private static readonly HttpClient client = new HttpClient();

        public Dictionary<int, Player> Watchers = new Dictionary<int, Player>();

        public int NumberOfPlayers { set; get; }
        public RoomStatus Status { set; get; }
        public int RoomId { set; get; }
        public static int Count = 0;
        public Dictionary<int, char> WordDict { set; get; }
        public string Word { set; get; }
        public string Category { set; get; }
        public Player Owner { set; get; }
        public Player Opponent { set; get; }
        public bool Join { set; get; } = true;
        public static int WatchersCount { set; get; } = 0;
        public List<string> doneFromWord = new List<string>();

        public string winner { set; get; }
        public string loser { set; get; }
        public  Room(string category)
        {
            WordDict = new Dictionary<int, char>();
            NumberOfPlayers = 1;
            Status = RoomStatus.waiting;
            RoomId = Count++;
            Category = category;
        }

        public static async Task<Room> CreateAsync(string category)
        {
            Room room = new Room(category);
            await room.fillWord();
            return room;
        }

        public async Task fillWord()
        {
            Word = await SelectedWord();
        }
        public override string ToString()
        {
            string Msg;
            if (Opponent != null)
            {
                Msg = RoomId.ToString() + ',' + NumberOfPlayers + ',' + Owner.Name + ',' + Opponent.Name + ',' + Status + ',' + Join.ToString() + ',' + Category + ',' + Word + ',' + Watchers.Count + ';';
            }
            else
            {
                Msg = RoomId.ToString() + ',' + NumberOfPlayers + ',' + Owner.Name + ',' + "" + ',' + Status + ',' + Join.ToString() + ',' + Category + ',' + Word + ',' + Watchers.Count + ';';
            }
            return Msg;
        }

        public async Task<string> SelectedWord()
        {
            string fileOnline = "https://grage.vps.kirellos.com/assets/file.txt";
            string[] lines = await ReadFileFromUrlAsync(fileOnline);
            Dictionary<string, List<string>> wordDictionary = new Dictionary<string, List<string>>();
            foreach (string line in lines)
            {
                string[] parts = line.Split(':');
                if (parts.Length == 2)
                {
                    string category = parts[0].Trim();
                    string word = parts[1].Trim();
                    if (!wordDictionary.ContainsKey(category))
                    {
                        wordDictionary[category] = new List<string>();
                    }
                    wordDictionary[category].Add(word);
                }
            }

            if (wordDictionary.ContainsKey(Category))
            {
                List<string> wordsForCategory = wordDictionary[Category];
                Word = wordsForCategory[new Random().Next(wordsForCategory.Count)];

            }

            char[] newWord = Word.ToCharArray();
            WordDict.Clear();
            for (int i = 0; i < newWord.Length; i++)
            {
                WordDict.Add(i, newWord[i]);
            }

            return Word;
        }
        static async Task<string[]> ReadFileFromUrlAsync(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string fileContent = await client.GetStringAsync(url);

                    //return fileContent.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                    return fileContent.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching file: {ex.Message}");
                    return new string[] { };  // Return an empty array in case of error
                }
            }
        }


        public async void Score()
        {
            string url = "https://grage.vps.kirellos.com/assets/score.txt";
            string data = $"{winner} win, {loser} lose\n";
            var content = new StringContent(data, Encoding.UTF8, "text/plain");

            try
            {
                HttpResponseMessage response = await client.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                Console.WriteLine("Score updated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
