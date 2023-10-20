using System.Collections;
using System.Collections.Generic;
using Encounter;
using UnityEngine;
using UnityEngine.UI;

public class CreatureVisual : MonoBehaviour
{
    [SerializeField] private Button button;

    public void Init(Creature owner) {
        button.onClick.AddListener(delegate{Game.instance.player.playerController.SelectCreature(owner);});
    }

    public void UpdateVisual(Creature owner) {
        Debug.Log("new stats for "+owner.name+": health:"+owner.health+" imagination:"+owner.imagination+" with "+owner.triggers.Count+" triggers");
    }
}
