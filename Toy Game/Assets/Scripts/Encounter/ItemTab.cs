using System.Collections;
using System.Collections.Generic;
using Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using HoverUI;
using System.Linq;

namespace Encounter {
    public class ItemTab : MonoBehaviour
    {
        [SerializeField] private HoverableUI buttonPrefab;
        private HoverableUI[] items;

        [SerializeField] private Button endTurn;
        [SerializeField] private TextMeshProUGUI imagination;

        [SerializeField] private float flipDuration;
        [SerializeField] private Sprite[] flipSprites;
        private float flipTarget;
        private float currentFlip;

        [SerializeField] private RectTransform selectedMarker;
        [SerializeField] private Image selectedMarkerImage;
        [SerializeField] private SpriteAnimation selectedAnimation;

        private void Update() {
            //update end turn button, since it has a fun little animation
            currentFlip = Mathf.MoveTowards(currentFlip, flipTarget, Time.deltaTime/flipDuration);
            endTurn.image.sprite = flipSprites[Mathf.RoundToInt(currentFlip*(flipSprites.Length-1))];

            selectedMarkerImage.sprite = selectedAnimation.GetSprite();
        }

        public void SetFlipTarget(float target) {
            flipTarget = target;
        }

        public void Init(Creature owner, PlayerController playerController) {
            // set up the buttons for selecting items
            items = new HoverableUI[owner.items.Count];
            for (int i = 0; i < items.Length; i++) {
                HoverableUI button = Instantiate(buttonPrefab, transform);
                int i2 = new int();
                i2 = i;
                ItemSlot itemSlot = owner.items[i];
                // passive items (eg Teddy Bear) cant be used, but graying them out would be weird
                // disabling the button with button.enabled = false would make hover info not work
                if (itemSlot.IsPassive()) {
                    button.transition = Selectable.Transition.None;
                } else {
                    button.onClick.AddListener(delegate {playerController.SelectItem(i2);});
                }
                ItemUI itemUI = itemSlot.GetItemUI();
                button.SetInfo(itemUI);
                button.image.sprite = itemUI.icon;
                items[i] = button;
            }

            endTurn.onClick.AddListener(playerController.EndTurn);
        }

        public void SetInteractable(Creature owner, bool interactable) {
            // i dont think this is really used properly - its supposed to be to disable the items when its not your turn
            if (!interactable) {
                foreach (HoverableUI hoverableUI in items) {
                    hoverableUI.interactable = false;
                }
            } else {
                UpdateAllIcons(owner);
            }
        }

        public void UpdateAllIcons(Creature owner) {
            // updates if items are usable and the current imagination text
            for (int i = 0; i < items.Length; i++) {
                UpdateItemIcon(owner, i);
            }
            imagination.text = owner.imagination.ToString();
        }

        public void UpdateItemIcon(Creature owner, int index) {
            HoverableUI hoverableUI = items[index];
            ItemSlot item = owner.items[index];
            hoverableUI.interactable = item.CanUse(owner) || item.IsPassive();
        }

        public void Select(int index) {
            // move selection marker
            if (index == -1) {
                selectedMarker.gameObject.SetActive(false);
                return;
            }
            selectedMarker.gameObject.SetActive(true);
            selectedMarker.SetParent(items[index].transform);
            selectedMarker.anchoredPosition = Vector2.zero;
        }
    }
}