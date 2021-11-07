using System;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Xml2CSharp;

namespace Assets.Editor
{
[ExecuteInEditMode]
    public class SpawnComponent : MonoBehaviour
    {
        public static bool DrawName = true;
        public static bool DrawHomeRange = true;
        public static bool DrawWalkRange = true;

        public static bool ModernUOSpawner = true;

        public bool IsJson => DataJSON != null;
        public bool IsXML => DataXML != null;

        [ShowIf("ModernUOSpawner")] [Label("Spawn Data")]
        public ModernUOBaseSpawner DataJSON;

        [HideIf("ModernUOSpawner")] [Label("Spawn Data")]
        public XML2Spawner DataXML;

        private ISpawner _spawner => DataJSON ?? (ISpawner)DataXML;

        private int instanceID;

        private void Awake()
        {
            instanceID = gameObject.GetInstanceID();
        }

        void Update()
        {
#if(UNITY_EDITOR)
            if(!Application.isPlaying)//if in the editor
            {
                if (instanceID != gameObject.GetInstanceID()) //if the instance ID doesnt match then this was copied!
                {
                    Debug.Log("Duplicated!!: " + gameObject + ", " + transform.parent);
                    _spawner.NewGUID();
                }
                else if(instanceID == 0)
                    instanceID = gameObject.GetInstanceID();//this object wasnt copied but set its ID to check for further copies
 
                return;//prevent any actual code from running
            }
#endif
        }
        
        /* public virtual void ToJson(DynamicJson json, JsonSerializerOptions options)
         {
             json.Type = GetType().Name;
             json.SetProperty("name", options, Name);
             json.SetProperty("guid", options, _guid);
             json.SetProperty("location", options, Location);
             json.SetProperty("map", options, Map);
             json.SetProperty("count", options, Count);
             json.SetProperty("minDelay", options, MinDelay);
             json.SetProperty("maxDelay", options, MaxDelay);
             json.SetProperty("team", options, Team);
             json.SetProperty("homeRange", options, HomeRange);
             json.SetProperty("walkingRange", options, WalkingRange);
             json.SetProperty("entries", options, Entries);
         }*/
      /* [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
       static void DrawGameObjectName(Transform transform, GizmoType gizmoType)
       {   
           Handles.Label(transform.position, transform.gameObject.name);
       }*/
        private void OnDrawGizmos()
        {
            if (DrawName)
            {
                Handles.color = Color.white;
                var str = transform.gameObject.name + "\r\n";
                if(ModernUOSpawner&& DataJSON != null && DataJSON.Entries != null)
                for (var index = 0; index < DataJSON.Entries.Count; index++)
                {
                    var e = DataJSON.Entries[index];
                    str +=e.SpawnedMaxCount +"x " +  e.Name + "\r\n";
                }
                Handles.Label(transform.position + new Vector3(-5,0,-5), str);

            }
            
            if (ModernUOSpawner && DataJSON != null)
            {
              

                if (DrawHomeRange)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawWireCube(transform.position + new Vector3(0,5,0),new Vector3(DataJSON.HomeRange,5f,DataJSON.HomeRange));
                }

                if (DrawWalkRange)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireCube(transform.position + new Vector3(0,5,0),new Vector3(DataJSON.walkingRange,5f,DataJSON.walkingRange));
                }

                if (Selection.activeTransform == this.transform)
                {
                    Gizmos.color = Color.magenta;

                    Gizmos.DrawWireSphere(transform.position + new Vector3(0,5,0),LargerRange);

                } 
                // make it clickable
                Gizmos.color = Color.clear;
                Gizmos.DrawCube(transform.position + new Vector3(0,5,0),new Vector3(LargerRange,5f,LargerRange));

            }
            
            if (!ModernUOSpawner && DataXML != null)
            {
                if (DrawHomeRange || DrawWalkRange)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireCube(transform.position + new Vector3(0,5,0),new Vector3(DataXML.Range,5f,DataXML.Range));
                }

               

                if (Selection.activeTransform == this.transform)
                {
                    Gizmos.color = Color.magenta;

                    Gizmos.DrawWireSphere(transform.position + new Vector3(0,5,0),DataXML.Range);

                } 
                // make it clickable
                Gizmos.color = Color.clear;
                Gizmos.DrawCube(transform.position + new Vector3(0,5,0),new Vector3(DataXML.Range,5f,DataXML.Range));

            }
            
        }

        private int LargerRange => Math.Max(DataJSON.HomeRange, DataJSON.walkingRange);
        public void InitSpawner(ModernUOBaseSpawner spawner)
        {
            DataJSON = spawner;
            spawner.Init();
            this.transform.position = DataJSON.ToUnityPos();
            if (Physics.Raycast(transform.position, -transform.up, out var hit, Mathf.Infinity)) {
                transform.position = hit.point;
            }
            
        }

        //Pull pos etc from Unity Obj.
        public void Sync()
        {
            var pos = transform.position;
            if (_spawner is ModernUOBaseSpawner)
            {
                DataJSON.Location = new[] { (int)pos.z, (int)pos.x, (int)pos.y };
            
                DataJSON.MinDelay = TimeSpan.Parse(DataJSON.MinSpawnDelay);
                DataJSON.MaxDelay = TimeSpan.Parse(DataJSON.MaxSpawnDelay);
            }
            else
            {
                DataXML.Objects2 = DataXML.GetSerializedObjectList2();
                DataXML.X = (int)pos.z;
                DataXML.Y = (int)pos.x;
                
                DataXML.CentreX = (int)pos.z;
                DataXML.CentreY = (int)pos.x;
            }
            
            
        }

        public void InitSpawner(XML2Spawner spawner)
        {
            DataXML = spawner;
            spawner.Init();
            this.transform.position = DataXML.ToUnityPos();
            if (Physics.Raycast(transform.position, -transform.up, out var hit, Mathf.Infinity)) {
                transform.position = hit.point;
            }

        }
    }

}