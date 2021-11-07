using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using ClassicUO.IO.Resources;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using Xml2CSharp;
using Client = ClassicUO.Client;
using Formatting = Newtonsoft.Json.Formatting;

namespace Assets.Editor
{

    public class WorldGenerator : EditorWindow
    {

        [MenuItem("UO Tools/Spawn Editor")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            WorldGenerator window = (WorldGenerator)EditorWindow.GetWindow(typeof(WorldGenerator));

            window.minSize = new Vector2(300f, 500f);
            window.maxSize = new Vector2(600f, 900f);

            window.autoRepaintOnSceneChange = true;
            window.titleContent.text = "Spawner Tools";
            window.Show();
        }

        [MenuItem("UO Tools/Add Spawner")]
        static void AddSpawner()
        {
            var go = new GameObject("New Spawner");
            var sp = go.AddComponent<SpawnComponent>();
            Selection.activeObject = go;
            SceneView.lastActiveSceneView.AlignWithView();
            var loc = go.transform.position;
            //TODO move to SKUO file loader and get avg Z from terrain/statics
            //var z = Ultima.Map.Felucca.Tiles.GetLandTile((int)loc.z, (int)loc.x);
            go.transform.position = new Vector3(loc.x, 5, loc.z);
            if (SpawnComponent.ModernUOSpawner)
                sp.DataJSON = new ModernUOBaseSpawner();
            else
                sp.DataXML = new XML2Spawner(true);

        }
     
        enum MapEnum
        {
            Felucca,
            Trammel,
            Ilshenar,
            Malas,
            Tokuno,
            TerMur
        }

        private bool showModernUOSpawnerTools;
        private bool showXMLSpawnerTools;

        private MapEnum _map;
        [SerializeField] private string _uopath = "/home/james/UOInfinity";

        public int width, height,index;
        private bool oldSize = true;
        void OnGUI()
        {
            EditorGUILayout.PrefixLabel("UO Files Path");
            _uopath = GUILayout.TextField(_uopath);
            if (GUILayout.Button("Select UO Folder"))
            {
                _uopath = EditorUtility.OpenFolderPanel("Select UO Data folder", "", "");
            }
            _map = (MapEnum)EditorGUILayout.EnumFlagsField(_map);
            if((int)_map <= 1)
                oldSize = GUILayout.Toggle(oldSize, "Use 6144 mapsize");

            if (GUILayout.Button("Generate Map"))
            {
                Client.Load(_uopath);
                index = (int)_map;

                switch (index)
                {
                    case 0: 
                    case 1:
                        if (oldSize)
                        {
                            width = 6144;
                            height = 4096;
                        }
                        else
                        {
                            width = 7168;
                            height = 4096;
                        }
                        break;
                    case 2:
                        width = 2304;
                        height = 1600;
                        break;
                    case 3:
                        width = 2560;
                        height = 2048;
                        break;
                    case 4:
                        width = 1448;
                        height = 1448;
                        break;
                    case 5:
                        width = 1280;
                        height = 4096;
                        break;
                }
               
                var text = MultiMapLoader.Instance.LoadFacet(index, width, height, 0, 0, width, height);
                var spr = Sprite.Create(text, new Rect(0, 0, width, height), Vector2.zero);
                var spr2 = SaveSpriteToEditorPath(spr, $"Assets/{_map.ToString()}.png");
                //File.WriteAllBytes("test.png",text.EncodeToPNG());
                var parent = new GameObject($"Terrain: _map.ToString()", typeof(SpriteRenderer));
                var render = parent.GetComponent<SpriteRenderer>();

                render.sprite = spr2;
                render.flipY = true;
                parent.transform.localPosition = new Vector3(height / 2, 0, width / 2);
                parent.transform.Rotate(90f,0f,90f);
                parent.transform.localScale = new Vector3(100, 100, 1);

                return;
            }

            //
            /*if (GUILayout.Button("Load Mobile/Item names"))
            {
                var dll = EditorUtility.OpenFilePanel("Select Scripts/UOContent.dll", "", "dll");
                if (!string.IsNullOrWhiteSpace(dll))
                {
                    foreach (var ty in Assembly.LoadFile(dll).ExportedTypes)
                    {
                        if(ty.GetConstructor(Type.EmptyTypes)?.GetCustomAttributesData().FirstOrDefault(t => t.AttributeType.Name == "ConstructibleAttribute") != null)
                        BaseSpawner.Constructables.Add(ty.Name);
                    }
                }
                Debug.Log($"Added {BaseSpawner.Constructables.Count} Constructibles.");
            }*/
            /*if (GUILayout.Button("Load Assets"))
            {
                Client.Load(_uopath);

            }*/
            
            EditorGUILayout.Separator();
            SpawnComponent.ModernUOSpawner =
                EditorGUILayout.Toggle("ModernUO or XML2", SpawnComponent.ModernUOSpawner);
            

            if (SpawnComponent.ModernUOSpawner)
            {
                showModernUOSpawnerTools = EditorGUILayout.Foldout(showModernUOSpawnerTools, "ModernUO JSON Spawners");

                if (GUILayout.Button("Load Spawner"))
                {
                    LoadJSONSpawnerFile();
                }
                if (GUILayout.Button("Export Selected Spawners"))
                {
                    ExportJSONSpawners(Selection.gameObjects);
                }            
                if (GUILayout.Button("Export All Spawners"))
                {
                    ExportJSONSpawners(GameObject.FindGameObjectsWithTag("Spawner"));
                }

                SpawnComponent.DrawWalkRange = EditorGUILayout.Toggle("Draw Walk Range", SpawnComponent.DrawWalkRange);

            }
            
 
            if (!SpawnComponent.ModernUOSpawner)
            {
                showXMLSpawnerTools = EditorGUILayout.Foldout(showXMLSpawnerTools, "XML Spawners");
                if (GUILayout.Button("Load Spawner"))
                {
                    LoadXMLSpawnerFile();

                }
                if (GUILayout.Button("Export Selected Spawners"))
                {
                    ExportXMLSpawners(Selection.gameObjects);
                }            
                if (GUILayout.Button("Export All Spawners"))
                {
                    ExportXMLSpawners(GameObject.FindGameObjectsWithTag("Spawner"));
                }      
            }
            SpawnComponent.DrawHomeRange = EditorGUILayout.Toggle("Draw Home Range", SpawnComponent.DrawHomeRange);
            SpawnComponent.DrawName = EditorGUILayout.Toggle("Draw Spawn Names", SpawnComponent.DrawName);


        }

