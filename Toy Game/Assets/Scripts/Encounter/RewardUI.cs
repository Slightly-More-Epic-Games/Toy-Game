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
    
        public void SetItem(Item item) {
            title.text = item.ui.name;
            description.text = item.ui.description;
            image.sprite = item.ui.icon;
        }

        public void SetChosen(bool active) {

        }
    }
}