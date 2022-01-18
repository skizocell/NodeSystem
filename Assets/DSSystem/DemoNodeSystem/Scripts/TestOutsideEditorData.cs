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
                    int onIndex = -1;
                    for (int i=0; i<choice.choices.Count; i++)
                    {
                        if (choice.choices[i].isOn) onIndex = i;
                        choice.choices[i].isOn = false;
                    }
                    onIndex++;
                    if (onIndex < choice.choices.Count) choice.choices[onIndex].isOn = true;
                    Debug.Log(choice.choices[onIndex].label);

                }

            }
            
        );
    }
}
