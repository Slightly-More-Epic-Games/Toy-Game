using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScaler : MonoBehaviour
{
    [SerializeField] private RectTransform canvas;
    private int lastHeight;

    private void Start() {
        SetSize();
    }

    private void Update() {
        // world space canvases dont support scaling to screen size, however its easy enough to just hardcode the one case we need to use
        if (Screen.height != lastHeight) {
            SetSize();
        }
    }

    private void SetSize() {
        canvas.sizeDelta = new Vector2((Screen.width/(float)Screen.height)*canvas.sizeDelta.y, canvas.sizeDelta.y);
        lastHeight = Screen.height;
    }
}
