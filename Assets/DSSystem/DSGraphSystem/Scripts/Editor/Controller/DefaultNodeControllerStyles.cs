using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DSGame.GraphSystem
{
    //Predefined style to use with attribute NodeBox.StyleName.
    public class DefaultNodeControllerStyles
    {
        static DefaultNodeControllerStyles instance;
        
        List<NodeControllerStyleNodeStyle> styles = new List<NodeControllerStyleNodeStyle>();

        private DefaultNodeControllerStyles()
        {
            styles.Add(new NodeControllerStyleNodeStyle(new Color(0.25f, 0.4f, 0.25f), Color.white)); //green
            styles.Add(new NodeControllerStyleNodeStyle(new Color(0.086f, 0.14f, 0.60f), Color.white)); //blue dark
            styles.Add(new NodeControllerStyleNodeStyle(new Color(0.05f, 0.32f, 0.78f), Color.white)); //blue
            styles.Add(new NodeControllerStyleNodeStyle(new Color(0.05f, 0.50f, 0.78f), Color.white)); //turquoise
            styles.Add(new NodeControllerStyleNodeStyle(new Color(0.48f, 0.17f, 0.47f), Color.white)); //purple
            styles.Add(new NodeControllerStyleNodeStyle(new Color(0.60f, 0.086f, 0.14f), Color.white)); //red
            styles.Add(new NodeControllerStyleNodeStyle(new Color(0.78132f, 0.51f, 0f), Color.white)); //orange
        }

        public static NodeControllerStyleNodeStyle GetStyle(NodeBox.StyleName index)
        {
            if (instance == null) instance = new DefaultNodeControllerStyles();
            return instance.styles[(int)index];
        }
    }
}
