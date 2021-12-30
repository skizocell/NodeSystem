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

                    Debug.Log(choice.choices.Where(c => c.isOn).Single().label);
                }
            }
        );
    }
}
