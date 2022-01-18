using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

namespace DSGame.GraphSystem
{
    //Custom inspector screen for portalin
    [CustomEditor(typeof(PortalIn))]
    public class PortalInDialogEditor : Editor
    {
        PortalIn portalIn;
        public string[] choices;
        public int portalOutSelectedIndex = 0;
        public int portalOutCurentIndex = 0;

        private void OnEnable()
        {
            portalIn = (PortalIn)target;
            choices = portalIn.graph.GetNodes().Where(n => n is PortalOut).Select(n => n.name).OrderBy(n => n).ToArray();
            InitCurrentSelectedPortalIndex();
            portalOutCurentIndex = -1;
            Update();
        }

        public override void OnInspectorGUI()
        {
            portalIn.name = EditorGUILayout.TextField("Portal name", portalIn.name);
            SerializedObject serializedObject = new UnityEditor.SerializedObject(portalIn);

            if (choices.Count() == 0)
            {
                EditorGUILayout.LabelField("No portal out found in this graph");
            }
            else
            {
                Update();
                InitCurrentSelectedPortalIndex();
                portalOutSelectedIndex = EditorGUILayout.Popup(portalOutSelectedIndex, choices);
            }
        }

        private void InitCurrentSelectedPortalIndex()
        {
            int i = 0;
            if (portalIn.portalOut != null)
            {
                foreach (string choice in choices)
                {
                    if (choice == portalIn.portalOut.name)
                    {
                        portalOutSelectedIndex = i;
                    }
                    i++;
                }
            }
        }

        private void Update()
        {
            if (portalOutSelectedIndex != portalOutCurentIndex)
            {
                portalOutCurentIndex = portalOutSelectedIndex;
                OnChangePortalOut();
            }
        }

        private void OnChangePortalOut()
        {
            portalIn.portalOut = (PortalOut)portalIn.graph.GetNodes().Where(n => n is PortalOut && n.name == choices[portalOutCurentIndex]).FirstOrDefault();
            portalIn.isEditorUpdateNeeded = true;
        }
    }
}
