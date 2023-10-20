using System.Collections;
using System.Collections.Generic;
using Items;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Items {
    public class HoverableUI : Button
    {
        public UIInfo uIInfo;

        public override void OnPointerEnter(PointerEventData eventData) {
            base.OnPointerEnter(eventData);
        }
    }
}