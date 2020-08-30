// ----------------------------------------------------------------------------
// -- Project : https://github.com/instance-id/SO-Persistent-Reference --------
// -- instance.id 2020 | http://github.com/instance-id | http://instance.id ---
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace instance.id.SOReference
{
    [Serializable]
    [ExecuteInEditMode]
    public class ReferenceManager : MonoBehaviour, IExposedPropertyTable
    {
        public List<ReferenceScriptableObject> referenceScriptableObjects = new List<ReferenceScriptableObject>();
        [Foldout("Referenced Scene Objects")] public List<PropertyName> listPropertyName;

        [Foldout("Referenced Scene Objects")] public List<Object> listReference;
        
        // -------------------------------------------------------------------------- GetSOObjects
        // -- GetSOObjects -----------------------------------------------------------------------
        private void GetScriptableOObjects()
        {
            if (referenceScriptableObjects.Count != 0) return;
#if UNITY_EDITOR
            referenceScriptableObjects = AssetDatabase.FindAssets("t:ReferenceScriptableObject")
                .Select(guid => AssetDatabase.LoadAssetAtPath<ReferenceScriptableObject>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToList();
#endif
        }

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
        }

        #region Monobehaviour callbacks

        
        // ---------------------------------------------------------------------------------------
        // --- Monobehaviour callbacks which locate the Reference ScriptableObjects in the project 
        private void Awake()
        {
            GetScriptableOObjects();
        }

        private void OnEnable()
        {
            GetScriptableOObjects();
        }

        private void OnValidate()
        {
            GetScriptableOObjects();
        }
        #endregion

    }
}