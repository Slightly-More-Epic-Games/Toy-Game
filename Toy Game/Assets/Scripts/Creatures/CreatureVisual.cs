using System.Collections;
using System.Collections.Generic;
using Encounter;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreatureVisual : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI text;

    public void Init(Creature owner, PlayerController playerController) {
        button.onClick.AddListener(delegate{playerController.SelectCreature(owner);});
        image.sprite = owner.ui.icon;
    }

    public void UpdateVisual(Creature owner) {
        text.text = "health: "+owner.health+"\nimagination: "+owner.imagination+"\ntriggers: "+owner.triggers.Count;
    }
}
