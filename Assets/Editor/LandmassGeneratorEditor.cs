using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(LandmassGenerator))]
public class LandmassGeneratorEditor : Editor {

    public override void OnInspectorGUI()
    {
        LandmassGenerator generator = (LandmassGenerator)target;
  
        if (DrawDefaultInspector())
        {
            
            if (generator.autoUpdate)
            {
                generator.GenerateMap();
            }
        }

        if (GUILayout.Button("Generate"))
        {
            generator.GenerateMap();
        }
    }
}
