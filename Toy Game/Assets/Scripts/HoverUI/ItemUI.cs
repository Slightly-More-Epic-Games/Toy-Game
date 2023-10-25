using System.Collections;
using System.Collections.Generic;
using Items;
using UnityEngine;

namespace HoverUI {
    [System.Serializable]
    public class ItemUI : UIInfo
    {
        public string name;
        public string description;
        public Sprite icon;
        private Item item;

        public void SetItem(Item item) {
            this.item = item;
        }

        public override string GetName() {
            return name;
        }

        public override string GetDescription() {
            string text = description;
            if (item == null) return text;
            string split = "\n";
            if (item.imaginationCost > 0) {
                text += split+item.imaginationCost+" Imagination";
                split = " - ";
            }
            if (item.healthCost > 0) {
                text += split+item.healthCost+" Health";
            }
            return text;
        }
    }
}