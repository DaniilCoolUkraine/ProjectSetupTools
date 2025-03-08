using UnityEditor;
using UnityEngine;

namespace ProjectSetupTools.Editor.ExtendedHierarchy
{
    public static class UnityEditorColors
    {
        private static readonly Color _defaultColor = new Color(0.7843f, 0.7843f, 0.7843f);
        private static readonly Color _defaultProColor = new Color(0.2196f, 0.2196f, 0.2196f);
        
        private static readonly Color _selectedColor = new Color(0.22745f, 0.447f, 0.6902f);
        private static readonly Color _selectedProColor = new Color(0.1725f, 0.3647f, 0.5294f);
        
        private static readonly Color _selectedUnfocusedColor = new Color(0.68f, 0.68f, 0.68f);
        private static readonly Color _selectedUnfocusedProColor = new Color(0.3f, 0.3f, 0.3f);
        
        private static readonly Color _hoveredColor = new Color(0.698f, 0.698f, 0.698f);
        private static readonly Color _hoveredProColor = new Color(0.2706f, 0.2706f, 0.2706f);

        public static Color Get(bool isSelected = false, bool isHovered = false, bool isFocused = false)
        {
            if (isSelected)
            {
                if (isFocused)
                    return EditorGUIUtility.isProSkin ? _selectedProColor : _selectedColor;
      
                return EditorGUIUtility.isProSkin ? _selectedUnfocusedProColor : _selectedUnfocusedColor;
            }
            if (isHovered)
                return EditorGUIUtility.isProSkin ? _hoveredProColor : _hoveredColor;
            
            return EditorGUIUtility.isProSkin ? _defaultProColor : _defaultColor;
        }
    }
}