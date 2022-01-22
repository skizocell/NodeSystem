using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DSGame.GraphSystem
{
    //Conditional branch in a graph
    [Serializable]
    public class Branch
    {
        public int id;
        public string label;
        [NonSerialized]
        public bool isOn;
    }
}
