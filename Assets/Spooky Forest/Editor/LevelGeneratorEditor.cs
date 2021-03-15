using System;
using Spooky_Forest.Scripts.Level_Construction;
using UnityEditor;
using UnityEngine;

namespace Spooky_Forest.Editor
{
    [CustomEditor(typeof(LevelGenerator))]
    public class LevelGeneratorEditor : UnityEditor.Editor
    {
        private SerializedProperty _highScorePoint;
        private SerializedProperty _spawnSpecificPlatforms;
        private SerializedProperty _grass;
        private SerializedProperty _water;
        private SerializedProperty _rails;
        private SerializedProperty _spider;
        private SerializedProperty _skeleton;
        private SerializedProperty _ghost;

        private void OnEnable()
        {
            _highScorePoint = serializedObject.FindProperty("highScorePoint");
            _spawnSpecificPlatforms = serializedObject.FindProperty("spawnOnlyThese");
            _grass = serializedObject.FindProperty("grass");
            _water = serializedObject.FindProperty("water");
            _rails = serializedObject.FindProperty("rails");
            _spider = serializedObject.FindProperty("spider");
            _skeleton = serializedObject.FindProperty("skeleton");
            _ghost = serializedObject.FindProperty("ghost");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.PropertyField(_highScorePoint);
            _spawnSpecificPlatforms.boolValue = EditorGUILayout.Toggle(new GUIContent("Spawn Specific Platforms"),_spawnSpecificPlatforms.boolValue);

            serializedObject.ApplyModifiedProperties();

            if (_spawnSpecificPlatforms.boolValue)
            {
                _grass.boolValue = EditorGUILayout.Toggle(new GUIContent("Tree Platform"),_grass.boolValue);
                _water.boolValue = EditorGUILayout.Toggle(new GUIContent("River Platform"),_water.boolValue);
                _rails.boolValue = EditorGUILayout.Toggle(new GUIContent("Train Platform"),_rails.boolValue);
                _spider.boolValue = EditorGUILayout.Toggle(new GUIContent("Spider Platform"),_spider.boolValue);
                _skeleton.boolValue = EditorGUILayout.Toggle(new GUIContent("Skeleton Platform"),_skeleton.boolValue);
                _ghost.boolValue = EditorGUILayout.Toggle(new GUIContent("Ghost Platform"),_ghost.boolValue);

            }
            serializedObject.ApplyModifiedProperties();
            
        }
    }
}