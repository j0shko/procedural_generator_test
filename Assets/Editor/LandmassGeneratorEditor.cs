using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(LandmassGenerator))]
public class NewBehaviourScript : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LandmassGenerator generator = (LandmassGenerator)target;
        if (GUILayout.Button("Generate"))
        {
            generator.generateHeightmap();
        }
    }
}
