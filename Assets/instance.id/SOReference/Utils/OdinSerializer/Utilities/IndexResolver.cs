// using System.Collections.Generic;
// using instance.id.OdinSerializer;
//
// namespace instance.id.SOReference.Utils.OdinSerializer.Utilities
// {
//     public class IndexResolver : IExternalIndexReferenceResolver
//     {
//         public List<UnityEngine.Object> ReferenceList;
//
//         public IndexResolver()
//         {
//             this.ReferenceList = new List<UnityEngine.Object>();
//         }
//
//         public IndexResolver(List<UnityEngine.Object> references)
//         {
//             this.ReferenceList = references;
//         }
//
//         public bool CanReference(object value, out int index)
//         {
//             if (value is UnityEngine.Object)
//             {
//                 index = this.ReferenceList.Count;
//                 this.ReferenceList.Add(value);
//             }
//
//             index = 0;
//             return false;
//         }
//
//         public bool TryResolveReference(int index, out object value)
//         {
//             value = this.referencedUnityObjects[index];
//             return true;
//         }
//         
//         byte[] Serialize(object obj, out List<UnityEngine.Object> references)
//         {
//             var resolver = new IndexResolver();
//             var context = new SerializationContext
//             {
//                 IndexReferenceResolver = resolver,
//             };
//             var bytes = SerializationUtility.SerializeValue(obj, DataFormat.Binary, context);
//             references = resolver.ReferenceList;
//
//             return bytes;
//         }
//         
//         object Deserialize(byte[] bytes, List<UnityEngine.Object> references)
//         {
//             var context = new DeserializationContext()
//             {
//                 IndexReferenceResolver = new IndexResolver(references),
//             };
//             return SerializationUtility.DeserializeValue<object>(bytes, DataFormat.Binary, context);
//         }
//     }
//     
//     
// }