using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoverUI {
    [System.Serializable]
    public abstract class UIInfo
    {
        public abstract string GetName();

        public abstract string GetDescription();
    }
}