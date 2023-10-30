using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpriteAnimation {
    public Sprite[] sprites;
    public float fps = 1f;

    public Sprite GetSprite() {
        // instead of individually tracking a current time or current sprite, Time.time can be used
        // then the sprites list can just be indexed based on the current time at a speed determined by fps
        // classes then just have to call the GetSprite() method on their animation instance each frame
        return sprites[Mathf.RoundToInt(Time.time*fps)%sprites.Length];
    }
}