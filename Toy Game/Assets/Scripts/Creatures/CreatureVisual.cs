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

    private CreatureUI ui;
    private Sprite[] sprites;
    private float fps;

    public void Init(Creature owner, PlayerController playerController) {
        button.onClick.AddListener(delegate{playerController.SelectCreature(owner);});
        this.ui = owner.ui;
        this.sprites = owner.sprites;
        this.fps = owner.fps;
    }

    private void Update() {
        image.sprite = sprites[Mathf.RoundToInt(Time.time*fps)%sprites.Length];
    }

    public void UpdateVisual(Creature owner) {
        healthBar.anchorMax = new Vector2(Mathf.Max(0f, owner.health/(float)owner.maxHealth), 1f);
        foreach (Transform child in triggerParent) {
            Destroy(child.gameObject);
        }
        foreach (Trigger trigger in owner.triggers) {
            HoverableUI ui = Instantiate(triggerVisual, triggerParent);
            ui.SetInfo(trigger.ui);
            ui.image.sprite = trigger.ui.icon;
        }
        info.text = ui.GetName();
    }
}
