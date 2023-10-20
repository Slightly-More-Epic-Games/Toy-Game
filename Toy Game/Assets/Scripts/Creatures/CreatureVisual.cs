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
}
