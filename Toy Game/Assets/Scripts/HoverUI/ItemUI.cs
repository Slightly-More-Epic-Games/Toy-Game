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
            if (item == null) return description;

            string text;
            if (item.healthCost == -1 && item.imaginationCost == -1) {
                text = "Passive:\n";
            } else {
                text = item.refresh switch {
                    Item.Refresh.EventFinished => "On your Turn",
                    Item.Refresh.TurnEnd => "Once per Turn",
                    Item.Refresh.EncounterEnd => "Once per Encounter",
                    Item.Refresh.Never => "Once per Game",
                    _ => ""
                };
                string split = " use for ";

                if (item.imaginationCost > 0) {
                    text += split+item.imaginationCost+" Imagination";
                    split = " and ";
                }
                if (item.healthCost > 0) {
                    text += split+item.healthCost+" Health";
                }
                text += ":\n";
            }
            text += description;

            return text;
        }
    }
}