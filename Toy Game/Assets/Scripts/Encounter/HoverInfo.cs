using System.Collections;
using System.Collections.Generic;
using Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HoverInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private RectTransform layout;

    public void HoverEnter(UIInfo uIInfo, Transform position) {
        if (uIInfo.name == "") return;
        gameObject.SetActive(true);
        transform.position = position.position;
        title.text = uIInfo.name;
        description.text = uIInfo.description;
        LayoutRebuilder.ForceRebuildLayoutImmediate(layout);
    }

    public void HoverExit() {
        gameObject.SetActive(false);
    }
}
