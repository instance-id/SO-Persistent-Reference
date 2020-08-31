using System.Collections.Generic;
using System.IO;
using instance.id.OdinSerializer;
using UnityEditor;
using UnityEngine;

namespace instance.id.SOReference.Utils
{
    struct SaveData
    {
        public byte[] referenceScriptableObjects;
        public byte[] listReference;
        public byte[] listPropertyName;
    }

    public class FileUtilities
    {
        private readonly ObjectHandler objectHandler;

        public FileUtilities(ObjectHandler objHandler)
        {
            objectHandler = objHandler;
        }
        
        // ----------------------------------------------- SaveObjectDatatoFile
        // -- SaveObjectDatatoFile --------------------------------------------
        public void SaveObjectDatatoFile()
        {
            if (objectHandler.pathExists)
            {
                var savePath = Path.Combine(
                    objectHandler.savePath,
                    $"{objectHandler.referenceManager.name}.json"
                );

                var savePathDat = Path.Combine(
                    objectHandler.savePath,
                    $"{objectHandler.referenceManager.name}.dat"
                );

                SaveToFile(savePath, objectHandler.referenceManager);

                // -- Attempt to save with OdinSerializer ---------------------
                // byte[] bytes = UnitySerializationUtility.SerializeUnityObject(objectHandler.referenceManager, DataFormat.Binary);
                byte[] referenceScriptableObjectsBytes = SerializationUtility.SerializeValue(objectHandler.referenceManager.referenceScriptableObjects, DataFormat.Binary);
                byte[] listReferenceBytes = SerializationUtility.SerializeValue(objectHandler.referenceManager.listReference, DataFormat.Binary);
                byte[] listPropertyNameBytes = SerializationUtility.SerializeValue(objectHandler.referenceManager.listPropertyName, DataFormat.Binary);

                var saveData = new SaveData
                {
                    referenceScriptableObjects = referenceScriptableObjectsBytes,
                    listReference = listReferenceBytes,
                    listPropertyName = listPropertyNameBytes
                };

                byte[] saveDatabytes = SerializationUtility.SerializeValue(saveData, DataFormat.Binary);
                File.WriteAllBytes(savePathDat, saveDatabytes);

                for (int i = 0; i < objectHandler.referenceManager.referenceScriptableObjects.Count; i++)
                {
                    var path = Path.Combine(
                        objectHandler.savePath,
                        $"{objectHandler.referenceManager.referenceScriptableObjects[i].name}.json"
                    );

                    SaveToFile(path, objectHandler.referenceManager.referenceScriptableObjects[i]);
                }
            }
            else
            {
                Debug.LogWarning($"Json Save Path: {objectHandler.savePath} does not exist. Creating...");
                objectHandler.pathExists = PathTools.CreateIfNotExists(objectHandler.savePath);
                if (objectHandler.pathExists)
                {
                    Debug.Log($" ✓ Path: {objectHandler.savePath} Created! Please save again.");
                    return;
                }
            }

            Debug.Log($" ✓ Files Created!");
        }

        // byte[] Serialize(object obj, out List<UnityEngine.Object> references)
        // {
        //     var resolver = new IndexResolver();
        //     var context = new SerializationContext
        //     {
        //         IndexReferenceResolver = resolver
        //     };
        //     var bytes = SerializationUtility.SerializeValue(obj, DataFormat.Binary, context);
        //     references = resolver.ReferenceList;
        //
        //     return bytes;
        // }

        private void SaveToFile(string path, object data)
        {
            File.WriteAllText(path, EditorJsonUtility.ToJson(data));
            if (File.Exists(path) && objectHandler.debug) Debug.Log($" ✓ File: {path} Created!");
        }

        // --------------------------------------------- LoadObjectDataFromFile
        // -- LoadObjectDataFromFile ------------------------------------------
        public void LoadObjectDataFromFile()
        {
            if (objectHandler.pathExists)
            {
                var loadPath = Path.Combine(
                    objectHandler.savePath,
                    $"{objectHandler.referenceManager.name}.json"
                );

                var loadPathDat = Path.Combine(
                    objectHandler.savePath,
                    $"{objectHandler.referenceManager.name}.dat"
                );

                byte[] loadDatabytes = File.ReadAllBytes(loadPathDat);
                
                var loadData = SerializationUtility.DeserializeValue<SaveData>(loadDatabytes, DataFormat.Binary);
                
                objectHandler.referenceManager.referenceScriptableObjects = SerializationUtility.DeserializeValue<List<ReferenceScriptableObject>>(loadData.referenceScriptableObjects, DataFormat.Binary);
                objectHandler.referenceManager.listReference = SerializationUtility.DeserializeValue<List<Object>>(loadData.referenceScriptableObjects, DataFormat.Binary);
                objectHandler.referenceManager.listPropertyName = SerializationUtility.DeserializeValue<List<PropertyName>>(loadData.referenceScriptableObjects, DataFormat.Binary);
     
                // byte[] bytes = File.ReadAllBytes(loadPathDat);
                // objectHandler.referenceManager = SerializationUtility.DeserializeValue<ReferenceManager>(bytes, DataFormat.Binary);
                Debug.Log(" ✓ Data possibly Loaded?");

                // --- Original tests ----------
                // -----------------------------
                /*var referenceManagerFile = LoadFromJson(
                    loadPath,
                    objectHandler.referenceManager
                ) as ReferenceManager;

                if (referenceManagerFile == null)
                {
                    Debug.LogWarning($"Json Save Path: {objectHandler.savePath} does not exist. Creating...");
                    return;
                }

                if (objectHandler.debug) Debug.Log($"Load referenceManagerFile: {referenceManagerFile.name}");

                var referenceSoList = new List<ReferenceScriptableObject>();
                for (int i = 0; i < objectHandler.referenceManager.referenceScriptableObjects.Count; i++)
                {
                    var dataPath = Path.Combine(objectHandler.savePath, $"{objectHandler.referenceManager.referenceScriptableObjects[i].name}.json");
                    referenceSoList.Add(LoadFromJson(dataPath, objectHandler.referenceManager) as ReferenceScriptableObject);
                }

                if (referenceSoList.Count is 0)
                {
                    Debug.LogWarning($"No referenceScriptableObjects could be found");
                    return;
                }

                foreach (var referenceSO in referenceSoList)
                {
                    if (referenceSO is null) Debug.LogError($"Loaded ScriptableObject reference is null");
                    else Debug.Log($"{referenceSO.name}: {referenceSO.objectType.ToString()}");
                }

                Debug.Log(" ✓ Data Loaded");*/
            }
        }

        private object LoadFromJson(string path, object target)
        {
            if (File.Exists(path))
            {
                EditorJsonUtility.FromJsonOverwrite(File.ReadAllText(path), target);
            }

            return target;
        }
    }
}