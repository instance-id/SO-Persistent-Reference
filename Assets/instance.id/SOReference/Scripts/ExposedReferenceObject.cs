// ----------------------------------------------------------------------------
// -- Project : https://github.com/instance-id/SO-Persistent-Reference --------
// -- instance.id 2020 | http://github.com/instance-id | http://instance.id ---
// ----------------------------------------------------------------------------

using UnityEngine;

//Helper class for drawing the exposed reference.
//Exposed references are structs so they cant be expanded on.

[System.Serializable]
public sealed class ExposedReferenceObject
{
    public ExposedReference<GameObject> reference;
}
