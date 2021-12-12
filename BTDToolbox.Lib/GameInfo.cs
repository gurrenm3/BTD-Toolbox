using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace BTDToolbox.Lib
{
    public class GameInfo
    {
        /// <summary>
        /// Contains the EXE name for each game.
        /// </summary>
        public static readonly Dictionary<GameType, string> gameExeNames = new Dictionary<GameType, string>() 
        {
            { GameType.BloonsAT, "btdadventuretime.exe" }, 
            { GameType.BloonsMC, "MonkeyCity-Win.exe" },
            { GameType.BloonsTDB, "Battles-Win.exe" },
            { GameType.BloonsTDB2, "btdb2_game.exe" },
            { GameType.BloonsTD5, "BTD5-Win.exe" }, 
            { GameType.BloonsTD6, "BloonsTD6.exe" }
        };

        /// <summary>
        /// Represents the actual game this info is for.
        /// </summary>
        public GameType Game { get; set; }

        /// <summary>
        /// The path to the game's main directory. Same folder that contains the EXE.
        /// </summary>
        public string GamePath { get; set; }

        /// <summary>
        /// The directory that holds all backups.
        /// </summary>
        public string BackupDirectory { get; set; }

        /// <summary>
        /// The version number the game had during the last backup.
        /// </summary>
        public string LastBackedupVersion { get; set; }

        /// <summary>
        /// Creates a GameInfo object based on the GameType.
        /// </summary>
        /// <param name="game"></param>
        public GameInfo(GameType game)
        {
            if (game == GameType.None)
                throw new Exception($"Tried making {nameof(GameInfo)} for the GameType \"None\"");

            Game = game;
            if (string.IsNullOrEmpty(BackupDirectory))
                BackupDirectory = $"{Environment.CurrentDirectory}\\{Game} Backups";
        }


        /// <summary>
        /// Returns whether or not <see cref="GamePath"/> exists and contains the EXE.
        /// </summary>
        /// <returns></returns>
        public bool IsGameDirValid()
        {
            bool gameDirExists = !string.IsNullOrEmpty(GamePath) && Directory.Exists(GamePath);
            bool exeExists = !string.IsNullOrEmpty(GetExePath());
            return gameDirExists && exeExists;
        }

        /// <summary>
        /// Returns the version of the game
        /// </summary>
        /// <returns></returns>
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
            if (string.IsNullOrEmpty(GamePath))
                return null;

            string exePath = $"{GamePath}\\{gameExeNames[Game]}";
            if (string.IsNullOrEmpty(exePath) || !File.Exists(exePath))
                return null;
            
            return exePath;
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
