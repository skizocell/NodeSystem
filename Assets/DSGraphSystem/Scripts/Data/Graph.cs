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

        //Read the graph and call your process function 
        public void Excecute(Action<Node> OnProcessNode)
        {
            Init();

            Node node = GetNextReadyNode();
            while (node != null)
            {
                ProcessNode(node, OnProcessNode);
                ProcessReadyLinks(node);
                node = GetNextReadyNode();
            }
        }
        #endregion

        #region Utility method

        //Init the Process status of Nodes and NodeLinks 
        private void Init()
        {
            //Get Node waitink for a link to process
            List<Node> waitingLinkComponents = links.Select(l => l.to).ToList();

            //if node not waiting a link process -> ready else -> wait
            foreach (Node n in nodes)
            {
                if (waitingLinkComponents.Contains(n)) n.processStatus = ProcessStatus.Waiting;
                else n.processStatus = ProcessStatus.Ready;
            }
            //init link to waiting
            foreach (NodeLink l in links)
            {
                l.processStatus = ProcessStatus.Waiting;
            }
        }

        //Process node
        private void ProcessNode(Node n, Action<Node> OnProcessNode)
        {
            n.processStatus = ProcessStatus.Running;
            //if portal
            if (n is PortalIn)
            {
                ProcessPortal((PortalIn)n);
            }
            else if (n is PortalOut==false)
            {
                //Give control on user code with current active Node
                OnProcessNode(n);

                //process link (get, set, call)
                ProcessWaitingLinks(n);

                //reset previous link for an other use
                ResetPreviousLink(n);

                n.processStatus = ProcessStatus.Done;
            }
        }

        private void ProcessPortal(PortalIn portalIn)
        {
            //Create a direct node link with portal link:
            NodeLink inLink = links.Where(l => l.to.Equals(portalIn)).FirstOrDefault();
            NodeLink outLink = links.Where(l => l.from.Equals(portalIn.portalOut)).FirstOrDefault();

            if(inLink == null || outLink == null)
            {
                Debug.LogWarning("Portal not correctly set is ignored. One of them has no entry");
            }

            //Create real ling between node
            NodeLink finalLink = new NodeLink();
            finalLink.from = inLink.from;
            finalLink.to = outLink.to;
            finalLink.fromPinId = inLink.fromPinId;
            finalLink.toPinId = outLink.toPinId;
            finalLink.linkType = inLink.linkType;
            ProcessLink(finalLink);

            //Make portal in processed
            portalIn.processStatus = ProcessStatus.Done;
            portalIn.portalOut.processStatus = ProcessStatus.Done;
            inLink.processStatus = ProcessStatus.Done;
            outLink.processStatus = ProcessStatus.Done;

            //reset to reuse if a loop is occured
            ResetPreviousLink(portalIn);

            //if target node have no link waiting, this node become ready
            if (!IsReadyLinkExistFor(finalLink.to, false))
            {
                finalLink.to.processStatus = ProcessStatus.Ready;
            }
        }

        //Process Link mark it to Done, put value in another node...
        private void ProcessLink(NodeLink l)
        {
            l.processStatus = ProcessStatus.Running;
            switch (l.linkType)
            {
                //set value to another link
                case NodeLink.LinkType.Set:
                    if (l.to is PortalIn == false)
                    {
                        int indexStart = 2;
                        string fromFieldName = l.fromPinId.Substring(indexStart);
                        string toFieldName = l.toPinId.Substring(indexStart);

                        FieldInfo toParam = l.to.GetType().GetField(toFieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                        FieldInfo fromParam = l.from.GetType().GetField(fromFieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                        if (toParam == null)
                            Debug.LogError("No field named " + toFieldName + " found in " + l.to.GetType() + " Object");
                        else if (fromParam == null)
                            Debug.LogError("No field named " + fromFieldName + " found in " + l.from.GetType() + " Object");
                        else
                            toParam.SetValue(l.to, fromParam.GetValue(l.from));
                    }
                    break;
                default:
                    break;
            }
            l.processStatus = ProcessStatus.Done;
        }

        //process Waiting link to become ready (activate conditional branch)
        private void ProcessWaitingLinks(Node n)
        {
            //List of waiting list for this node
            foreach (NodeLink link in links.Where(l => l.from == n && l.processStatus == ProcessStatus.Waiting).ToList())
            {
                //if fromPinId is an expression F$(fieldName)$[0] the link is a branch list
                //link become ready when the branch isOn...
                if (link.fromPinId.LastIndexOf("$") > 1)
                {
                    //take the field name
                    int indexStart = 3;
                    int indexEnd = link.fromPinId.LastIndexOf(")");
                    string fieldName = link.fromPinId.Substring(indexStart, indexEnd - indexStart);

                    //take the index of the branch
                    indexStart = link.fromPinId.IndexOf("[") + 1;
                    indexEnd = link.fromPinId.IndexOf("]");
                    int branchIndex = int.Parse(link.fromPinId.Substring(indexStart, indexEnd - indexStart));

                    //test if it's a list of branch
                    FieldInfo fieldInfo = n.GetType().GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (typeof(IList).IsAssignableFrom(fieldInfo.FieldType))
                    {
                        List<Branch> branches = (List<Branch>)fieldInfo.GetValue(n);

                        //Get the branch and test if she is on
                        if (branches.Where(b => b.id == branchIndex).SingleOrDefault().isOn)
                        {
                            //link become ready
                            link.processStatus = ProcessStatus.Ready;
                        }
                    }
                    //if it's a solo branch
                    else if (typeof(Branch).IsAssignableFrom(fieldInfo.FieldType))
                    {
                        Branch branch = (Branch)fieldInfo.GetValue(n);
                        if (branch.isOn)
                        {
                            //link become ready
                            link.processStatus = ProcessStatus.Ready;
                        }
                    }
                }
                else
                {
                    link.processStatus = ProcessStatus.Ready;
                }
            }
        }

        //Process all ready link for a node
        private void ProcessReadyLinks(Node node)
        {
            //For each ready link on the node -> process it
            //order to make set before call (see LinkType)
            foreach (NodeLink l in GetNextsReadyNodeLink(node).OrderBy(o => o.linkType))
            {
                ProcessLink(l);
                //if target node have no link waiting, this node become ready
                if (!IsReadyLinkExistFor(l.to, false))
                {
                    l.to.processStatus = ProcessStatus.Ready;
                }
            }
        }

        //Reset Previous excecuted links to be waiting for a next process 
        private void ResetPreviousLink(Node n)
        {
            //the previous process link became waiting
            foreach(NodeLink link in links.Where(l => l.to == n))
            {
                link.processStatus = ProcessStatus.Waiting;
            }
        }

        //Is link of type caller or called exist for a node pin
        public bool IsLinkExistFor(Node component, String pinId, bool isCallerType)
        {
            if (isCallerType)
                return links.Where(l => l.from == component && l.fromPinId.Equals(pinId)).Count() > 0;
            else
            {
                return links.Where(l => l.to == component && l.toPinId.Equals(pinId)).Count() > 0;
            }
        }

        //Is ready link of type caller or called exist for a node
        private bool IsReadyLinkExistFor(Node component, bool isCallerType)
        {
            if (isCallerType)
                return links.Where(l => l.from == component && l.processStatus == ProcessStatus.Ready).Count() > 0;
            else
            {
                return links.Where(l => l.to == component && l.processStatus == ProcessStatus.Ready).Count() > 0;
            }
        }

        //Get list of next ready node link for a node
        private IEnumerable<NodeLink> GetNextsReadyNodeLink(Node node)
        {
            return links.Where(l => l.processStatus == ProcessStatus.Ready
                                    && l.from.Equals(node));
        }

        //Get next ready Nodes
        private List<Node> GetNextReadyNodes()
        {
            return nodes.Where(n => n.processStatus == ProcessStatus.Ready).ToList();
        }

        //Get next ready Node
        private Node GetNextReadyNode()
        {
            return nodes.Where(n => n.processStatus == ProcessStatus.Ready).FirstOrDefault();
        }
        #endregion
    }
}
