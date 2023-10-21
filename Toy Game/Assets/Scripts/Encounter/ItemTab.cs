using System.Collections;
using System.Collections.Generic;
using Items;
using UnityEngine;
using UnityEngine.UI;

namespace Encounter {
    public class ItemTab : MonoBehaviour
    {
        [SerializeField] private HoverableUI buttonPrefab;
        private HoverableUI[] items;

        public void Init(Creature owner, PlayerController playerController) {
            items = new HoverableUI[owner.items.Count];
            for (int i = 0; i < items.Length; i++) {
                HoverableUI button = Instantiate(buttonPrefab, transform);
                int i2 = new int();
                i2 = i;
                button.onClick.AddListener(delegate {playerController.SelectItem(i2);});
                button.SetInfo(owner.items[i].ui);
                items[i] = button;
            }
        }

        public void SetInteractable(Creature owner, bool interactable) {
            if (!interactable) {
                foreach (HoverableUI hoverableUI in items) {
                    hoverableUI.interactable = false;
                }
            } else {
                UpdateAllIcons(owner);
            }
        }

        public void UpdateAllIcons(Creature owner) {
            for (int i = 0; i < items.Length; i++) {
                UpdateItemIcon(owner, i);
            }
        }

        public void UpdateItemIcon(Creature owner, int index) {
            HoverableUI hoverableUI = items[index];
            Item item = owner.items[index];
            hoverableUI.interactable = item.healthCost >= 0 && item.imaginationCost >= 0 && item.healthCost <= owner.health && item.imaginationCost <= owner.imagination;
        }
    }
}