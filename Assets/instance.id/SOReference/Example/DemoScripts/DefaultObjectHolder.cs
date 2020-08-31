// ----------------------------------------------------------------------------
// -- Project : https://github.com/instance-id/SO-Persistent-Reference --------
// -- instance.id 2020 | http://github.com/instance-id | http://instance.id ---
// ----------------------------------------------------------------------------

using NaughtyAttributes;
using UnityEngine;

namespace instance.id.SOReference
{
    // -- This script just designates the container holding "other"
    // -- GameObjects that are not members of the defined type examples
    public class DefaultObjectHolder : MonoBehaviour
    {
        [InfoBox("The DefaultObjectHolder Monobehaviour is simply used to easily find the child 'Untagged' GameObjects")]
        [ReadOnly] public bool dummyBoolToDisplayInfoBoxAbove;
    }
}