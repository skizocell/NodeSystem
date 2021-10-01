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

        Debug.Log("node process (count=" + testGraph.nodes.Count + ")");
        testGraph.nodes.ForEach(p =>
        {
            ((NodeDialog)p).process.Process();
        });

        NodeDialog dialog1 = ScriptableObject.CreateInstance<NodeDialog>();
        dialog1.name = "dialog1";
        dialog1.GetData().text = "coucou";

        NodeDialog dialog2 = ScriptableObject.CreateInstance<NodeDialog>();
        dialog2.name = "dialog2";
        dialog2.GetData().text = "En garde";

        dialog1.nextCall.SetTarget(dialog2.process);//attach dialog2 to dialog1
        dialog1.emiter.SetTarget(dialog2.receiver);//call the receiver

        dialog1.Process();
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
