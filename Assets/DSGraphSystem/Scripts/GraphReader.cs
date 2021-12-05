using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DSGame.GraphSystem
{
    public class GraphReader<G> where G : Graph
    {
        public G graph;
        public GraphReader(G graph)
        {
            this.graph = graph;
        }

        public void Reset()
        {

        }

        public List<Node> GetNextNodes()
        {
            return null;
        }

        public void DoNodeLink(NodeLink l)
        {

        }

        public List<NodeLink> GetNodeLinks(Node node)
        {
            return null;
        }

        private void NodeDone(Node node)
        {

        }
    }
}
