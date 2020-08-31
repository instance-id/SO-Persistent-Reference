// ----------------------------------------------------------------------------
// -- Project : https://github.com/instance-id/SO-Persistent-Reference --------
// -- instance.id 2020 | http://github.com/instance-id | http://instance.id ---
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using instance.id.OdinSerializer;
using instance.id.SOReference.Utils;
using JetBrains.Annotations;
using NaughtyAttributes;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace instance.id.SOReference
{
    [ExecuteInEditMode]
    public class ObjectHandler : SerializedMonoBehaviour // @formatter:off
    {
        // ------------------------------------------------------------------ Variables/Collections
        // -- Variables/Collections ---------------------------------------------------------------
       
        // [Label("Select type to search")] [OnValueChanged("OnObjectTypeValueChanged"), EnumFlags]
        // public ObjectType searchObjectType; // @formatter:on

        
        [HeaderLarge("Object Handler", order = -1)]
        [HorizontalLine(1f, EColor.White)]
        [InfoBox("Select a type to automatically locate and create references for that type or manually add Scene Objects to the Object Reference List", order = 1)]
        [Dropdown("GetObjectSelectionValues")]
        [OnValueChanged("OnSearchObjectTypeValueChanged")]
        public string objectSearchType;

        public Dictionary<string, bool> objectSearchSelection = new Dictionary<string, bool>();

#pragma warning disable 414
        private bool showObjectReferenceList = true;
#pragma warning restore 414
        [HideInInspector, SerializeField] public ReferenceManager referenceManager;
        [HideInInspector] public List<ReferenceScriptableObject> referenceScriptableObject;

        [ShowIf(EConditionOperator.Or, "showObjectReferenceList")] [ReorderableList]
        public List<Object> objectReferenceList = new List<Object>(); 
        [SerializeField] public Dictionary<string, List<Object>> objectReferenceTypeDict = new Dictionary<string, List<Object>>();

        private bool refSearch;
        private bool objectListOnly;
        private bool getUntagged; // @formatter:off

        // ------------------------------------------------------ Debug related
        // -- Debug related ---------------------------------------------------
        [Foldout("Data Paths"), ShowIf(EConditionOperator.Or, "debug")] 
        [ReadOnly] public string dataPath;
        [Foldout("Data Paths"), ShowIf(EConditionOperator.Or, "debug")] 
        [ReadOnly] public string pathString;
        [Foldout("Data Paths"), ShowIf(EConditionOperator.Or, "debug")] 
        [ReadOnly] public string savePath;
        [Foldout("Data Paths"), ShowIf(EConditionOperator.Or, "debug")] 
        [HideInInspector] public bool debug; // @formatter:on

        List<string> selectedTypes = new List<string>();

        List<string> refTypeKeys = new List<string>();

        // --------------------------------------------------------------------------- AssignObject
        // Using either predefined "TypeData" components to search and locate objects automatically
        // or manually adding objects to the objectReferenceList, this method binds the objects in 
        // the list to their predefined "Type" (being the various TypeDataBase inheritors) if they
        // are selected in the searchObjectType dropdown or if objects are manually added they will
        // be placed into the default "catchall" ReferenceScriptableObject (TypeData_Default)
        // -- AssignObject ------------------------------------------------------------------------
        [Button("Bind Object", EButtonEnableMode.Editor)]
        public void AssignObject()
        {
            // -- Make list of selected search types ----------------------------------------------
            // -- (Note: I do not mind Linq allocations for editor scripts, only for runtime) -----
            selectedTypes = objectSearchSelection
                .Where(x => x.Value)
                .Select(x => x.Key).ToList(); // @formatter:off

            if (selectedTypes.Count == 0) { Debug.Log("Could not parse types "); return; } 
                
            var typeString = selectedTypes.Aggregate("", (current, type) => current + $"{type} ");
            if (debug)
            {
                Debug.Log($"Found Types: {typeString}");
            }
            
            // -- Check if specific types are selected, or is objects were manually added ---------
            // -- (This was starting to become a mess and I need to come back and sort it out) ----
            if (objectReferenceList.Count > 0 && selectedTypes.Contains("None")) { refSearch = false; objectListOnly = true;} else {refSearch = true;}
            if (objectReferenceList.Count > 0 || selectedTypes.Contains("Untagged")) { getUntagged = true;}
            if (selectedTypes.Count == 1 && selectedTypes.Contains("Untagged")) { refSearch = false; getUntagged = true;}
            
            if (objectReferenceList.Count == 0 && selectedTypes.Contains("None")) // @formatter:on
            {
                Debug.LogWarning("You must select a type to search for matching objects, " +
                                 "toggle 'Get Non-specific Objects', or place them manually into Object Reference List");
                return;
            }

            if (debug) Debug.Log($"objectReferenceList: {objectReferenceList.Count.ToString()} refSearch: {refSearch.ToString()} getUntagged: {getUntagged.ToString()}");

            if (refSearch) // -- getUntagged takes the selected searchObjectType 
            {
                referenceScriptableObject = referenceManager.referenceScriptableObjects
                    .Where(x => selectedTypes.Contains(x.objectType.ToString()))
                    .ToList();
                if (debug) Debug.Log($"Matching Reference ScriptableObjects found for Types: {referenceScriptableObject.Count}"); // @formatter:off

                // -- If no matching Reference ScriptableObjects were found, return error ---------
                if (referenceScriptableObject.Count is 0){ Debug.LogError("Could not locate matching Object => Container types"); return;} // @formatter:on

                for (int i = 0; i < selectedTypes.Count; i++)
                {
                    if (selectedTypes[i].Equals("Untagged")) continue;

                    if (debug) Debug.Log($"Getting Type: {selectedTypes[i]}");

                    var objType = GetEnumType(selectedTypes[i]);
                    if (debug) Debug.Log($"Selected Type: {selectedTypes[i]} Found Type: {objType}");

                    var goList = FindObjectsOfType(objType).ToList();
                    if (goList.Count == 0)
                    {
                        Debug.LogError("No Objects found");
                        return;
                    }

                    var goTypeList = new List<Object>();
                    for (int j = 0; j < goList.Count; j++)
                    {
                        TypeDataBase go = goList[j] as TypeDataBase;
                        if (debug) Debug.Log($"Object Name: {goList[j].name} : {goList[j].GetType()}");
                        // if (!(go is null)) objectReferenceList.Add(go.gameObject);
                        if (!(go is null)) goTypeList.Add(go.gameObject);
                    }

                    objectReferenceTypeDict.Add(selectedTypes[i], goTypeList);
                }
            }

            if (getUntagged)
            {
                var defaultHolder = FindObjectOfType<DefaultObjectHolder>();
                var objTransforms = defaultHolder.GetComponentsInChildren<Transform>(); // @formatter:off
                if(objTransforms == null) {Debug.LogError("Could not locate DefaultObjectHolder Object holder in scene"); return;} // @formatter:on

                var untaggedObjects = new List<Object>();
                if (!objectListOnly)
                {
                    untaggedObjects = objTransforms
                        .Where(x => !x.gameObject.GetComponent<DefaultObjectHolder>())
                        .Select(x => x.gameObject as Object)
                        .ToList(); // @formatter:off
                    if(untaggedObjects.Count == 0) {Debug.LogError("Could not locate DefaultObjectHolder Object holder in scene"); return;} 
                }
                untaggedObjects.AddRange(objectReferenceList);
                objectReferenceTypeDict.Add("None", untaggedObjects); // @formatter:on

                var defaultContainer = referenceManager.referenceScriptableObjects
                    .FirstOrDefault(x => x.objectType.ToString() == "None"); // @formatter:off
                if(defaultContainer == null) {Debug.LogError("Could not locate TypeData_Default Reference ScriptableObject"); return;} 
                
                referenceScriptableObject.Add(defaultContainer); // @formatter:on
                if (debug)
                {
                    Debug.Log($"Added {defaultContainer.name} for Untagged objects");
                    foreach (var untaggedObject in untaggedObjects)
                    {
                        Debug.Log(untaggedObject.name);
                    }
                }
            }

            refTypeKeys = objectReferenceTypeDict.Keys.ToList();
            for (var i = 0; i < refTypeKeys.Count; i++)
            {
                var typeObjects = objectReferenceTypeDict[refTypeKeys[i]];
                if (debug) Debug.Log($"{refTypeKeys[i]} : {typeObjects.Count.ToString()}");

                var refObjectSO = referenceScriptableObject
                    .FirstOrDefault(x => x.objectType.ToString() == refTypeKeys[i]); // @formatter:off
                if (refObjectSO == null) { Debug.LogError($"{refTypeKeys[i]} refObjectSO null"); return; } // @formatter:on

                if (debug) Debug.Log($"{refTypeKeys[i]} : {refObjectSO.name}");
                var refObjectList = refObjectSO.exposedReferenceList;
                for (int j = 0; j < typeObjects.Count; j++)
                {
                    var refObject = new ExposedReferenceObject
                    {
                        reference = new ExposedReference<GameObject>
                        {
                            defaultValue = typeObjects[j],
                            exposedName = typeObjects[j].name
                        }
                    };
                    refObjectList.Add(refObject);
                }

                for (int r = 0; r < refObjectList.Count; r++)
                {
                    referenceManager.SetReferenceValue(refObjectList[r].reference.exposedName, refObjectList[r].reference.defaultValue);
                }

                SaveAssetData(refObjectSO);
            }

            // for (int r = 0; r < referenceScriptableObject.Count; r++)
            // {
            //     var refObjectList = referenceScriptableObject[r].exposedReferenceList;
            //     for (int i = 0; i < objectReferenceList.Count; i++)
            //     {
            //         var refObject = new ExposedReferenceObject
            //         {
            //             reference = new ExposedReference<GameObject>
            //             {
            //                 defaultValue = objectReferenceList[i],
            //                 exposedName = objectReferenceList[i].name
            //             }
            //         };
            //         refObjectList.Add(refObject);
            //     }
            //
            //     for (int i = 0; i < refObjectList.Count; i++)
            //     {
            //         referenceManager.SetReferenceValue(refObjectList[i].reference.exposedName, refObjectList[i].reference.defaultValue);
            //     }
            //
            //     SaveAssetData(referenceScriptableObject[r]);
            // }

            Debug.Log($"✓ Setting Object References for types: [{typeString}] Completed!");

            referenceScriptableObject.Clear();
            objectReferenceList.Clear();
            getUntagged = false;
            refSearch = false;
        }

        // ------------------------------------------------------------------------- Helper Methods
        // -- Helper Methods ----------------------------------------------------------------------

        #region Helper Methods

        // ----------------------------------------------------- Initialization
        // -- Initialization --------------------------------------------------
        void FindNeededReferences()
        {
            if (referenceManager == null) referenceManager = FindObjectOfType<ReferenceManager>();
            dataPath = Application.dataPath;
            pathString = dataPath.Substring(0, dataPath.Length - "Assets".Length);
            savePath = $"{pathString}/ObjectReferenceData/";
            pathExists = PathTools.CreateIfNotExists(savePath);
        }

        [ContextMenu("Refresh Search Selection Options")]
        void PopulateObjectSearch()
        {
            var sObjects = referenceManager.referenceScriptableObjects;
            if (sObjects.Count == 0) referenceManager.GetScriptableObjects(true);

            objectSearchSelection.Clear();
            objectSearchSelection.Add("None", true);
            objectSearchSelection.Add("Untagged", false);

            for (int i = 0; i < sObjects.Count; i++)
            {
                objectSearchSelection.Add(sObjects[i].name, false);
            }
        }

        // ------------------------------------------------------- Enum Related
        // -- Enum Related ----------------------------------------------------
        List<string> GetEnumStringList(string enumFlags)
        {
            return new List<string>(enumFlags
                .Split(',')
                .Select(s => s
                    .Replace(" ", "")));
        }

        public Type GetEnumType(string objType)
        {
            return Type.GetType($"instance.id.SOReference.{objType}");
        }


        // -------------------------------------------------- Interface Related
        // -- Interface Related -----------------------------------------------
        DropdownList<string> objectDropdownList = new DropdownList<string>();

        private DropdownList<string> GetObjectSelectionValues()
        {
            objectDropdownList = new DropdownList<string>();
            objectDropdownList.Add("None", "None");
            objectDropdownList.Add(objectSearchSelection["Untagged"]
                ? "Untagged  ✓"
                : "Untagged", "Untagged");

            for (int i = 0; i < referenceManager.referenceScriptableObjects.Count; i++)
            {
                var rm = referenceManager.referenceScriptableObjects[i];
                if (rm.name == "TypeData_Default") continue;

                objectDropdownList.Add(objectSearchSelection[rm.name]
                    ? $"{rm.name}  ✓"
                    : rm.name, rm.name);
            }

            objectSearchType = objectDropdownList
                .Select(x => x.Key == "None")
                .FirstOrDefault()
                .ToString();

            return objectDropdownList;
        }

        private void OnSearchObjectTypeValueChanged()
        {
            objectSearchSelection["None"] = false;
            var result = objectSearchSelection[objectSearchType];
            objectSearchSelection[objectSearchType] = !result;
            if (objectSearchSelection["None"]) PopulateObjectSearch();

            if (debug) Debug.Log($"{objectSearchType}: Selected: {objectSearchSelection[objectSearchType].ToString()}");
        }

        [ContextMenu("Enable Debug")]
        void EnableDebug(MenuCommand command)
        {
            debug = !debug;
        }

        [Button("Clear All Objects", EButtonEnableMode.Editor)]
        public void ClearObjects()
        {
            if (referenceManager.referenceScriptableObjects == null)
                referenceManager.referenceScriptableObjects = new List<ReferenceScriptableObject>();

            referenceManager.referenceScriptableObjects.Clear();
            referenceManager.GetScriptableObjects(true);

            referenceManager.listReference = new List<Object>();
            referenceManager.listPropertyName = new List<PropertyName>();
            for (int i = 0; i < referenceManager.referenceScriptableObjects.Count; i++)
            {
                referenceManager.referenceScriptableObjects[i].exposedReferenceList = new List<ExposedReferenceObject>();
            }

            objectReferenceList.Clear();
            objectReferenceTypeDict.Clear();
            PopulateObjectSearch();
            referenceManager.DataAssignments();

            Debug.Log("✓ All Data Cleared");
        }

        #endregion

        // ------------------------------------------------------------------------- Save/Load Data
        // -- Save/Load Data ----------------------------------------------------------------------
        #region Save/Load Data

        private FileUtilities fileUtilities;

        // ----------------------------------------------- SaveObjectDatatoFile
        // -- SaveObjectDatatoFile --------------------------------------------
        [HideInInspector] public bool pathExists;
        [UsedImplicitly] private bool enableSerialization;

        [EnableIf(EConditionOperator.Or, "enableSerialization")]
        [Button("Save Data to File")]
        public void SaveObjectDatatoFile()
        {
            if (!enableSerialization) return;
            if (fileUtilities == null) fileUtilities = new FileUtilities(this);
            fileUtilities.SaveObjectDatatoFile();
            enableSerialization = false;
        }

        // --------------------------------------------- LoadObjectDataFromFile
        // -- LoadObjectDataFromFile ------------------------------------------
        [EnableIf(EConditionOperator.Or, "enableSerialization")]
        [Button("Load Data from File")]
        public void LoadObjectDataFromFile()
        {
            if (fileUtilities == null) fileUtilities = new FileUtilities(this);
            fileUtilities.LoadObjectDataFromFile();
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

        // ------------------------------------------------------------------------------ TestItems
        // -- TestItems ---------------------------------------------------------------------------

        #region TestItems

        // [Button("Test Button", EButtonEnableMode.Editor)]
        public void TestButton()
        {
            var selectedTypes = objectSearchSelection
                .Where(x => x.Value)
                .Select(x => x.Key).ToList();

            if (selectedTypes.Count == 0)
            {
                Debug.Log("Could not parse types ");
                return;
            }

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

                foreach (var items in goList)
                {
                    Debug.Log($"Type: {objType.Name} Objects: {items.name}");
                }

                if (debug) Debug.Log($"Data Types found: {goList.Count}");
            }
        }

        #endregion

        // ----------------------------------------------------------------------- Events/Callbacks
        // -- Events/Callbacks --------------------------------------------------------------------

        #region System Events/Callbacks

        void Awake()
        {
            FindNeededReferences();
            PopulateObjectSearch();
        }

        private void OnEnable()
        {
            FindNeededReferences();
            PopulateObjectSearch();
            showObjectReferenceList = true;
        }

        #endregion
    }
}