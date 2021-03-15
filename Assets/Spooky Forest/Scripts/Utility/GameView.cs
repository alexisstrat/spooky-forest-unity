#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Spooky_Forest.Scripts.Utility
{
    [InitializeOnLoad]
    public static class GameView
    {
        public enum GameViewSizeType
        {
            AspectRatio,
            FixedResolution
        }

        private static object gameViewSizesInstance;
        private static MethodInfo getGroup;

        static GameView()
        {
            var sizesType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizes");
            var singleType = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
            var instanceProp = singleType.GetProperty("instance");
            getGroup = sizesType.GetMethod("GetGroup");
            gameViewSizesInstance = instanceProp.GetValue(null, null);

        }
        
        public static void SetSize(int index)
        {
            var gameViewWindowType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
            var selectedSizeIndexProp = gameViewWindowType.GetProperty("selectedSizeIndex",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var gameViewWindow = EditorWindow.GetWindow(gameViewWindowType);
            selectedSizeIndexProp.SetValue(gameViewWindow, index, null);
        }

        public static void AddCustomSize(GameViewSizeType viewSizeType, GameViewSizeGroupType sizeGroupType, int width, int height, string text)
        {
            var group = GetGroup(sizeGroupType);
            var addCustomSize = getGroup.ReturnType.GetMethod("AddCustomSize");
            var gvsType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSize");
            var gvstType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizeType");
            var ctor = gvsType.GetConstructor(new Type[] { gvstType, typeof(int), typeof(int), typeof(string) });
            var newSize = ctor.Invoke(new object[] { (int)viewSizeType, width, height, text });
            addCustomSize.Invoke(group, new object[] { newSize });
        }

        public static int GetTotalCount(GameViewSizeGroupType sizeGroupType)
        {
            var group = GetGroup(sizeGroupType);
            var getTotalCount = getGroup.ReturnType.GetMethod("GetTotalCount");
            return (int)getTotalCount.Invoke(group, null);
        }

        public static void UpdateZoomAreaAndParent()
        {
            var gvWndType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
            var updateZoomAreaAndParentMethod = gvWndType.GetMethod("UpdateZoomAreaAndParent",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var gvWnd = EditorWindow.GetWindow(gvWndType);
            updateZoomAreaAndParentMethod.Invoke(gvWnd, null);
        }
        

        public static bool SizeExists(GameViewSizeGroupType sizeGroupType, int width, int height)
        {
            return FindSize(sizeGroupType, width, height) != -1;
        }

        public static GameViewSizeGroupType GetGameViewSizeGroupTypeByCurrentBuild()
        {
            switch (EditorUserBuildSettings.activeBuildTarget)
            {
                case BuildTarget.Android:
                    return GameViewSizeGroupType.Android;
                case BuildTarget.iOS:
                    return GameViewSizeGroupType.iOS;
            }

            return GameViewSizeGroupType.Standalone;
        }

        public static int FindSize(GameViewSizeGroupType sizeGroupType, int width, int height)
        {
            var group = GetGroup(sizeGroupType);
            var groupType = group.GetType();
            var getBuiltinCount = groupType.GetMethod("GetBuiltinCount");
            var getCustomCount = groupType.GetMethod("GetCustomCount");
            int sizesCount = (int)getBuiltinCount.Invoke(group, null) + (int)getCustomCount.Invoke(group, null);
            var getGameViewSize = groupType.GetMethod("GetGameViewSize");
            var gvsType = getGameViewSize.ReturnType;
            var widthProp = gvsType.GetProperty("width");
            var heightProp = gvsType.GetProperty("height");
            var indexValue = new object[1];
            for (int i = 0; i < sizesCount; i++)
            {
                indexValue[0] = i;
                var size = getGameViewSize.Invoke(group, indexValue);
                int sizeWidth = (int)widthProp.GetValue(size, null);
                int sizeHeight = (int)heightProp.GetValue(size, null);
                if (sizeWidth == width && sizeHeight == height)
                {
                    return i;
                }
            }
            return -1;
        }

        private static object GetGroup(GameViewSizeGroupType type)
        {
            return getGroup.Invoke(gameViewSizesInstance, new object[] { (int)type });
        }

        public static GameViewSizeGroupType GetCurrentGroupType()
        {
            var getCurrentGroupTypeProp = gameViewSizesInstance.GetType().GetProperty("currentGroupType");
            return (GameViewSizeGroupType)(int)getCurrentGroupTypeProp.GetValue(gameViewSizesInstance, null);
        }
        
        public static void SetSizeAndRepaint(int index)
        {
            var currType = GetCurrentGroupType();
            if (index < GetTotalCount(currType))
            {
                SetSize(index);
                UpdateZoomAreaAndParent();
            }
            else
            {
                Debug.LogError("Invalid quick select index.");
            }
        }
    }
}
#endif