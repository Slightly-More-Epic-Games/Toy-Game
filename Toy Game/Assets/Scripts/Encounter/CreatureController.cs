using System.Collections;
using System.Collections.Generic;
using Items;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Encounter {
    public class CreatureController : MonoBehaviour
    {
        protected bool usingItem;
        [SerializeField] protected Image itemUseVisual;
        protected Transform currentVisual;
        protected Transform targetVisual;
        [SerializeField] protected float itemUseTime;
        [SerializeField] protected float minUseSpeed;
        protected float itemUseSpeed;
        protected UnityAction onComplete;

        protected virtual void Update() {
            if (usingItem) {
                // update the item throw animation
                // once it hits, use the item and destroy
                currentVisual.position = Vector3.MoveTowards(currentVisual.position, targetVisual.position, Time.deltaTime*itemUseSpeed);
                if (Vector3.Distance(currentVisual.position, targetVisual.position) < 1f) {
                    onComplete.Invoke();
                    Destroy(currentVisual.gameObject);
                    usingItem = false;
                }
            }
        }

        public virtual void UpdateTurn(Creature owner, int turnNumber) {}

        public virtual void OnTurnStart(Creature owner) {}

        public virtual void OnTurnEnd(Creature owner) {}

        public virtual void OnEncounterStart(Creature owner) {}

        public virtual void OnEncounterEnd(Creature owner) {}

        public virtual void OnEventsFinished(Creature owner) {}

        public void UseItemPhysically(ItemSlot itemSlot, Creature source, Creature target, int index) {
            usingItem = true;
            onComplete = delegate {
                itemSlot.Use(source, target, index);
            };
            Image image = Instantiate(itemUseVisual, source.creatureVisual.transform.parent.parent.parent);
            image.sprite = itemSlot.GetItemUI().icon;
            currentVisual = image.transform;
            currentVisual.position = source.creatureVisual.transform.position;
            if (source == target) {
                currentVisual.position -= new Vector3(Mathf.Sign(currentVisual.position.x)*24, 0, 0);
            }
            targetVisual = target.creatureVisual.transform;
            itemUseSpeed = Vector3.Distance(currentVisual.position, targetVisual.position)/itemUseTime;
            itemUseSpeed = Mathf.Max(itemUseSpeed, minUseSpeed);
        }
    }
}