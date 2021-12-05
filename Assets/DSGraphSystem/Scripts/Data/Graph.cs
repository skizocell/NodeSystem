using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DSGame.GraphSystem
{
    public enum ProcessStatus { Waiting, Ready, Running, Done }
    [Serializable]
    public class Graph : ScriptableObject
    {
        #region public Variables
        public string nodeGraphControllerType;
        [SerializeField]
        private List<Node> nodes = new List<Node>();
        [SerializeField]
        private List<NodeLink> links = new List<NodeLink>();
        #endregion

        #region main 
        public void AddNode(Node node)
        {
            nodes.Add(node);
        }

        public void RemoveNode(Node node)
        {
            nodes.Remove(node);
        }

        public void AddLink(NodeLink link)
        {
            links.Add(link);
        }

        public void RemoveLink(NodeLink link)
        {
            links.Remove(link);
        }

        public List<Node> GetNodes()
        {
            return nodes;
        }

        public List<NodeLink> GetLinks()
        {
            return links;
        }

        public void Excecute(Action<Node> OnProcessNode)
        {
            Init();

            Node node = GetNextReadyNode();
            while (node != null)
            {
                ProcessNode(node, OnProcessNode);
                foreach (NodeLink l in GetNextsReadyNodeLink(node).OrderBy(o => o.linkType))//order to make set before call see LinkType
                {
                    ProcessLink(l);
                    if (!IsReadyLinkExistFor(l.to, false))
                    {
                        l.to.processStatus = ProcessStatus.Ready;
                    }
                }
                node = GetNextReadyNode();
            }
        }

        //For specific nodeGraph just used with ref link and purely has sequence like dialog system.... 
        //Or with system with a preprocess that can produce a chained link node in term...
        //praticaly the Same than execute
        public List<Node> GetChainedList()
        {
            List<Node> nodeList = new List<Node>();
            Init();
            Node node = GetNextReadyNode();
            while (node != null)
            {
                nodeList.Add(node);
                node.processStatus = ProcessStatus.Done;
                foreach (NodeLink l in GetNextsReadyNodeLink(node).OrderBy(o => o.linkType))//order to make set before call see LinkType
                {
                    l.processStatus = ProcessStatus.Done;
                    if (!IsReadyLinkExistFor(l.to, false))
                    {
                        l.to.processStatus = ProcessStatus.Ready;
                    }
                }
                node = GetNextReadyNode();
            }
            return nodeList;
        }
        #endregion

        #region Utility method
        private void Init()
        {
            //Get Node waitink for a link to process
            List<Node> waitingLinkComponents = links.Select(l => l.to).ToList();

            foreach (Node n in nodes)
            {
                if (waitingLinkComponents.Contains(n)) n.processStatus = ProcessStatus.Waiting;
                else n.processStatus = ProcessStatus.Ready;
            }
            foreach (NodeLink l in links)
            {
                if(l.fromPinId.LastIndexOf("$")>1) l.processStatus = ProcessStatus.Waiting;
                else l.processStatus = ProcessStatus.Ready;
            }
        }

        public List<Node> GetNextReadyNodes()
        {
            return nodes.Where(n => n.processStatus == ProcessStatus.Ready).ToList();
        }

        public Node GetNextReadyNode()
        {
            return nodes.Where(n => n.processStatus == ProcessStatus.Ready).FirstOrDefault();
        }

        private void ProcessNode(Node n, Action<Node> OnProcessNode)
        {
            n.processStatus = ProcessStatus.Running;
            OnProcessNode(n);

            //List of waiting list for this node
            foreach(NodeLink link in links.Where(l => l.from == n && l.processStatus == ProcessStatus.Waiting).ToList())
            {
                //F$(choices)$[0]
                //TODO Improve that:
                //Activate link where branch isOn...
                if (link.fromPinId.LastIndexOf("$") > 1)
                {
                    int indexStart = 3;
                    int indexEnd = link.fromPinId.LastIndexOf(")");
                    string field = link.fromPinId.Substring(indexStart, indexEnd-indexStart);

                    indexStart = link.fromPinId.IndexOf("[")+1;
                    indexEnd = link.fromPinId.IndexOf("]");
                    int id = int.Parse(link.fromPinId.Substring(indexStart, indexEnd - indexStart));

                    FieldInfo fieldInfo = n.GetType().GetField(field, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    List<Branch> branches = (List<Branch>) fieldInfo.GetValue(n);
                    if(branches.Where(b => b.id == id).SingleOrDefault().isOn)
                    {
                        link.processStatus = ProcessStatus.Ready;
                    }
                }
            }
            ResetPreviousLink(n);
            n.processStatus = ProcessStatus.Done;
        }

        private void ResetPreviousLink(Node n)
        {
            foreach(NodeLink link in links.Where(l => l.to == n))
            {
                link.processStatus = ProcessStatus.Waiting;
            }
        }

        private void ProcessLink(NodeLink l)
        {
            l.processStatus = ProcessStatus.Running;
            switch (l.linkType)
            {
                //case NodeLink.LinkType.Call:
                //    MethodInfo method = l.to.GetType().GetMethod(l.toPinId);
                //    method.Invoke(l.to, null);
                //    break;
                //case NodeLink.LinkType.Set:
                //    MethodInfo setMethod = l.to.GetType().GetMethod(l.toPinId);
                //    MethodInfo getMethod = l.from.GetType().GetMethod(l.fromPinId);
                //    setMethod.Invoke(l.to, new object[] { getMethod.Invoke(l.from, null) });
                //    break;
                default:
                    break;
            }
            l.processStatus = ProcessStatus.Done;
        }

        public bool IsLinkExistFor(Node component, String pinId, bool isCallerType)
        {
            if (isCallerType)
                return links.Where(l => l.from == component && l.fromPinId.Equals(pinId)).Count() > 0;
            else
            {
                return links.Where(l => l.to == component && l.toPinId.Equals(pinId)).Count() > 0;
            }
        }

        private bool IsReadyLinkExistFor(Node component, bool isCallerType)
        {
            if (isCallerType)
                return links.Where(l => l.from == component && l.processStatus == ProcessStatus.Ready).Count() > 0;
            else
            {
                return links.Where(l => l.to == component && l.processStatus == ProcessStatus.Ready).Count() > 0;
            }
        }

        private IEnumerable<NodeLink> GetNextsReadyNodeLink(Node node)
        {
            return links.Where(l => l.processStatus == ProcessStatus.Ready
                                    && l.from.Equals(node));
        }
        #endregion
    }
}
