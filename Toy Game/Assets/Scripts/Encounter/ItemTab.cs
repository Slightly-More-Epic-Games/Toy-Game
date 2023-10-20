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
                items[i] = button;
            }
        }

        public void SetInteractable(bool interactable) {

        }

        public void UpdateAllIcons() {
            for (int i = 0; i < items.Length; i++) {
                UpdateItemIcon(i);
            }
        }

        public void UpdateItemIcon(int index) {

        }
    }
}