using System.Collections;
using System.Collections.Generic;
using Encounter;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HoverUI {
    public class HoverableUI : Button
    {
        // hoverable ui extends unities builtin button, since only one selectable can be on an object and in most cases it also needs to be a button
        // it could have also been done by adding listeners on Start()

        private UIInfo uIInfo;
        [SerializeField] private Vector3 offset;

        public override void OnPointerEnter(PointerEventData eventData) {
            base.OnPointerEnter(eventData);
            Game.instance.hoverInfo.HoverEnter(uIInfo, transform.position, offset, transform);
        }

        public override void OnPointerExit(PointerEventData eventData) {
            base.OnPointerExit(eventData);
            Game.instance.hoverInfo.HoverExit();
        }

        public void SetInfo(UIInfo uIInfo) {
            // a hoverable ui needs to be linked to a ui info or it wont be able to display text
            // by using a uiinfo it obviously means that any classes using hoverable ui have to use a uiinfo
            // this is a good thing - since uiinfos in turn need to be inherited
            // this then forces each object that wants to use a hoverable ui (and in turn hoverinfo) to keep track of a single object reference
            // this *then* means that no matter where eg an item shows up, if its hoverable it will have the exact same formatting, where the cost is dynamically added to the description
            // this is all becuase the code makes it impossible (or atleast sufficiently difficult) to just directly pass in a title and description as strings
            this.uIInfo = uIInfo;
        }

        public void FlipOffset() {
            offset.y *= -1;
        }
    }
}