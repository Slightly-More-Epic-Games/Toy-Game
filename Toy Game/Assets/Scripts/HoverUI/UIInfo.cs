using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoverUI {
    [System.Serializable]
    public abstract class UIInfo
    {
        // very simple class, but by being abstract it makes it easy for subclasses to add on extra bits to descriptions (eg current health or item costs) on top of any serialized strings
        public abstract string GetName();

        public abstract string GetDescription();
    }
}