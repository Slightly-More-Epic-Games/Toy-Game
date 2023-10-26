using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpriteAnimation {
    public Sprite[] sprites;
    public float fps = 1f;

    public Sprite GetSprite() {
        return sprites[Mathf.RoundToInt(Time.time*fps)%sprites.Length];
    }
}