using System.Collections;
using System.Collections.Generic;
using Items;
using UnityEngine;

namespace HoverUI {
    [System.Serializable]
    public class TriggerUI : UIInfo
    {
        // uiinfo isnt *just* used by hoverableUI, so by having a public Sprite the same object can also control how the object appears
        // this means the visual properties of an object are kept nice and seperate from the behind the scenes code part
        public string name;
        public string description;
        public Sprite icon;

        public override string GetName() {
            return name;
        }

        public override string GetDescription() {
            return description;
        }
    }
}