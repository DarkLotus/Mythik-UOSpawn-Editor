using System;
using System.Collections.Generic;
using Assets.Editor;
using NaughtyAttributes;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;


namespace Server.Engines.Spawners
{
    [Serializable]
    public class SpawnerEntry
    {
        [JsonProperty("name")]public string Name;
        
        [JsonProperty("maxCount")] public int SpawnedMaxCount;
        
        [JsonProperty("probability")] public int SpawnedProbability;

        //[JsonProperty("parameters")] public string Parameters;

        //[JsonProperty("properties")] public string Properties;

        public SpawnerEntry(string name, int probability, int maxcount)
        {
            this.Name = name;
            SpawnedProbability = probability;
            SpawnedMaxCount = maxcount;
        }
    }

}