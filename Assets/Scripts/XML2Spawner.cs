   /* 
    Licensed under the Apache License, Version 2.0
    
    http://www.apache.org/licenses/LICENSE-2.0
    */
using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Assets.Editor;
using NaughtyAttributes;
using UnityEngine;

   namespace Xml2CSharp
{
	[XmlRoot(ElementName="Points")][Serializable]
	public class XML2Spawner  : ISpawner
	{

		[XmlElement(ElementName = "Name")]
		public string Name;//;
		[XmlElement(ElementName="UniqueId")]
		public string UniqueId;
		[XmlElement(ElementName="Map")]
		public string Map;

		[InfoBox(
			"X/Y/CentreX/CentreY are synced from the spawners visual location set in the above transform by default")]
		[XmlIgnore]
		public bool SyncFromTransForm = true;
		[XmlElement(ElementName="X")][HideIf("SyncFromTransForm")]
		public int X;
		[XmlElement(ElementName="Y")][HideIf("SyncFromTransForm")]
		public int Y;
		[XmlElement(ElementName="Width")]
		public int Width;
		[XmlElement(ElementName="Height")]
		public int Height;
		[XmlElement(ElementName="CentreX")][HideIf("SyncFromTransForm")]
		public int CentreX;
		[XmlElement(ElementName="CentreY")][HideIf("SyncFromTransForm")]
		public int CentreY;
		[XmlElement(ElementName="CentreZ")]
		public int CentreZ;
		[XmlElement(ElementName="Range")]
		public int Range;
		[XmlElement(ElementName="MaxCount")]
		public int MaxCount;
		[XmlElement(ElementName="MinDelay")]
		public int MinDelay;
		[XmlElement(ElementName="MaxDelay")]
		public int MaxDelay;
		[XmlElement(ElementName="DelayInSec")]
		public bool DelayInSec;
		[XmlElement(ElementName="Duration")]
		public int Duration;
		[XmlElement(ElementName="DespawnTime")]
		public int DespawnTime;
		[XmlElement(ElementName="ProximityRange")]
		public int ProximityRange;
		[XmlElement(ElementName="ProximityTriggerSound")]
		public string ProximityTriggerSound;
		[XmlElement(ElementName="TriggerProbability")]
		public string TriggerProbability;
		[XmlElement(ElementName="InContainer")]
		public bool InContainer;
		[XmlElement(ElementName="MinRefractory")]
		public int MinRefractory;
		[XmlElement(ElementName="MaxRefractory")]
		public int MaxRefractory;
		[XmlElement(ElementName="TODStart")]
		public int TODStart;
		[XmlElement(ElementName="TODEnd")]
		public int TODEnd;
		[XmlElement(ElementName="TODMode")]
		public int TODMode;
		[XmlElement(ElementName="KillReset")]
		public int KillReset;
		[XmlElement(ElementName="ExternalTriggering")]
		public bool ExternalTriggering;
		[XmlElement(ElementName="SequentialSpawning")]
		public int SequentialSpawning;
		[XmlElement(ElementName="AllowGhostTriggering")]
		public bool AllowGhostTriggering;
		[XmlElement(ElementName="AllowNPCTriggering")]
		public bool AllowNPCTriggering;
		[XmlElement(ElementName="SpawnOnTrigger")]
		public bool SpawnOnTrigger;
		[XmlElement(ElementName="SmartSpawning")]
		public bool SmartSpawning;
		[XmlElement(ElementName="TickReset")]
		public bool TickReset;
		[XmlElement(ElementName="Team")]
		public int Team;
		[XmlElement(ElementName="Amount")]
		public int Amount;
		[XmlElement(ElementName="IsGroup")]
		public bool IsGroup;
		[XmlElement(ElementName="IsRunning")]
		public bool IsRunning;
		[XmlElement(ElementName="IsHomeRangeRelative")]
		public bool IsHomeRangeRelative;
		[XmlElement(ElementName="Objects2")][ReadOnly]
		public string Objects2;

		[XmlIgnore]
		public SpawnObject[] Spawns;
		
		public Vector3 ToUnityPos() => new Vector3(Y, 5, X);

		public XML2Spawner()
		{
			
		}
		public XML2Spawner(bool initSpawner)
		{
			UniqueId = Guid.NewGuid().ToString();
			DelayInSec = false;
			MinDelay = 5;
			MaxDelay = 5;
		}
		public void Init()
		{
			Spawns = SpawnObject.LoadSpawnObjectsFromString2(Objects2);
			/*if (string.IsNullOrWhiteSpace(MinSpawnDelay))
			{
				MinSpawnDelay= MinDelay.Hours.ToString("D2") + ":" + MinDelay.Minutes.ToString("D2") + ":" + MinDelay.Seconds.ToString("D2");
				MaxSpawnDelay= MaxDelay.Hours.ToString("D2") + ":" + MaxDelay.Minutes.ToString("D2") + ":" + MaxDelay.Seconds.ToString("D2");
				//Position = new Vector3Int(Location[0], Location[1], Location[2]);
			}
			else
			{
				MinDelay = TimeSpan.Parse(MinSpawnDelay);
				MaxDelay = TimeSpan.Parse(MaxSpawnDelay);

			}*/
		}
		internal string GetSerializedObjectList2()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();

