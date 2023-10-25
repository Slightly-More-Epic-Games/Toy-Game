using System.Collections;
using System.Collections.Generic;
using Items;
using UnityEngine;

namespace HoverUI {
    [System.Serializable]
    public class TriggerUI : UIInfo
    {
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