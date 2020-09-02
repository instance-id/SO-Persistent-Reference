// ----------------------------------------------------------------------------
// -- Project : https://github.com/instance-id/SO-Persistent-Reference --------
// -- instance.id 2020 | http://github.com/instance-id | http://instance.id ---
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using instance.id.OdinSerializer;
using instance.id.SOReference.Utils;
using NaughtyAttributes;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace instance.id.SOReference
{
    [ExecuteInEditMode]
    [Serializable]
    public class ReferenceManager : MonoBehaviour, IExposedPropertyTable // @formatter:off
    {
        // -- referenceScriptableObjects (located automatically) are a list of the ScriptableObject
        // -- containers which hold the scene objects in which you want to maintain a persistent reference
        /*[HeaderLarge("Reference Manager", order = -1)]*/ 
        // [ReadOnly]
        // [ResizableTextArea] 
        // public string info;
        [InfoBox("The Reference Manager maintains the references between the Scene Objects and the ScriptableObject containers")]
        [HeaderLarge("Reference Manager", order = -1)]
        [HorizontalLine(1f,EColor.White)]
        [ReadOnly] public int referenceScriptableObjectsTotal;
        [Label("Reference Manager: Referenced ScriptableObjects")]
        [Foldout("Referenced ScriptableObjects")] [ReorderableList]
        public List<ReferenceScriptableObject> referenceScriptableObjects = new List<ReferenceScriptableObject>();
        
        // -- These containers make it possible to hold/bind the scene objects to the ScriptableObject  
        
        [ReadOnly] public int listReferenceTotal;
        [Foldout("Referenced Scene Objects")] [ReorderableList] 
        public List<Object> listReference = new List<Object>(); // @formatter:on

        [ReadOnly] public int listPropertyNameTotal;
        [Foldout("Referenced Object Names")] [ReorderableList]
        public List<PropertyName> listPropertyName = new List<PropertyName>();
        
        // ------------------------------------------------------------------- Interface Implements
        // -- Interface Implements ----------------------------------------------------------------

        #region IExposedPropertyTable Interface Implements

        public void ClearReferenceValue(PropertyName id)
        {
            int index = listPropertyName.IndexOf(id);
            if (index != -1)
            {
                listReference.RemoveAt(index);
                listPropertyName.RemoveAt(index);
            }
        }

        public Object GetReferenceValue(PropertyName id, out bool idValid)
        {
            int index = listPropertyName.IndexOf(id);
            if (index != -1)
            {
                idValid = true;
                return listReference[index];
            }

            idValid = false;
            return null;
        }

        public void SetReferenceValue(PropertyName id, Object value)
        {
            int index = listPropertyName.IndexOf(id);
            if (index != -1)
            {
                listReference[index] = value;
            }
            else
            {
                listPropertyName.Add(id);
                listReference.Add(value);
            }

            listReferenceTotal = listReference.Count;
            listPropertyNameTotal = listPropertyName.Count;
        }

        #endregion

        // ------------------------------------------------------------------ GetScriptableObjects
        // -- GetScriptableObjects ---------------------------------------------------------------
        public void GetScriptableObjects(bool getObjects = false)
        {
            if (!getObjects)
                if (referenceScriptableObjects.Count != 0)
                    return;
#if UNITY_EDITOR
            referenceScriptableObjects = AssetDatabase.FindAssets("t:ReferenceScriptableObject")
                .Select(guid => AssetDatabase.LoadAssetAtPath<ReferenceScriptableObject>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToList();
#endif
        }

        public void DataAssignments()
        {
            referenceScriptableObjectsTotal = referenceScriptableObjects.Count;
            listReferenceTotal = listReference.Count;
            listPropertyNameTotal = listPropertyName.Count;
        }

        // ---------------------------------------------------------------- Monobehaviour Callbacks
        // --- Monobehaviour Callbacks ------------------------------------------------------------ 

        #region Monobehaviour Callbacks

        private void Awake()
        {
            GetScriptableObjects();
            DataAssignments();
        }

        private void OnEnable()
        {
            GetScriptableObjects();
            DataAssignments();
        }

        private void OnValidate()
        {
            GetScriptableObjects();
            DataAssignments();
        }

        #endregion
    }
}