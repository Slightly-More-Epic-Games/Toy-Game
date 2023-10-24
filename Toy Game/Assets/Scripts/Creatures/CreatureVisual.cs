using System.Collections;
using System.Collections.Generic;
using Encounter;
using Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreatureVisual : MonoBehaviour
{
    [SerializeField] private HoverableUI button;
    [SerializeField] private Image image;
    [SerializeField] private RectTransform healthBar;
    [SerializeField] private Transform triggerParent;
    [SerializeField] private HoverableUI triggerVisual;

    public void Init(Creature owner, PlayerController playerController) {
        button.onClick.AddListener(delegate{playerController.SelectCreature(owner);});
        button.SetInfo(owner.ui);
    }

    public void UpdateVisual(Creature owner) {
        healthBar.anchorMax = new Vector2(Mathf.Max(0f, owner.health/(float)owner.maxHealth), 1f);
        foreach (Transform child in triggerParent) {
            Destroy(child.gameObject);
        }
        foreach (Trigger trigger in owner.triggers) {
            HoverableUI ui = Instantiate(triggerVisual, triggerParent);
            ui.SetInfo(trigger.ui);
        }
    }
}
