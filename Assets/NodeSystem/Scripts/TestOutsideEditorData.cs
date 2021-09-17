using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class TestOutsideEditorData : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        NodeGraph testGraph = Resources.Load<NodeGraph>("Test");

        if (testGraph == null) return;
        //AssetBundle.LoadFromFile(Application.dataPath + " / NodeEditor / Database / Test.asset"); //To test
        Debug.Log("Enable Test " + testGraph.name);

        Debug.Log("node process (count="  + testGraph.nodes.Count+")");
        testGraph.nodes.ForEach(p =>
        {
            p.Update();
        });

        NodePinCall pin = new NodePinCall(); //initial caller Starter

        NodeDialog dialog1 = new NodeDialog();
        dialog1.GetData().text = "coucou";

        pin.SetTarget(dialog1.process); //attach the starter to dialog1


        NodeDialog dialog2 = new NodeDialog();
        dialog2.GetData().text = "En garde";

        dialog1.nextCall = new NodePinCall(); //attach dialog2 to dialog1
        dialog1.nextCall.SetTarget(dialog2.process);

        pin.Process();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Test()
    {
        Debug.Log("Test called");
    }
}
