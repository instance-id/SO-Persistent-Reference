using System;
using UnityEditor;
using UnityEngine;

namespace instance.id.SOReference.Editor
{
    // // [CustomEditor(typeof(ObjectHandler))]
    // public class ObjectHandlerEditor : UnityEditor.Editor
    // {
    //     private void OnEnable()
    //     {
    //     }
    //
    //
    //     public override void OnInspectorGUI()
    //     {
    //         serializedObject.Update();
    //         ObjectHandler target = (ObjectHandler)this.target;
    //         var soList = target.referenceManager.referenceScriptableObjects;
    //         var refSoEnabler = target.refSOSelector;
    //         var genericMenu = new GenericMenu();
    //         for (int i = 0; i < soList.Count; i++)
    //         {
    //             var itemName = soList[i].name;
    //             genericMenu.AddItem(new GUIContent(itemName), false, Callback, $"Test_{i.ToString()}");
    //         }
    //         
    //         
    //         serializedObject.ApplyModifiedProperties();
    //         base.OnInspectorGUI();
    //     }
    //     
    //     void Callback(object obj)
    //     {
    //         Debug.Log("Selected: " + obj);
    //     }
    // }
}