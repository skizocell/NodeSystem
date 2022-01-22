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
        int onIndex = 0;
        testGraph.Excecute(
            n =>
            {
                if (n is DemoNodeDialog)
                {
                    DemoNodeDialog dialog = (DemoNodeDialog)n;
                    Debug.Log(dialog.data.text);
                }

                else if (n is DemoNodeChoice)
                {
                    DemoNodeChoice choice = (DemoNodeChoice)n;

                    for (int i=0; i<choice.choices.Count; i++)
                    {
                        if (onIndex==i) choice.choices[i].isOn = true; 
                        else choice.choices[i].isOn = false;
                    }
                    Debug.Log(choice.choices[onIndex].label);
                    onIndex++;
                }
            }
        );
    }
}
