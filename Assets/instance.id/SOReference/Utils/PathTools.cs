using System;
using System.IO;
using UnityEngine;

namespace instance.id.SOReference.Utils
{
    public static class PathTools
    {
        public static void CreateIfNotExists(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"SubStream failed to create data folder: {path} : {ex}");
            }
        }
    }
}