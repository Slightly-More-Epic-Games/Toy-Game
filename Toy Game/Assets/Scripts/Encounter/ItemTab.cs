using System.Collections;
using System.Collections.Generic;
using Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using HoverUI;

namespace Encounter {
    public class ItemTab : MonoBehaviour
    {
        [SerializeField] private HoverableUI buttonPrefab;
        private HoverableUI[] items;

        [SerializeField] private Button endTurn;
        [SerializeField] private TextMeshProUGUI imagination;

        public void Init(Creature owner, PlayerController playerController) {
            items = new HoverableUI[owner.items.Count];
            for (int i = 0; i < items.Length; i++) {
                HoverableUI button = Instantiate(buttonPrefab, transform);
                int i2 = new int();
                i2 = i;
                button.onClick.AddListener(delegate {playerController.SelectItem(i2);});
                ItemUI itemUI = owner.items[i].GetItemUI();
                button.SetInfo(itemUI);
                button.image.sprite = itemUI.icon;
                items[i] = button;
            }

            endTurn.onClick.AddListener(playerController.EndTurn);
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
            imagination.text = owner.imagination.ToString();
        }

        public void UpdateItemIcon(Creature owner, int index) {
            HoverableUI hoverableUI = items[index];
            ItemSlot item = owner.items[index];
            hoverableUI.interactable = item.CanUse(owner);
        }
    }
}