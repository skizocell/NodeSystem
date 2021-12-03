using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DSGame.GraphSystem
{
    public class GraphExcecutor<Graph>
    {
        
        public void Excecute()
        {
            Debug.Log("Excecute Test");
            //Init();

            //GraphNode node = GetNextReadyNode();
            //while (node != null)
            //{
            //    ProcessNode(node);
            //    foreach (NodeLink l in GetNextsWaitingNodeLink(node).OrderBy(o => o.linkType))//order to make set before call see LinkType
            //    {
            //        ProcessLink(l);
            //        if (!IsWaitingLinkExistFor(l.to, false))
            //        {
            //            l.to.processStatus = NodeProcessStatus.Ready;
            //        }
            //    }
            //    node = GetNextReadyNode();
            //}
        }

        
    }
}
