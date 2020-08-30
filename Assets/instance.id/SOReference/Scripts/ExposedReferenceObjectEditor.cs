// ----------------------------------------------------------------------------
// -- Project : https://github.com/instance-id/SO-Persistent-Reference --------
// -- instance.id 2020 | http://github.com/instance-id | http://instance.id ---
// ----------------------------------------------------------------------------

using UnityEngine;
using UnityEditor;

namespace instance.id.SOReference
{
    [CustomPropertyDrawer(typeof(ExposedReferenceObject))]
    public class ExposedReferenceObjectEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var assetSo = new SerializedObject(property.serializedObject.targetObject, GameObject.FindObjectOfType<ReferenceManager>());

            var reference = assetSo.FindProperty(property.propertyPath).FindPropertyRelative("reference");
            EditorGUI.PropertyField(position, reference, label, true);
            assetSo.ApplyModifiedProperties();
        }
    }
}