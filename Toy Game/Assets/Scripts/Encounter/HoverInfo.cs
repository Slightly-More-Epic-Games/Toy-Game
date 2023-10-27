using System.Collections;
using System.Collections.Generic;
using Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using HoverUI;

public class HoverInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private RectTransform layout;
    [SerializeField] private RectTransform anchoring;

    private Transform obj;

    private void Update() {
        if (obj == null) {
            //if the currently hovered object dissapears, hide the hover info
            HoverExit();
        }
    }

    public void HoverEnter(UIInfo uIInfo, Vector3 position, Vector3 offset, Transform obj) {
        if (uIInfo == null) return;
        string name = uIInfo.GetName();
        if (name == "") return;
        this.obj = obj;
        gameObject.SetActive(true);
        transform.position = position+offset;
        anchoring.pivot = new Vector2(0.5f, offset.y >= 0 ? 0 : 1);
        title.text = name;
        description.text = uIInfo.GetDescription();
        LayoutRebuilder.ForceRebuildLayoutImmediate(layout);
    }

    public void HoverExit() {
        gameObject.SetActive(false);
    }
}
