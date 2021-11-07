#  A Unity Editor based visual spawn editor

### Supports ModernUO .json Spawners as well as the XML2 Spawners found in ServUO and RunUO.


1. To get started open the project in Unity Editor, anything past 2018 should work okay, but personally used 2021.1
2. You should see a toolbar entry called "UO Tools", you can open the main tool window from here
3. Set your UO folder path.
4. Select JSON or XML spawners
5. Process to Load an XML or JSON spawner file.

You can now make any changes to each spawner, or use the Tool menu to add a new spawner, or simply duplicate an existing one

Once finished hit export all spawners to save the spawners back to a file.

Currently does not support converting spawners between types.


Uses parts of XML2Spawner to handle parsing XML2 Spawners. 
Makes use of parts of ClassicUO to handle file loading https://github.com/ClassicUO/ClassicUO
