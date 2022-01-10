using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using DSGame.GraphSystem;

public class TestOutsideEditorData : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Graph testGraph = Resources.Load<Graph>("Test");
        int i = 0;
        testGraph.Excecute(
            n =>
            {
                if (i > 10)
                {
                    Debug.Log("Stop");
                    #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
                    #endif
                    throw new System.Exception("Stop");
                }
                if (n is DemoNodeDialog)
                {
                    DemoNodeDialog dialog = (DemoNodeDialog)n;
                    Debug.Log(dialog.data.text + " test = " + dialog.test);
                }

                else if (n is DemoNodeChoice)
                {
                    DemoNodeChoice choice = (DemoNodeChoice)n;

                    Debug.Log(choice.choices.Where(c => c.isOn).Single().label);
                }
                i++;

            }
            
        );
    }
}
