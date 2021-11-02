using System;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    
    public class ModernUOSpawner : MonoBehaviour
    {
        public static bool DrawName = true;
        public static bool DrawHomeRange = true;
        public static bool DrawWalkRange = true;

        public BaseSpawner Data;
        
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
            Data ??= new BaseSpawner();
            if (DrawName)
            {
                Handles.color = Color.white;
                var str = transform.gameObject.name + "\r\n";
                for (var index = 0; index < Data.Entries.Count; index++)
                {
                    var e = Data.Entries[index];
                    str +=e.SpawnedMaxCount +"x " +  e.Name + "\r\n";
                }
                Handles.Label(transform.position + new Vector3(-Data.walkingRange/2.5f,0,-Data.walkingRange/2.5f), str);

            }

            if (DrawHomeRange)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireCube(transform.position + new Vector3(0,5,0),new Vector3(Data.HomeRange,5f,Data.HomeRange));
            }

            if (DrawWalkRange)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(transform.position + new Vector3(0,5,0),new Vector3(Data.walkingRange,5f,Data.walkingRange));
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

        private int LargerRange => Math.Max(Data.HomeRange, Data.walkingRange);
        public void InitSpawner(BaseSpawner spawner)
        {
            Data = spawner;
            spawner.Init();
            this.transform.position = Data.ToUnityPos();
            if (Physics.Raycast(transform.position, -transform.up, out var hit, Mathf.Infinity)) {
                transform.position = hit.point;
            }
            
        }

        //Pull pos etc from Unity Obj.
        public void Sync()
        {
            var pos = transform.position;
            Data.Location = new[] { (int)pos.z, (int)pos.x, (int)pos.y };
            
            Data.MinDelay = TimeSpan.Parse(Data.MinSpawnDelay);
            Data.MaxDelay = TimeSpan.Parse(Data.MaxSpawnDelay);
            
        }
    }

}