			foreach (SpawnObject so in Spawns)
			{
				if (sb.Length > 0)
					sb.Append(":OBJ="); // Separates multiple object types

				sb.AppendFormat("{0}:MX={1}:SB={2}:RT={3}:TO={4}:KL={5}:RK={6}:CA={7}:DN={8}:DX={9}:SP={10}:PR={11}",
					so.TypeName, so.ActualMaxCount, so.SubGroup, so.SequentialResetTime, so.SequentialResetTo, so.KillsNeeded,
					so.RestrictKillsToSubgroup ? 1 : 0, so.ClearOnAdvance ? 1 : 0, so.MinDelay, so.MaxDelay, so.SpawnsPerTick, so.PackRange);
			}

			return sb.ToString();
		}

		public void NewGUID()
		{
			UniqueId = Guid.NewGuid().ToString();
		}
	}

	[XmlRoot(ElementName="Spawns")]
	public class Spawns {
		[XmlElement(ElementName="Points")]
		public List<XML2Spawner> Points;
	}
	
	[Serializable]
	public class SpawnObject
		{
			private string m_TypeName;
			private int m_MaxCount;
			private int m_SubGroup;
			private int m_SequentialResetTo;
			private int m_KillsNeeded;
			private bool m_RestrictKillsToSubgroup = false;
			private bool m_ClearOnAdvance = true;
			private double m_MinDelay = -1;
			private double m_MaxDelay = -1;
			private int m_SpawnsPerTick = 1;
			private bool m_Disabled = false;
			private int m_PackRange = -1;
			private bool m_Ignore = false;
			// temporary variable used to calculate weighted spawn probabilities
			public bool Available;


			public List<object> SpawnedObjects;
			public double SequentialResetTime;


			// these are externally accessible to the SETONSPAWNENTRY keyword
			public string TypeName;
			public int MaxCount;

			public int ActualMaxCount;
			public int SubGroup;
			public int SpawnsPerTick;
			public int SequentialResetTo;
			public int KillsNeeded;
			public bool RestrictKillsToSubgroup;
			public bool ClearOnAdvance;
			public double MinDelay;
			public double MaxDelay;
			public int PackRange;

			public SpawnObject()
			{
				
			}

			public SpawnObject(string name, int maxamount)
			{
				TypeName = name;
				MaxCount = maxamount;
				SubGroup = 0;
				SequentialResetTime = 0;
				SequentialResetTo = 0;
				KillsNeeded = 0;
				RestrictKillsToSubgroup = false;
				ClearOnAdvance = true;
				SpawnedObjects = new List<object>();
			}

			public SpawnObject(string name, int maxamount, int subgroup, double sequentialresettime, int sequentialresetto, int killsneeded,
				bool restrictkills, bool clearadvance, double mindelay, double maxdelay, int spawnsper, int packrange)
			{
				TypeName = name;
				MaxCount = maxamount;
				SubGroup = subgroup;
				SequentialResetTime = sequentialresettime;
				SequentialResetTo = sequentialresetto;
				KillsNeeded = killsneeded;
				RestrictKillsToSubgroup = restrictkills;
				ClearOnAdvance = clearadvance;
				MinDelay = mindelay;
				MaxDelay = maxdelay;
				SpawnsPerTick = spawnsper;
				PackRange = packrange;
				SpawnedObjects = new List<object>();
			}

			internal static string GetParm(string str, string separator)
			{
				// find the parm separator in the string
				// then look for the termination at the ':'  or end of string
				// and return the stuff between
				string[] arg = SplitString(str, separator);
				//should be 2 args
				if (arg.Length > 1)
				{
					// look for the end of parm terminator (could also be eol)
					string[] parm = arg[1].Split(':');
					if (parm.Length > 0)
					{
						return (parm[0]);
					}
				}
				return (null);
			}
			public static string[] SplitString(string str, string separator)
			{
				if (str == null || separator == null) return null;

				int lastindex = 0;
				int index = 0;
				List<string> strargs = new List<string>();
				while (index >= 0)
				{
					// go through the string and find the first instance of the separator
					index = str.IndexOf(separator);
					if (index < 0)
					{
						// no separator so its the end of the string
						strargs.Add(str);
						break;
					}

					string arg = str.Substring(lastindex, index);

					strargs.Add(arg);

					str = str.Substring(index + separator.Length, str.Length - (index + separator.Length));
				}

				// now make the string args
				string[] args = new string[strargs.Count];
				for (int i = 0; i < strargs.Count; i++)
				{
					args[i] = strargs[i];
				}

				return args;
			}

			internal static SpawnObject[] LoadSpawnObjectsFromString(string ObjectList)
			{
				// Clear the spawn object list
				List<SpawnObject> NewSpawnObjects = new List<SpawnObject>();

				if (ObjectList != null && ObjectList.Length > 0)
				{
					// Split the string based on the object separator first ':'
					string[] SpawnObjectList = ObjectList.Split(':');

					// Parse each item in the array
					foreach (string s in SpawnObjectList)
					{
						// Split the single spawn object item by the max count '='
						string[] SpawnObjectDetails = s.Split('=');

						// Should be two entries
						if (SpawnObjectDetails.Length == 2)
						{
							// Validate the information

							// Make sure the spawn object name part has a valid length
							if (SpawnObjectDetails[0].Length > 0)
							{
								// Make sure the max count part has a valid length
								if (SpawnObjectDetails[1].Length > 0)
								{
									int maxCount = 1;

									try
									{
										maxCount = int.Parse(SpawnObjectDetails[1]);
									}
									catch (System.Exception)
									{ // Something went wrong, leave the default amount }
									}

									// Create the spawn object and store it in the array list
									SpawnObject so = new SpawnObject(SpawnObjectDetails[0], maxCount);
									NewSpawnObjects.Add(so);
								}
							}
						}
					}
				}

				return NewSpawnObjects.ToArray();
			}



			internal static SpawnObject[] LoadSpawnObjectsFromString2(string ObjectList)
			{
				// Clear the spawn object list
				List<SpawnObject> NewSpawnObjects = new List<SpawnObject>();

				// spawn object definitions will take the form typestring:MX=int:SB=int:RT=double:TO=int:KL=int
				// or typestring:MX=int:SB=int:RT=double:TO=int:KL=int:OBJ=typestring...
				if (ObjectList != null && ObjectList.Length > 0)
				{
					string[] SpawnObjectList = SplitString(ObjectList, ":OBJ=");

					// Parse each item in the array
					foreach (string s in SpawnObjectList)
					{
						// at this point each spawn string will take the form typestring:MX=int:SB=int:RT=double:TO=int:KL=int
						// Split the single spawn object item by the max count to get the typename and the remaining parms
						string[] SpawnObjectDetails = SplitString(s, ":MX=");

						// Should be two entries
						if (SpawnObjectDetails.Length == 2)
						{
							// Validate the information

							// Make sure the spawn object name part has a valid length
							if (SpawnObjectDetails[0].Length > 0)
							{
								// Make sure the parm part has a valid length
								if (SpawnObjectDetails[1].Length > 0)
								{
									// now parse out the parms
									// MaxCount
									string parmstr = GetParm(s, ":MX=");
									int maxCount = 1;
									int.TryParse( parmstr, out maxCount );


									// SubGroup
									parmstr = GetParm(s, ":SB=");

									int subGroup = 0;
									int.TryParse( parmstr, out subGroup );


									// SequentialSpawnResetTime
									parmstr = GetParm(s, ":RT=");
									double resetTime = 0;
									double.TryParse( parmstr, out resetTime );

									// SequentialSpawnResetTo
									parmstr = GetParm(s, ":TO=");
									int resetTo = 0;
									int.TryParse( parmstr, out resetTo );


									// KillsNeeded
									parmstr = GetParm(s, ":KL=");
									int killsNeeded = 0;
									int.TryParse( parmstr, out killsNeeded );

									// RestrictKills
									parmstr = GetParm(s, ":RK=");
									bool restrictKills = false;
									bool.TryParse( parmstr, out restrictKills );


									// ClearOnAdvance
									parmstr = GetParm(s, ":CA=");
									bool clearAdvance = true;
									// if kills needed is zero, then set CA to false by default.  This maintains consistency with the
									// previous default behavior for old spawn specs that havent specified CA
									if (killsNeeded == 0)
										clearAdvance = false;
									if (parmstr != null)
										try { clearAdvance = (int.Parse(parmstr) == 1); }
										catch { }

									// MinDelay
									parmstr = GetParm(s, ":DN=");
									double minD = -1;
									double.TryParse( parmstr, out minD );


									// MaxDelay
									parmstr = GetParm(s, ":DX=");
									double maxD = -1;
									double.TryParse( parmstr, out maxD );


									// SpawnsPerTick
									parmstr = GetParm(s, ":SP=");
									int spawnsPer = 1;
									int.TryParse( parmstr, out spawnsPer );


									// PackRange
									parmstr = GetParm(s, ":PR=");
									int packRange = -1;
									int.TryParse( parmstr, out packRange );


									// Create the spawn object and store it in the array list
									SpawnObject so = new SpawnObject(SpawnObjectDetails[0], maxCount, subGroup, resetTime, resetTo, killsNeeded,
										restrictKills, clearAdvance, minD, maxD, spawnsPer, packRange);

									NewSpawnObjects.Add(so);
								}
							}
						}
					}
				}

				return NewSpawnObjects.ToArray();
			}
		}



}