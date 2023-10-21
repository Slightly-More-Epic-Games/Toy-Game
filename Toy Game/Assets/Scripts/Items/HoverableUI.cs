using System.Collections;
using System.Collections.Generic;
using Encounter;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Items {
    public class HoverableUI : Button
    {
        private UIInfo uIInfo;

        public override void OnPointerEnter(PointerEventData eventData) {
            base.OnPointerEnter(eventData);
            Manager.instance.hoverInfo.HoverEnter(uIInfo, transform);
        }

        public override void OnPointerExit(PointerEventData eventData) {
            base.OnPointerExit(eventData);
            Manager.instance.hoverInfo.HoverExit();
        }

        public void SetInfo(UIInfo uIInfo) {
            this.uIInfo = uIInfo;
            image.sprite = uIInfo.icon;
        }
    }
}