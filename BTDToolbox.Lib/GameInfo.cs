using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace BTDToolbox.Lib
{
    public class GameInfo
    {
        static Dictionary<GameType, string> gameExeNames = new Dictionary<GameType, string>() { { GameType.BloonsAT, "btdadventuretime.exe" }, 
            { GameType.BloonsMC, "MonkeyCity-Win.exe" }, { GameType.BloonsTDB, "Battles-Win.exe" }, { GameType.BloonsTDB2, "btdb2_game.exe" },
            { GameType.BloonsTD5, "BTD5-Win.exe" }, { GameType.BloonsTD6, "BloonsTD6.exe" }};


        public GameType Game { get; set; }
        public string GamePath { get; set; }
        public string BackupDirectory { get; set; }
        public string LastBackedupVersion { get; set; }

        public GameInfo(GameType game)
        {
            Game = game;
            if (string.IsNullOrEmpty(BackupDirectory))
                BackupDirectory = $"{Environment.CurrentDirectory}\\{Game} Backups";
        }


        public bool IsGameDirValid() => !string.IsNullOrEmpty(GamePath);

        public string GetGameVersion()
        {
            string exePath = GetExePath();
            if (string.IsNullOrEmpty(exePath))
                return null;

            var version = FileVersionInfo.GetVersionInfo(exePath);
            return version.FileVersion;
        }

        public string GetExePath()
        {
            if (Game == GameType.None || string.IsNullOrEmpty(GamePath))
                return null;

            return $"{GamePath}\\{gameExeNames[Game]}";
        }

        public string GetDefaultProjectDir(bool createIfNotFound = true)
        {
            string defaultDir = $"{Environment.CurrentDirectory}\\Toolbox Projects";
            if (createIfNotFound)
                Directory.CreateDirectory(defaultDir);
            return defaultDir;
        }

        public bool HasBackup()
        {
            if (string.IsNullOrEmpty(BackupDirectory) || string.IsNullOrEmpty(LastBackedupVersion))
                return false;

            var dirInfo = new DirectoryInfo(BackupDirectory);
            if (!dirInfo.Exists || dirInfo.GetFiles().Length == 0)
                return false;

            return true;
        }

        public bool IsBackupOutOfDate()
        {
            if (!HasBackup())
                return true;

            string currentGameVersion = GetGameVersion();
            if (string.IsNullOrEmpty(currentGameVersion))
                return true;

            return LastBackedupVersion != currentGameVersion;
        }
    }
}
