using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Server.Data
{
    public interface ILoader<Key, Value>
    {
        Dictionary<Key, Value> MakeDick();
    }

    public class DataManager
    {
        public static Dictionary<int, Stat> StatDict { get; private set; } = new Dictionary<int, Stat>();
        public static Dictionary<int, Skill> SkillDict { get; private set; } = new Dictionary<int, Skill>();

        public static void Init()
        {
            StatDict = LoadJSon<StatData, int, Stat>("StatData").MakeDick();
            SkillDict = LoadJSon<SkillData, int, Skill>("SkillData").MakeDick();
        }

        static Loader LoadJSon<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
        {
            string text = File.ReadAllText($"{ConfigManager.Config.dataPath}/{path}.json");
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Loader>(text);
        }
    }
}
