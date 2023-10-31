using System.Collections;
using System.Collections.Generic;
using Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Encounter {
    public class WinManager : MonoBehaviour
    {
        private RewardUI[] uis;

        [SerializeField] private Transform rewardsParent;
        [SerializeField] private RewardUI rewardUIPrefab;

        private Item chosenItem;

        [SerializeField] private TextMeshProUGUI winTitle;
        [SerializeField] private TextMeshProUGUI winPrompt;

        [SerializeField] private string regularWinTitle;
        [SerializeField] private string regularWinPrompt;
        [SerializeField] private string largeWinTitle;
        [SerializeField] private string largeWinPrompt;

        private void Start() {
            Map.EncounterNode encounterNode = (Map.EncounterNode)Map.Manager.instance.currentNode;

            // add drop pool of each killed enemy to respective lists
            // -- dont add duplicates
            List<Item> largePool = new List<Item>();
            List<Item> pool = new List<Item>();
            foreach (Creature creature in Manager.killedEnemies) {
                AddItemsToPool(creature, creature.isLarge ? largePool : pool);
            }

            // set up reward array based on the nodes rewards
            int rewards = Mathf.Min(Mathf.Min(pool.Count, encounterNode.rewards)+1, 3);
            uis = new RewardUI[rewards];

            int large = largePool.Count;

            // if a "large creature" (a boss) was killed, its items get prioritised
            for (int i = 0; i < (large == 0 ? uis.Length-1 : uis.Length); i++) {
                List<Item> p = i < large ? largePool : pool;
                int index = Random.Range(0, p.Count);
                Item item = p[index];
                item.ui.SetItem(item);
                p.RemoveAt(index);
                AddReward(item, i);
            }

            // if the max rewards (3) isnt hit, add a heal reward
            if (rewards != encounterNode.rewards) {
                AddHealReward();
            }

            // the text when killing a boss is different to regular encounters
            // so this is where the "Healed Fully, Choose Item" message on boss kill is done, and generally its how winmanager knows if it was a boss level
            if (large == 0) {
                winTitle.text = regularWinTitle;
                winPrompt.text = regularWinPrompt;
            } else {
                winTitle.text = largeWinTitle;
                winPrompt.text = largeWinPrompt;
            }
        }

        private void AddHealReward() {
            // adds a heal reward to the last slot in the array
            RewardUI rewardUI = Instantiate(rewardUIPrefab, rewardsParent);
            rewardUI.SetItem(null);
            uis[uis.Length-1] = rewardUI;
            // by default a no item reward is healing
            // this would make it hard to add any other non-item rewards
            rewardUI.button.onClick.AddListener(delegate {
                ChooseItem(null, rewardUI);
            });
            rewardUI.SetChosen(true);
        }

        private void AddReward(Item item, int index) {
            RewardUI rewardUI = Instantiate(rewardUIPrefab, rewardsParent);
            rewardUI.SetItem(item);
            uis[index] = rewardUI;
            rewardUI.button.onClick.AddListener(delegate {
                ChooseItem(item, rewardUI);
            });
        }

        private void AddItemsToPool(Creature creature, List<Item> pool) {
            foreach (Item item in creature.dropPool) {
                if (!pool.Contains(item)) {
                    pool.Add(item);
                }
            }
        }

        public void ChooseItem(Item item, RewardUI rewardUI) {
            chosenItem = item;
            foreach (RewardUI ui in uis) {
                ui.SetChosen(ui == rewardUI);
            }
        }

        public void Continue() {
            // give the chosen item or heal the player
            if (chosenItem != null) {
                Game.instance.player.AddItem(chosenItem);
            } else {
                Game.instance.player.health = Mathf.Min(Game.instance.player.health+5, Game.instance.player.maxHealth);
            }
            Game.instance.LoadGameScene(Game.GameScene.Map);
        }
    }
}