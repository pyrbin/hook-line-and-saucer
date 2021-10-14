using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CharacterBody), true), CanEditMultipleObjects]
public class CharacterBodyEditor : Editor
{
    private CharacterBody Body => (target as CharacterBody);

    protected virtual void OnSceneGUI()
    {

    }

    public override void OnInspectorGUI()
    {
        base.DrawDefaultInspector();

        GUILayout.BeginHorizontal(new GUILayoutOption[0]);
        GUILayout.EndHorizontal();
    }
}

