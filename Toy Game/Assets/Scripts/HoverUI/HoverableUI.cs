using System.Collections;
using System.Collections.Generic;
using Encounter;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HoverUI {
    public class HoverableUI : Button
    {
        private UIInfo uIInfo;
        [SerializeField] private Vector3 offset;

        public override void OnPointerEnter(PointerEventData eventData) {
            base.OnPointerEnter(eventData);
            Game.instance.hoverInfo.HoverEnter(uIInfo, transform, offset);
        }

        public override void OnPointerExit(PointerEventData eventData) {
            base.OnPointerExit(eventData);
            Game.instance.hoverInfo.HoverExit();
        }

        public void SetInfo(UIInfo uIInfo) {
            this.uIInfo = uIInfo;
        }

        public void FlipOffset() {
            offset.y *= -1;
        }
    }
}