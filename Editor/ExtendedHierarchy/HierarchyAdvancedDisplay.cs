using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ProjectSetupTools.Editor.ExtendedHierarchy
{
    [InitializeOnLoad]
    public static class HierarchyAdvancedDisplay
    {
        private static EditorWindow editorWindow;
        private static bool hierarchyHasFocus = false;
        
        static HierarchyAdvancedDisplay()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemOnGUI;
            EditorApplication.update += OnEditorUpdate;
        }

        private static void OnEditorUpdate()
        {
            if (editorWindow == null)
                editorWindow = EditorWindow.GetWindow(System.Type.GetType("UnityEditor.SceneHierarchyWindow,UnityEditor"));

            hierarchyHasFocus = EditorWindow.focusedWindow != null && EditorWindow.focusedWindow == editorWindow;
        }

        private static void OnHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (gameObject == null)
                return;

            DrawToggle(selectionRect, gameObject);
            DrawIcon(selectionRect, gameObject, instanceID);
        }

        private static void DrawToggle(Rect selectionRect, GameObject gameObject)
        {
            Rect toggleRect = new Rect(selectionRect);

            toggleRect.x -= 27;
            toggleRect.width = 13;

            bool active = EditorGUI.Toggle(toggleRect, gameObject.activeSelf);
            if (active != gameObject.activeSelf)
            {
                Undo.RecordObject(gameObject, "Toggle GameObject active state");
                gameObject.SetActive(active);
                EditorUtility.SetDirty(gameObject);
            }
        }
        
        private static void DrawIcon(Rect selectionRect, GameObject gameObject, int instanceID)
        {
            if (PrefabUtility.GetCorrespondingObjectFromOriginalSource(gameObject) != null)
                return;

            var mainComponentType = GetMainComponentType(gameObject);
            if (mainComponentType == null)
                return;
            if (mainComponentType == typeof(Transform) && gameObject.transform.childCount > 0)
                return;
            
            OverlayOldIcon(selectionRect, instanceID);
            DrawComponentIcon(selectionRect, mainComponentType);
        }

        private static void OverlayOldIcon(Rect selectionRect, int instanceID)
        {
            var color = UnityEditorColors.Get(
                Selection.instanceIDs.Contains(instanceID), 
                selectionRect.Contains(Event.current.mousePosition),
                hierarchyHasFocus
                );
            var background = selectionRect;
            background.width = 18.5f;
            EditorGUI.DrawRect(background, color);
        }

        private static void DrawComponentIcon(Rect selectionRect, Type mainComponentType)
        {
            GUIContent content = EditorGUIUtility.ObjectContent(null, mainComponentType);
            content.text = null;
            content.tooltip = mainComponentType.Name;
            
            if (content.image == null)
            {
                MonoScript script = MonoScript.FromScriptableObject(ScriptableObject.CreateInstance(mainComponentType)) as MonoScript;
                if (script != null)
                    content.image = AssetPreview.GetMiniThumbnail(script);
            }
            
            if (content.image == null)
                return;
            EditorGUI.LabelField(selectionRect, content);
        }

        private static Type GetMainComponentType(GameObject gameObject)
        {
            var components = gameObject.GetComponents<Component>();
            if (components == null || components.Length == 0)
                return null;

            return (components.Length > 1 ? components[1] : components[0])?.GetType();
        }
    }
}
