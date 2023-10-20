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
        itemTab.SetInteractable(true);
        base.OnTurnStart();
    }

    protected override void OnTurnEnd() {
        playerController.gameObject.SetActive(false);
        itemTab.SetInteractable(true);
        base.OnTurnEnd();
    }

    protected override void OnEncounterStart() {
        base.OnEncounterStart();
        itemTab = Manager.instance.CreateItemTab();
        itemTab.Init(this, playerController);
    }

    public override void EventFinished() {
        base.EventFinished();
        itemTab.UpdateAllIcons();
    }
}