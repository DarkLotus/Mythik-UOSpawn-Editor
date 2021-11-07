using System;
using System.Collections.Generic;
using NaughtyAttributes;
using Newtonsoft.Json;
using Server.Engines.Spawners;
using UnityEngine;

namespace Assets.Editor
{
    [Serializable]
    public class ModernUOBaseSpawner : ISpawner
    {
        public static List<string> Constructables = new List<string>();

        
        [JsonProperty]
        [HideInInspector] 
        public string type;

        [JsonProperty]
        [AllowNesting]
        public string guid;
        
        [JsonProperty]
        public string name;

        [JsonProperty("location")]
        [HideInInspector]
        public int[] Location;
        
        [JsonProperty]
        public string map;
        
        [JsonProperty]
        [MinValue(0)]
        [AllowNesting]
        public int count;
        


        [JsonProperty("minDelay")]
        public TimeSpan MinDelay;
        
        [JsonProperty("maxDelay")]
        public TimeSpan MaxDelay;


        [JsonIgnore][ValidateInput("IsValidTimestamp","Please enter a valid time in following format HH:MM:SS")][AllowNesting]
        public string MinSpawnDelay;// 
        
        [JsonIgnore][ValidateInput("IsValidTimestamp","Please enter a valid time in following format HH:MM:SS")][AllowNesting]
        public string MaxSpawnDelay;
        
        [JsonProperty]
        public int team;
        
        [JsonProperty("homeRange")][MinValue(0)][AllowNesting]
        public int HomeRange;
        
        [JsonProperty][MinValue(0)][AllowNesting]
        public int walkingRange;



        [JsonProperty("entries")]
        public List<SpawnerEntry> Entries;

        public ModernUOBaseSpawner()
        {
            Entries = new List<SpawnerEntry>();
            walkingRange = 10;
            MinDelay = new TimeSpan(0, 5, 0);
            MaxDelay = new TimeSpan(0, 10, 0);
            guid = Guid.NewGuid().ToString();
            Init();
        }

        public void NewGUID()
        {
            guid = Guid.NewGuid().ToString();

        }
        public void Init()
        {
            if (string.IsNullOrWhiteSpace(MinSpawnDelay))
            {
                MinSpawnDelay= MinDelay.Hours.ToString("D2") + ":" + MinDelay.Minutes.ToString("D2") + ":" + MinDelay.Seconds.ToString("D2");
                MaxSpawnDelay= MaxDelay.Hours.ToString("D2") + ":" + MaxDelay.Minutes.ToString("D2") + ":" + MaxDelay.Seconds.ToString("D2");
                //Position = new Vector3Int(Location[0], Location[1], Location[2]);
            }
            else
            {
                MinDelay = TimeSpan.Parse(MinSpawnDelay);
                MaxDelay = TimeSpan.Parse(MaxSpawnDelay);
               /* Location[0] = Position.x;
                Location[1] = Position.y;
                Location[2] = Position.z;*/
            }

        }

        public Vector3 ToUnityPos() => new Vector3(Location[1], Location[2], Location[0]);
        private bool IsValidTimestamp(string str) => (TimeSpan.TryParse(str, out _));



    }

    
    public interface ISpawner
    {
        void NewGUID();
    }
}