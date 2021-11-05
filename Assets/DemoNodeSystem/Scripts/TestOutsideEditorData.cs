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
        Debug.Log("Excecute Test");
        testGraph.Excecute();

        DemoNodeDialog dialog1 = ScriptableObject.CreateInstance<DemoNodeDialog>();
        DemoNodeDialog dialog2 = ScriptableObject.CreateInstance<DemoNodeDialog>();
        dialog1.name = "dialog1";
        dialog1.GetData().text = "dialog1";
        dialog2.name = "dialog2";
        dialog2.GetData().text = "dialog2";

        testGraph.AddNode(dialog1);
        testGraph.AddNode(dialog2);

        List<NodeComponent> nodes = testGraph.GetChainedList();

        NodeLink link = new NodeLink();
        link.from = dialog1;
        link.linkType = NodeLink.LinkType.NextRef;
        link.to = dialog2;

        NodeLink link2 = new NodeLink();
        link2.from = nodes.First();
        link2.linkType = NodeLink.LinkType.NextRef;
        link2.to = dialog1;

        testGraph.AddLink(link);
        testGraph.AddLink(link2);

        Debug.Log("Get Chained List");
        nodes = testGraph.GetChainedList();
        foreach (NodeComponent node in nodes)
        {
            Debug.Log("Node name = " + node.name);
        }

        Debug.Log("Excecute 2");
        testGraph.Excecute();
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
