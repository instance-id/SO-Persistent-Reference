// ----------------------------------------------------------------------------
// -- Project : https://github.com/instance-id/SO-Persistent-Reference --------
// -- instance.id 2020 | http://github.com/instance-id | http://instance.id ---
// ----------------------------------------------------------------------------

using UnityEngine;

namespace instance.id.SOReference.Utils
{
    /// <summary>
    /// HeaderLarge adds a large sized header to an inspector.
    /// </summary>
    public class HeaderLarge : PropertyAttribute
    {
        public string name;
        public string color;

        public HeaderLarge(string name)
        {
            this.name = name;
            this.color = "white";
        }

        public HeaderLarge(string name, string color)
        {
            this.name = name;
            this.color = color;
        }
    }
}