        private static string lastDirectory = "";
        private void LoadXMLSpawnerFile()
        {
            if (!TagExists("Spawner"))
                CreateTag("Spawner");
            var file = EditorUtility.OpenFilePanel("Select Spawner", lastDirectory, "xml");
            lastDirectory = Path.GetDirectoryName(file);

            var parent = new GameObject(Path.GetFileNameWithoutExtension(file));

            if (!string.IsNullOrWhiteSpace(file))
            {
                var spawners = DeserializeObject<Spawns>(File.OpenRead(file));
                foreach (var spawner in spawners.Points)
                {
                    var go = new GameObject(spawner.Name);
                    go.tag = "Spawner";
                    var sp = go.AddComponent<SpawnComponent>();
                    go.transform.parent = parent.transform;
                    sp.InitSpawner(spawner);
                }
            }
        }
        

        public static T DeserializeObject<T>(Stream xml)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            using (var r = new MyXmlReader(new StreamReader(xml)))
            return (T)xs.Deserialize(r);
        }
        public class MyXmlReader : XmlTextReader
        {
            public MyXmlReader(TextReader reader) : base(reader) { }
            public override string ReadElementString()
            {
                var text = base.ReadElementString();

                // bool TryParse accepts case-insensitive 'true' and 'false'
                if (bool.TryParse(text, out bool result))
                {
                    text = XmlConvert.ToString(result);
                }

                return text;
            }
        }
        private void ExportXMLSpawners(GameObject[] gameObjects)
        {
            var export = new Spawns();
            export.Points = new List<XML2Spawner>();
            foreach (var go in gameObjects)
            {
                var sp = go.GetComponent<SpawnComponent>();
                sp.Sync();
                export.Points.Add(sp.DataXML);
            }
            var name = "spawner";
            try
            {
                name = gameObjects.First().transform.parent.gameObject.name;
            }
            catch
            {
            }
            var filepath = EditorUtility.SaveFilePanel("Save File", lastDirectory, name,"xml");
            if (string.IsNullOrWhiteSpace(filepath))
                return;
            lastDirectory = Path.GetDirectoryName(filepath);
            
            File.WriteAllText(filepath, XmlSerialize<Spawns>(export));
            Debug.Log($"Exported {export.Points.Count} spawners");
        }
        
        public static string XmlSerialize<T>(T entity) where T : class
        {
            // removes version
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            settings.Indent = true;

            XmlSerializer xsSubmit = new XmlSerializer(typeof(T));
            using (StringWriter sw = new StringWriter())
            using (XmlWriter writer = XmlWriter.Create(sw, settings))
            {
                // removes namespace
                var xmlns = new XmlSerializerNamespaces();
                xmlns.Add(string.Empty, string.Empty);

                xsSubmit.Serialize(writer, entity, xmlns);
                return sw.ToString(); // Your XML
            }
        }

