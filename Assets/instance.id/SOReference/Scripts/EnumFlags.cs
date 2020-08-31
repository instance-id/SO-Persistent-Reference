// ----------------------------------------------------------------------------
// -- Project : https://github.com/instance-id/SO-Persistent-Reference --------
// -- instance.id 2020 | http://github.com/instance-id | http://instance.id ---
// ----------------------------------------------------------------------------

using NaughtyAttributes;
using UnityEngine;

namespace instance.id.SOReference
{
    public enum ObjectType
    {
        None = 0,
        TypeData1 = 1 << 0,
        TypeData2 = 1 << 1,
        TypeData3 = 1 << 2,
        TypeData4 = 1 << 3,
        TypeData5 = 1 << 4,
        AllTypeData = TypeData1 | TypeData2 | TypeData3 | TypeData4 | TypeData5
    }
    
    public enum ObjectSearchType
    {
        None = 0,
        NoType,
        TypeData1,
        TypeData2,
        TypeData3,
        TypeData4,
        TypeData5,
        AllTypeData = TypeData1 | TypeData2 | TypeData3 | TypeData4 | TypeData5
    }

    public enum ContainerType
    {
        None,
        TypeData1,
        TypeData2,
        TypeData3,
        TypeData4,
        TypeData5,
    }

    public class ObjectTypeFlags : MonoBehaviour
    {
        [EnumFlags] public ObjectType flags0;
    }
}