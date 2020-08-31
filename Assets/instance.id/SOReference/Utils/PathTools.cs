// ----------------------------------------------------------------------------
// -- Project : https://github.com/instance-id/SO-Persistent-Reference --------
// -- instance.id 2020 | http://github.com/instance-id | http://instance.id ---
// ----------------------------------------------------------------------------

using System;
using System.IO;
using UnityEngine;

namespace instance.id.SOReference.Utils
{
    public static class PathTools
    {
        public static bool CreateIfNotExists(string path)
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

            return Directory.Exists(path);
        }
    }
}