        private static Sprite SaveSpriteToEditorPath(Sprite sp,string path) {
 
            string dir = Path.GetDirectoryName (path);
 
            Directory.CreateDirectory (dir);
 
            File.WriteAllBytes(path, sp.texture.EncodeToPNG());
            AssetDatabase.Refresh();
            //AssetDatabase.AddObjectToAsset(sp, path);
            AssetDatabase.SaveAssets();
 
            TextureImporter ti = AssetImporter.GetAtPath (path) as TextureImporter;
            if (ti != null)
            {
                ti.textureType = TextureImporterType.Sprite;
                ti.spritePixelsPerUnit = sp.pixelsPerUnit;
                ti.mipmapEnabled = false;
                EditorUtility.SetDirty(ti);
                ti.SaveAndReimport();
            }

            return  AssetDatabase.LoadAssetAtPath(path, typeof (Sprite)) as Sprite;
        }
        
        
        private static void ExportJSONSpawners(GameObject[] gameObjects)
        {
            var export = new List<ModernUOBaseSpawner>();
            foreach (var go in gameObjects)
            {
                var sp = go.GetComponent<SpawnComponent>();
                sp.Sync();
                export.Add(sp.DataJSON);
            }

            var name = "spawner";
            try
            {
                name = gameObjects.First().transform.parent.gameObject.name;
            }
            catch
            {
            }

            var filepath = EditorUtility.SaveFilePanel("Save File", lastDirectory, name,"json");
            if (string.IsNullOrWhiteSpace(filepath))
                return;
            lastDirectory = Path.GetDirectoryName(filepath);

            if (!string.IsNullOrWhiteSpace(filepath))
            {
                File.WriteAllText(filepath,JsonConvert.SerializeObject(export,Formatting.Indented,new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                }));
                Debug.Log($"Saved {export.Count} Spawners to {filepath}");
            }
        }
        private static void LoadJSONSpawnerFile()
        {
            if (!TagExists("Spawner"))
                CreateTag("Spawner");
            var file = EditorUtility.OpenFilePanel("Select Spawner", lastDirectory, "json");
            lastDirectory = Path.GetDirectoryName(file);
            var parent = new GameObject(Path.GetFileNameWithoutExtension(file));
            
            if (!string.IsNullOrWhiteSpace(file))
            {
                var spawners = JsonConvert.DeserializeObject<List<ModernUOBaseSpawner>>(File.ReadAllText(file));
                foreach (var spawner in spawners)
                {
                    var go = new GameObject(spawner.name);
                    go.tag = "Spawner";
                    var sp = go.AddComponent<SpawnComponent>();
                    go.transform.parent = parent.transform;
                    sp.InitSpawner(spawner);
                }
            }
        }
        
        
        private static bool TagExists(string tagName)
        {
            // Open tag manager
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
 
            // Layers Property
            SerializedProperty tagsProp = tagManager.FindProperty("tags");
            return PropertyExists(tagsProp, 0, 100, tagName);
        }
        /// <summary>
        /// Adds the tag.
        /// </summary>
        /// <returns><c>true</c>, if tag was added, <c>false</c> otherwise.</returns>
        /// <param name="tagName">Tag name.</param>
        private static bool CreateTag(string tagName)
        {
            // Open tag manager
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            // Tags Property
            SerializedProperty tagsProp = tagManager.FindProperty("tags");
            if (tagsProp.arraySize >= 100)
            {
                Debug.Log("No more tags can be added to the Tags property. You have " + tagsProp.arraySize + " tags");
                return false;
            }
            // if not found, add it
            if (!PropertyExists(tagsProp, 0, tagsProp.arraySize, tagName))
            {
                int index = tagsProp.arraySize;
                // Insert new array element
                tagsProp.InsertArrayElementAtIndex(index);
                SerializedProperty sp = tagsProp.GetArrayElementAtIndex(index);
                // Set array element to tagName
                sp.stringValue = tagName;
                Debug.Log("Tag: " + tagName + " has been added");
                // Save settings
                tagManager.ApplyModifiedProperties();
 
                return true;
            }
            else
            {
                //Debug.Log ("Tag: " + tagName + " already exists");
            }
            return false;
        }
        private static bool PropertyExists(SerializedProperty property, int start, int end, string value)
        {
            for (int i = start; i <property.arraySize; i++)
            {
                SerializedProperty t = property.GetArrayElementAtIndex(i);
                if (t.stringValue.Equals(value))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
