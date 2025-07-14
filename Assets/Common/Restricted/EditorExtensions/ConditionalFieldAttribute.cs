using UnityEngine;
using System;
using System.Collections;

//Original version of the ConditionalHideAttribute created by Brecht Lecluyse (www.brechtos.com)
//Modified by:- Soumen Koley

namespace DevCommon
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
        AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
    public class ConditionalFieldAttribute : PropertyAttribute
    {
        public string ConditionalSourceField = "";
        public string ConditionalSourceField2 = "";
        public string[] ConditionalSourceFields = new string[] { };
        public bool[] ConditionalSourceFieldInverseBools = new bool[] { };
        public bool HideInInspector = false;
        public bool Inverse = false;
        public bool UseOrLogic = false;

        public bool InverseCondition1 = false;
        public bool InverseCondition2 = false;


        // Use this for initialization
        public ConditionalFieldAttribute(string a_ConditionalSourceField)
        {
            this.ConditionalSourceField = a_ConditionalSourceField;
            this.HideInInspector = false;
            this.Inverse = false;
        }

        public ConditionalFieldAttribute(string a_ConditionalSourceField, bool a_HideInInspector)
        {
            this.ConditionalSourceField = a_ConditionalSourceField;
            this.HideInInspector = a_HideInInspector;
            this.Inverse = false;
        }

        public ConditionalFieldAttribute(string a_ConditionalSourceField, bool a_HideInInspector, bool a_Inverse)
        {
            this.ConditionalSourceField = a_ConditionalSourceField;
            this.HideInInspector = a_HideInInspector;
            this.Inverse = a_Inverse;
        }

        public ConditionalFieldAttribute(bool a_HideInInspector = false)
        {
            this.ConditionalSourceField = "";
            this.HideInInspector = a_HideInInspector;
            this.Inverse = false;
        }

        public ConditionalFieldAttribute(string[] a_ConditionalSourceFields, bool[] a_ConditionalSourceFieldInverseBools, bool a_HideInInspector, bool a_Inverse)
        {
            this.ConditionalSourceFields = a_ConditionalSourceFields;
            this.ConditionalSourceFieldInverseBools = a_ConditionalSourceFieldInverseBools;
            this.HideInInspector = a_HideInInspector;
            this.Inverse = a_Inverse;
        }

        public ConditionalFieldAttribute(string[] a_ConditionalSourceFields, bool a_HideInInspector, bool a_Inverse)
        {
            this.ConditionalSourceFields = a_ConditionalSourceFields;
            this.HideInInspector = a_HideInInspector;
            this.Inverse = a_Inverse;
        }
    }
}