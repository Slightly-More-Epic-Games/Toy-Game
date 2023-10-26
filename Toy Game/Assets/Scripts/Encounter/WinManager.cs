using System.Collections;
using System.Collections.Generic;
using Items;
using UnityEngine;
using UnityEngine.UI;

namespace Encounter {
    public class WinManager : MonoBehaviour
    {
        private RewardUI[] uis;

        [SerializeField] private Transform rewardsParent;
        [SerializeField] private RewardUI rewardUIPrefab;

        private Item chosenItem;

        private void Start() {
            Map.EncounterNode encounterNode = (Map.EncounterNode)Map.Manager.instance.currentNode;

            List<Item> largePool = new List<Item>();
            List<Item> pool = new List<Item>();
            foreach (Creature creature in Manager.killedEnemies) {
                AddItemsToPool(creature, creature.isLarge ? largePool : pool);
            }

            uis = new RewardUI[Mathf.Min(pool.Count, encounterNode.rewards)+1];

            int large = largePool.Count;

            for (int i = 0; i < uis.Length-1; i++) {
                List<Item> p = i < large ? largePool : pool;
                int index = Random.Range(0, p.Count);
                Item item = p[index];
                item.ui.SetItem(item);
                p.RemoveAt(index);
                AddReward(item, i);
            }

            AddHealReward();
        }

        private void AddHealReward() {
            RewardUI rewardUI = Instantiate(rewardUIPrefab, rewardsParent);
            rewardUI.SetItem(null);
            uis[uis.Length-1] = rewardUI;
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
            if (chosenItem != null) Game.instance.player.AddItem(chosenItem);
            else Game.instance.player.health = Mathf.Min(Game.instance.player.health+5, Game.instance.player.maxHealth);
            Game.instance.LoadGameScene(Game.GameScene.Map);
        }
    }
}