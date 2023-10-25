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

    public void HoverEnter(UIInfo uIInfo, Transform position, Vector3 offset) {
        if (uIInfo == null) return;
        string name = uIInfo.GetName();
        if (name == "") return;
        gameObject.SetActive(true);
        transform.position = position.position+offset;
        anchoring.pivot = new Vector2(0.5f, offset.y >= 0 ? 0 : 1);
        title.text = name;
        description.text = uIInfo.GetDescription();
        LayoutRebuilder.ForceRebuildLayoutImmediate(layout);
    }

    public void HoverExit() {
        gameObject.SetActive(false);
    }
}
