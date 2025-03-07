using System;
using UnityEditor;
using UnityEngine;

namespace ProjectSetupTools.Editor
{
    [InitializeOnLoad]
    public static class HierarchyActivationDisplay
    {
        static HierarchyActivationDisplay()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemOnGUI;
        }

        private static void OnHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (gameObject!=null)
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
        }
    }
}
