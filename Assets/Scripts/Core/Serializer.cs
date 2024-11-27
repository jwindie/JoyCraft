using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;

public class Serializer : MonoBehaviour
{
    public void Save() {
        StringBuilder s = new StringBuilder("Debug Save File : Format 0x00");

        var serializableObjects = SimulationManager.GetSerializables();
        //iterate through all of them and print their information to a text file
        for (int i = 0; i < serializableObjects.Length; i ++){
            //var data = serializableObjects[i].Deserialize();
            // for (int x = 0; x < data.length; x ++) {
                // s.AppendLine (data

        }


        string path = Path.Combine(Application.dataPath, "model" + ".obj");
        File.WriteAllText(path, s.ToString());

        Debug.Log($"Mesh saved to: {path}");

        System.Diagnostics.Process.Start (path);
    }

    public void Load() {
    }
 //serialize data to file
 //serializer from file
 //validate files
 //create directory
}
