// ----------------------------------------------------------------------------
// -- Project : https://github.com/instance-id/SO-Persistent-Reference --------
// -- instance.id 2020 | http://github.com/instance-id | http://instance.id ---
// ----------------------------------------------------------------------------

using UnityEditor;
using UnityEngine;

namespace instance.id.SOReference.Utils.Editor
{
    // -- Custom drawer for the LargeHeader attribute
    [CustomPropertyDrawer (typeof (HeaderLarge))]
    public class HeaderLargeDrawer : DecoratorDrawer 
    {
        // -- Used to calculate the height of the box
        // public static Texture2D lineTex = null;
        private GUIStyle style;
		
        HeaderLarge largeHeader => (HeaderLarge) attribute;

        // -- Retrieve Element Height 
        public override float GetHeight () 
        {
            return base.GetHeight () * 2f;
        }
        
        // -- Override attribute drawer
        public override void OnGUI (Rect pos) 
        {	
            // -- Get supplied color. If none exists, default to white
            Color color = Color.white;
            switch (largeHeader.color.ToLower())
            {
                case "black": color = Color.black; break;
                case "blue": color = Color.blue; break;
                case "gray": color = Color.gray; break;
                case "green": color = Color.green; break;
                case "grey": color = Color.grey; break;
                case "red": color = Color.red; break;
                case "white": color = Color.white; break;
            }

            // -- Slightly dim 
            // color *= 0.7f;

            style = new GUIStyle(GUI.skin.label);
            style.fontSize = 16;
            style.fontStyle = FontStyle.Normal;
            style.alignment = TextAnchor.LowerLeft;
            GUI.color = color;

            Rect labelRect = pos;
            EditorGUI.LabelField(labelRect, largeHeader.name, style);
            GUI.color = Color.white;
        }
    }
}