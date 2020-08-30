// ----------------------------------------------------------------------------
// -- Project : https://github.com/instance-id/SO-Persistent-Reference --------
// -- instance.id 2020 | http://github.com/instance-id | http://instance.id ---
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using instance.id.SOReference.Utils;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace instance.id.SOReference
{
    [Serializable]
    [ExecuteInEditMode]
    public class ObjectHandler : MonoBehaviour
    {
        [Label("Enable debug messages")] public bool debug;
        [HideInInspector, SerializeField] private ReferenceManager referenceManager;
        [HideInInspector] public List<ReferenceScriptableObject> referenceScriptableObject;
        [ReorderableList, Expandable] public List<Object> objectReferenceList = new List<Object>();

        [Foldout("Data Paths"), ShowIf(EConditionOperator.Or, "debug")] [ReadOnly]
        public string dataPath;

        [Foldout("Data Paths"), ShowIf(EConditionOperator.Or, "debug")] [ReadOnly]
        public string pathString;

        [Foldout("Data Paths"), ShowIf(EConditionOperator.Or, "debug")] [ReadOnly]
        public string savePath;

        [Foldout("Data Paths"), ShowIf(EConditionOperator.Or, "debug")] [ReadOnly]
        public string dataPathJson;

        [InfoBox("Select a type to automatically locate and create references for that type or manually add Scene Objects to the Object Reference List")]
        [EnumFlags]
        [Label("Select data type to create references ")]
        public ObjectType objectType = ObjectType.TypeData1;

        private bool refSearch;

        [Button("Bind Object", EButtonEnableMode.Editor)]
        public void AssignObject() // @formatter:off
        {
            if (objectReferenceList.Count > 0 && objectType != ObjectType.None) { refSearch = false;  objectType = ObjectType.None; }
            if (objectReferenceList.Count == 0 && objectType == ObjectType.None) // @formatter:on
            {
                Debug.LogWarning("You must select a type to search for matching objects or place them manually into Object Reference List");
                return;
            }

            if (objectType == ObjectType.All)
            {
                refSearch = true;
                referenceScriptableObject = referenceManager.referenceScriptableObjects;
            }
            else
            {
                refSearch = true;
                referenceScriptableObject = referenceManager.referenceScriptableObjects
                    .Where(x => x.objectType.ToString() == objectType.ToString("F"))
                    .ToList();
                if (debug) Debug.Log($"Matching Reference ScriptableObjects found for Types: {referenceScriptableObject.Count}");
            } // @formatter:off

            if (referenceScriptableObject.Count is 0){ Debug.LogError("Could not locate matching Object => Container types"); return;} // @formatter:on

            if (refSearch)
            {
                var enumFlags = objectType.ToString("F");
                if (debug) Debug.Log($"Found Flags: {enumFlags}");

                var selectedTypes = new List<string>(enumFlags
                    .Split(',')
                    .Select(s => s
                        .Replace(" ", ""))); // @formatter:off
                
                if (selectedTypes.Count == 0) { Debug.Log("Could not parse types "); return; } // @formatter:on

                if (debug)
                {
                    var typeString = selectedTypes.Aggregate("", (current, type) => current + $"{type} ");
                    Debug.Log($"Found Types: {typeString}");
                }

                for (int i = 0; i < selectedTypes.Count; i++)
                {
                    Debug.Log($"Getting Type: {selectedTypes[i]}");

                    var objType = GetEnumType(selectedTypes[i]);
                    if (debug) Debug.Log($"Selected Type: {selectedTypes[i]} Found Type: {objType}");

                    var goList = FindObjectsOfType(objType).ToList();
                    if (goList.Count == 0)
                    {
                        Debug.LogError("No Objects found");
                        return;
                    }

                    for (int j = 0; j < goList.Count; j++)
                    {
                        TypeDataBase go = goList[j] as TypeDataBase;
                        if (debug) Debug.Log($"Object Name: {goList[j].name} : {goList[j].GetType()}");
                        if (!(go is null)) objectReferenceList.Add(go.gameObject);
                    }
                }
            }
            else
            {
                var defaultHolder = FindObjectOfType<DefaultObjectHolder>();
                var objTransforms = defaultHolder.GetComponentsInChildren<Transform>();
                objectReferenceList = objTransforms.Select(x => x.gameObject as Object).ToList();
            }

            for (int r = 0; r < referenceScriptableObject.Count; r++)
            {
                var layerObjectList = referenceScriptableObject[r].exposedReferenceList;
                for (int i = 0; i < objectReferenceList.Count; i++)
                {
                    var refObject = new ExposedReferenceObject
                    {
                        reference = new ExposedReference<GameObject>
                        {
                            defaultValue = objectReferenceList[i],
                            exposedName = objectReferenceList[i].name
                        }
                    };
                    layerObjectList.Add(refObject);
                }

                for (int i = 0; i < layerObjectList.Count; i++)
                {
                    referenceManager.SetReferenceValue(layerObjectList[i].reference.exposedName, layerObjectList[i].reference.defaultValue);
                }

                SaveAssetData(referenceScriptableObject[r]);
            }


            Debug.Log($"✓ Setting Object References for types: [{objectType:F}] Completed!");

            referenceScriptableObject.Clear();
            objectReferenceList.Clear();
        }

        public Type GetEnumType(string objType)
        {
            return Type.GetType($"instance.id.SOReference.{objType}");
        }

        [Button("Clear Objects", EButtonEnableMode.Editor)]
        public void ClearObjects()
        {
            referenceManager.listReference?.Clear();
            referenceManager.listPropertyName?.Clear();
            for (int i = 0; i < referenceManager.referenceScriptableObjects?.Count; i++)
            {
                referenceManager.referenceScriptableObjects?[i].exposedReferenceList.Clear();
            }

            objectReferenceList.Clear();
        }

        void FindNeededReferences()
        {
            if (referenceManager == null) referenceManager = FindObjectOfType<ReferenceManager>();
            dataPath = Application.dataPath;
            pathString = dataPath.Substring(0, dataPath.Length - "Assets".Length);
            savePath = $"{pathString}/ObjectReferenceData/";
            PathTools.CreateIfNotExists(savePath);
        }

        [Button("Save Data to Json")]
        public void SaveLayerDatatoJson()
        {
            dataPathJson = Path.Combine(savePath, $"{referenceManager.name}.json");
            SaveToJson(dataPathJson, referenceManager);

            for (int i = 0; i < referenceManager.referenceScriptableObjects.Count; i++)
            {
                dataPathJson = Path.Combine(savePath, $"{referenceManager.referenceScriptableObjects[i].name}.json");
                SaveToJson(dataPathJson, referenceManager.referenceScriptableObjects[i]);
            }
        }

        private void SaveToJson(string path, object data)
        {
            File.WriteAllText(path, EditorJsonUtility.ToJson(data));
        }

        public ContainerType containerType;

        [Button("Test Button", EButtonEnableMode.Editor)]
        public void TestButton()
        {
            var containerString = containerType.ToString();
            Debug.Log($"Container Type: {containerType.ToString()}");
            var enumFlags = objectType.ToString("F");

            Assert.AreEqual(containerString, enumFlags);

            var selectedTypes = new List<string>(enumFlags
                .Split(',')
                .Select(s => s
                    .Replace(" ", "")));

            foreach (var types in selectedTypes)
            {
                Debug.Log(types);
            }

            for (int i = 0; i < selectedTypes.Count; i++)
            {
                Debug.Log($"Getting Type: {selectedTypes[i]}");

                var objType = GetEnumType(selectedTypes[i]);
                if (debug) Debug.Log($"Selected Type: {selectedTypes[i]} Found Type: {objType}");

                var goList = FindObjectsOfType(objType).ToList();
                if (goList.Count == 0)
                {
                    Debug.LogError("No Objects found");
                    return;
                }

                foreach (var items in goList)
                {
                    Debug.Log($"Type: {objType.Name} Objects: {items.name}");
                }

                if (debug) Debug.Log($"Data Types found: {goList.Count}");
            }
        }

        #region Events and Callbacks

        void Awake()
        {
            FindNeededReferences();
        }

        private void OnEnable()
        {
            FindNeededReferences();
        }

        void SaveAssetData(Object obj)
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(obj);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }

        #endregion
    }
}