using BTDToolbox.Lib.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace BTDToolbox.Lib.Persistance
{
    public class ToolboxProject
    {
        public const string fileExtension = ".toolbox";
        public JetMod JetProject { get; set; }
        public GameType Game { get; set; }
        public string FilePath { get; set; }


        public ToolboxProject()
        {

        }

        public ToolboxProject(GameType game, string filePath)
        {
            Game = game;
            FilePath = filePath;
        }

        public void SaveToFile() => SaveToFile(FilePath);

        public void SaveToFile(string filePath)
        {
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                WriteIndented = true,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };

            string json = JsonSerializer.Serialize(this, options);
            File.WriteAllBytes(filePath, Encoding.UTF8.GetBytes(json));
        }

        public static ToolboxProject LoadFromFile(string filePath)
        {
            var bytes = File.ReadAllBytes(filePath);
            string json = Encoding.UTF8.GetString(bytes);
            return JsonSerializer.Deserialize<ToolboxProject>(json);
        }
    }
}
