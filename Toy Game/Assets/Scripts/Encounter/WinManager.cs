using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Encounter {
    public class WinManager : MonoBehaviour
    {
        private Item[] rewards;
        private RewardUI[] uis;

        [SerializeField] private Transform rewardsParent;
        [SerializeField] private RewardUI rewardUIPrefab;

        private void Start() {
            Map.EncounterNode encounterNode = (Map.EncounterNode)Map.Manager.instance.currentNode;

            List<Item> pool = new List<Item>();
            foreach (Creature creature in Manager.killedEnemies) {
                foreach (Item item in creature.dropPool) {
                    if (!pool.Contains(item)) {
                        pool.Add(item);
                    }
                }
            }

            rewards = new Item[Mathf.Min(pool.Count, encounterNode.rewards)];
            uis = new RewardUI[rewards.Length];
            for (int i = 0; i < rewards.Length; i++) {
                int index = Random.Range(0, pool.Count);
                Item item = pool[index];
                rewards[i] = item;
                pool.RemoveAt(index);

                RewardUI rewardUI = Instantiate(rewardUIPrefab, rewardsParent);
                rewardUI.SetItem(item);
                uis[i] = rewardUI;
                rewardUI.button.onClick.AddListener(delegate {
                    ChooseItem(item, rewardUI);
                });
            }
        }

        public void ChooseItem(Item item, RewardUI rewardUI) {
            Game.instance.player.items.Add(item);
            foreach (RewardUI ui in uis) {
                ui.SetChosen(ui == rewardUI);
            }
        }

        public void Continue() {
            Game.instance.LoadGameScene(Game.GameScene.Map);
        }
    }
}