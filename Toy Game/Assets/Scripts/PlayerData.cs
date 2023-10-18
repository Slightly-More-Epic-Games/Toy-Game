using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Toy Game/PlayerData", order = 0)]
public class PlayerData : ScriptableObject {
    public int health;
    public List<string> items;
}