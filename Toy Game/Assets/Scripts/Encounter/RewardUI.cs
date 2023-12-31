using System.Collections;
using System.Collections.Generic;
using Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Encounter {
    public class RewardUI : MonoBehaviour
    {
        public TextMeshProUGUI title;
        public TextMeshProUGUI description;
        public Image image;
        public Button button;
        public TextMeshProUGUI prompt;
        public string promptText;
        public string selectedText;
    
        public void SetItem(Item item) {
            if (item == null) return;
            title.text = item.ui.GetName();
            description.text = item.ui.GetDescription();
            image.sprite = item.ui.icon;
            prompt.text = promptText;
        }

        public void SetChosen(bool active) {
            prompt.text = active ? selectedText : promptText;
        }
    }
}