// ----------------------------------------------------------------------------
// -- Project : https://github.com/instance-id/SO-Persistent-Reference --------
// -- instance.id 2020 | http://github.com/instance-id | http://instance.id ---
// ----------------------------------------------------------------------------

using System.Collections.Generic;
using instance.id.OdinSerializer;
using UnityEngine;

namespace instance.id.SOReference
{
    [CreateAssetMenu]
    public class ReferenceScriptableObject : SerializedScriptableObject
    {
        // --- ObjectType is used to assign a differentiating "Type" to each Reference ScriptableObject
        // --- which is then matched to the appropriate TypeData Monobehaviour used to locate scene objects
        public ContainerType objectType;
        
        // --- exposedReferenceList is used to store a list of the appropriately typed scene objects
        public List<ExposedReferenceObject> exposedReferenceList = new List<ExposedReferenceObject>();
    }
}
