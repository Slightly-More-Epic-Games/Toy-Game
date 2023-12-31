using System.Collections;
using System.Collections.Generic;
using Encounter;
using Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using HoverUI;

public class CreatureVisual : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image image;
    [SerializeField] private RectTransform healthBar;
    [SerializeField] private Transform triggerParent;
    [SerializeField] private HoverableUI triggerVisual;
    [SerializeField] private TextMeshProUGUI info;

    [SerializeField] private Image targetedIndicator;

    private CreatureUI ui;
    private SpriteAnimation spriteAnimation;

    public void Init(Creature owner, PlayerController playerController) {
        button.onClick.AddListener(delegate{playerController.SelectCreature(owner);});
        this.ui = owner.ui;
        this.spriteAnimation = owner.spriteAnimation;
    }

    private void Update() {
        image.sprite = spriteAnimation.GetSprite();
    }

    public void UpdateVisual(Creature owner) {
        // update health bar position
        healthBar.anchorMax = new Vector2(Mathf.Max(0f, owner.health/(float)owner.maxHealth), 1f);

        // update list of triggers above each creatures head
        foreach (Transform child in triggerParent) {
            Destroy(child.gameObject);
        }
        foreach (Trigger trigger in owner.triggers) {
            HoverableUI ui = Instantiate(triggerVisual, triggerParent);
            ui.SetInfo(trigger.ui);
            ui.image.sprite = trigger.ui.icon;
        }

        // update healthbar text (the creatures name and its current health)
        info.text = ui.GetName();
    }

    public void SetTargeted(bool active, float transparency) {
        // when a creature is targetted, a little box appears around it
        targetedIndicator.gameObject.SetActive(active);
        if (active) targetedIndicator.color = new Color(1, 1, 1, transparency);
    }
}
