using System.Collections;
using System.Collections.Generic;
using Encounter;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerCreature", menuName = "Toy Game/Player Creature", order = 0)]
public class PlayerCreature : Creature {
    public PlayerController playerController;
    public ItemTab itemTab;

    protected override void OnTurnStart() {
        playerController.gameObject.SetActive(true);
        itemTab.SetInteractable(this, true);
    }

    protected override void OnTurnEnd() {
        playerController.gameObject.SetActive(false);
        itemTab.SetInteractable(this, true);
    }

    protected override void OnEncounterStart() {
        itemTab = Manager.instance.CreateItemTab();
        itemTab.Init(this, playerController);
    }

    protected override void OnEncounterEnd() {
        imagination = 0;
        triggers.Clear();
    }

    public override void EventFinished() {
        base.EventFinished();
        itemTab.UpdateAllIcons(this);
    }
}