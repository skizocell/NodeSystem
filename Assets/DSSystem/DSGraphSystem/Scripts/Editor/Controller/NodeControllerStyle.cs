using UnityEngine;

namespace DSGame.GraphSystem
{
    //Node Controller Style
    public class NodeControllerStyle
    {
        public Texture2D headerTexture = null;
        public GUIStyle headerStyle = null;

        public NodeControllerStyle(Color headerBackGroundColor, Color textColor)
        {
            headerTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            headerTexture.SetPixel(0, 0, headerBackGroundColor);
            headerTexture.Apply();

            headerStyle = new GUIStyle();
            headerStyle.fontStyle = FontStyle.Bold;
            headerStyle.normal.textColor = textColor;
        }
    }
}
