using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace BTDToolbox.Lib
{
    public class Settings
    {
        public static string defaultPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\BTD Toolbox\\Settings.json";

        public static Settings Instance
        {
            get { return instance == null? instance = Load() : instance; }
            set { instance = value; }
        }
        private static Settings instance;

        public List<GameInfo> AllGameInfo { get; set; } = new List<GameInfo>();


        public Settings()
        {

        }

        public void TryGetAllGameDirs()
        {
            /* Enum.GetValues<GameType>().Where(game => game != GameType.None).ForEach(game =>
             {
                 AllGameInfo.Add(new GameInfo() { Game = game, GamePath = SteamUtils.GetGameDir(game) });
             });*/

            foreach (var game in Enum.GetValues<GameType>())
            {
                if (game == GameType.None)
                    continue;

                AllGameInfo.Add(new GameInfo(game) { GamePath = SteamUtils.GetGameDir(game) });
            }
        }

        public GameInfo GetGameInfo(GameType selectedGame)
        {
            return AllGameInfo?.Find(game => game?.Game == selectedGame);
        }

        public void Save() => Save(defaultPath);

        public void Save(string filePath)
        {
            var json = JsonSerializer.Serialize<Settings>(this, new JsonSerializerOptions() { WriteIndented = true });
            new FileInfo(filePath).Directory.Create();
            File.WriteAllBytes(filePath, Encoding.UTF8.GetBytes(json));
        }
        
        public static Settings Load() => Load(defaultPath);

        public static Settings Load(string filePath)
        {
            if (!File.Exists(filePath))
            {
                var settings = new Settings();

                settings.TryGetAllGameDirs();

                settings.Save(filePath);
                return settings;
            }
            else
            {
                string json = Encoding.UTF8.GetString(File.ReadAllBytes(filePath));
                return JsonSerializer.Deserialize<Settings>(json);
            }
        }
    }
}
