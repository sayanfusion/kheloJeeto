using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

//Original version of the ConditionalHideAttribute created by Brecht Lecluyse (www.brechtos.com)
//Modified by:- Soumen Koley

namespace DevCommon
{
    [CustomPropertyDrawer(typeof(ConditionalFieldAttribute))]
    public class ConditionalFieldAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect a_Position, SerializedProperty a_Property, GUIContent a_Label)
        {
            ConditionalFieldAttribute t_CondFAtt = (ConditionalFieldAttribute)attribute;
            bool t_Enabled = GetConditionalHideAttributeResult(t_CondFAtt, a_Property);

            bool t_WasEnabled = UnityEngine.GUI.enabled;
            UnityEngine.GUI.enabled = t_Enabled;
            if (!t_CondFAtt.HideInInspector || t_Enabled)
            {
                EditorGUI.PropertyField(a_Position, a_Property, a_Label, true);
            }

            UnityEngine.GUI.enabled = t_WasEnabled;
        }

        public override float GetPropertyHeight(SerializedProperty a_Property, GUIContent a_Label)
        {
            ConditionalFieldAttribute t_CondFAtt = (ConditionalFieldAttribute)attribute;
            bool t_Enabled = GetConditionalHideAttributeResult(t_CondFAtt, a_Property);

            if (!t_CondFAtt.HideInInspector || t_Enabled)
            {
                return EditorGUI.GetPropertyHeight(a_Property, a_Label);
            }
            else
            {
                //The property is not being drawn
                //We want to undo the spacing added before and after the property
                return -EditorGUIUtility.standardVerticalSpacing;
                //return 0.0f;
            }


            /*
            //Get the base height when not expanded
            var height = base.GetPropertyHeight(property, label);

            // if the property is expanded go through all its children and get their height
            if (property.isExpanded)
            {
                var propEnum = property.GetEnumerator();
                while (propEnum.MoveNext())
                    height += EditorGUI.GetPropertyHeight((SerializedProperty)propEnum.Current, GUIContent.none, true);
            }
            return height;*/
        }

        private bool GetConditionalHideAttributeResult(ConditionalFieldAttribute a_CondFAtt, SerializedProperty a_Property)
        {
            bool t_Enabled = (a_CondFAtt.UseOrLogic) ? false : true;

            //Handle primary property
            SerializedProperty t_SourcePropertyValue = null;
            //Get the full relative property path of the sourcefield so we can have nested hiding.Use old method when dealing with arrays
            if (!a_Property.isArray)
            {
                string t_PropertyPath = a_Property.propertyPath; //returns the property path of the property we want to apply the attribute to
                string t_ConditionPath = t_PropertyPath.Replace(a_Property.name, a_CondFAtt.ConditionalSourceField); //changes the path to the conditionalsource property path
                t_SourcePropertyValue = a_Property.serializedObject.FindProperty(t_ConditionPath);

                //if the find failed->fall back to the old system
                if (t_SourcePropertyValue == null)
                {
                    //original implementation (doens't work with nested serializedObjects)
                    t_SourcePropertyValue = a_Property.serializedObject.FindProperty(a_CondFAtt.ConditionalSourceField);
                }
            }
            else
            {
                //original implementation (doens't work with nested serializedObjects)
                t_SourcePropertyValue = a_Property.serializedObject.FindProperty(a_CondFAtt.ConditionalSourceField);
            }


            if (t_SourcePropertyValue != null)
            {
                t_Enabled = CheckPropertyType(t_SourcePropertyValue);
                if (a_CondFAtt.InverseCondition1) t_Enabled = !t_Enabled;
            }
            else
            {
                //Debug.LogWarning("Attempting to use a ConditionalHideAttribute but no matching SourcePropertyValue found in object: " + condHAtt.ConditionalSourceField);
            }

            //handle secondary property
            SerializedProperty t_SourcePropertyValue2 = null;
            if (!a_Property.isArray)
            {
                string propertyPath = a_Property.propertyPath; //returns the property path of the property we want to apply the attribute to
                string conditionPath = propertyPath.Replace(a_Property.name, a_CondFAtt.ConditionalSourceField2); //changes the path to the conditionalsource property path
                t_SourcePropertyValue2 = a_Property.serializedObject.FindProperty(conditionPath);

                //if the find failed->fall back to the old system
                if (t_SourcePropertyValue2 == null)
                {
                    //original implementation (doens't work with nested serializedObjects)
                    t_SourcePropertyValue2 = a_Property.serializedObject.FindProperty(a_CondFAtt.ConditionalSourceField2);
                }
            }
            else
            {
                // original implementation(doens't work with nested serializedObjects) 
                t_SourcePropertyValue2 = a_Property.serializedObject.FindProperty(a_CondFAtt.ConditionalSourceField2);
            }

            //Combine the results
            if (t_SourcePropertyValue2 != null)
            {
                bool prop2Enabled = CheckPropertyType(t_SourcePropertyValue2);
                if (a_CondFAtt.InverseCondition2) prop2Enabled = !prop2Enabled;

                if (a_CondFAtt.UseOrLogic)
                    t_Enabled = t_Enabled || prop2Enabled;
                else
                    t_Enabled = t_Enabled && prop2Enabled;
            }
            else
            {
                //Debug.LogWarning("Attempting to use a ConditionalHideAttribute but no matching SourcePropertyValue found in object: " + condHAtt.ConditionalSourceField);
            }

            //Handle the unlimited property array
            string[] t_ConditionalSourceFieldArray = a_CondFAtt.ConditionalSourceFields;
            bool[] t_ConditionalSourceFieldInverseArray = a_CondFAtt.ConditionalSourceFieldInverseBools;
            for (int index = 0; index < t_ConditionalSourceFieldArray.Length; ++index)
            {
                SerializedProperty t_SourcePropertyValueFromArray = null;
                if (!a_Property.isArray)
                {
                    string t_PropertyPath = a_Property.propertyPath; //returns the property path of the property we want to apply the attribute to
                    string t_ConditionPath = t_PropertyPath.Replace(a_Property.name, t_ConditionalSourceFieldArray[index]); //changes the path to the conditionalsource property path
                    t_SourcePropertyValueFromArray = a_Property.serializedObject.FindProperty(t_ConditionPath);

                    //if the find failed->fall back to the old system
                    if (t_SourcePropertyValueFromArray == null)
                    {
                        //original implementation (doens't work with nested serializedObjects)
                        t_SourcePropertyValueFromArray = a_Property.serializedObject.FindProperty(t_ConditionalSourceFieldArray[index]);
                    }
                }
                else
                {
                    // original implementation(doens't work with nested serializedObjects) 
                    t_SourcePropertyValueFromArray = a_Property.serializedObject.FindProperty(t_ConditionalSourceFieldArray[index]);
                }

                //Combine the results
                if (t_SourcePropertyValueFromArray != null)
                {
                    bool propertyEnabled = CheckPropertyType(t_SourcePropertyValueFromArray);
                    if (t_ConditionalSourceFieldInverseArray.Length >= (index + 1) && t_ConditionalSourceFieldInverseArray[index]) propertyEnabled = !propertyEnabled;

                    if (a_CondFAtt.UseOrLogic)
                        t_Enabled = t_Enabled || propertyEnabled;
                    else
                        t_Enabled = t_Enabled && propertyEnabled;
                }
                else
                {
                    //Debug.LogWarning("Attempting to use a ConditionalHideAttribute but no matching SourcePropertyValue found in object: " + condHAtt.ConditionalSourceField);
                }
            }


            //wrap it all up
            if (a_CondFAtt.Inverse) t_Enabled = !t_Enabled;

            return t_Enabled;
        }

        private bool CheckPropertyType(SerializedProperty a_SourcePropertyValue)
        {
            //Note: add others for custom handling if desired
            switch (a_SourcePropertyValue.propertyType)
            {
                case SerializedPropertyType.Boolean:
                    return a_SourcePropertyValue.boolValue;
                case SerializedPropertyType.ObjectReference:
                    return a_SourcePropertyValue.objectReferenceValue != null;
                default:
                    Debug.LogError("Data type of the property used for conditional hiding [" + a_SourcePropertyValue.propertyType + "] is currently not supported");
                    return true;
            }
        }
    }
}