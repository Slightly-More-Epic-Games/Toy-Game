using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Items;
using UnityEngine;

namespace Encounter {
    public class AutoController : CreatureController
    {
        public bool isAlly;

        private bool readyToEnd;
        private float useItemAt;

        [SerializeField] private float itemDelay;

        public override void OnTurnStart(Creature owner) {
            readyToEnd = false;
            useItemAt = Time.time + itemDelay;
        }

        public override void UpdateTurn(Creature owner, int turnNumber) {
            // use item, once the item has hit the target end turn
            if (!usingItem) {
                if (readyToEnd) {
                    owner.EndTurn();
                } else if (useItemAt < Time.time) {
                    useItemAt = Time.time + itemDelay;
                    bool success = UseBestItem(owner, turnNumber);
                    //once 2 bosses have been killed, give creatures a chance to use multiple items
                    if (!success || Random.Range(0,4) >= Mathf.Min(Map.Manager.instance.bossesKilled-1, 2)) {
                        readyToEnd = true;
                    }
                }
            }
        }

        protected bool UseBestItem(Creature owner, int turnNumber) {
            // gets the "best" item, then uses it on the "best" target
            if (owner.isDead) return false;
            List<Creature> allies = new List<Creature>(isAlly ? Manager.instance.playerAllies : Manager.instance.playerEnemies);
            List<Creature> enemies = new List<Creature>(isAlly ? Manager.instance.playerEnemies : Manager.instance.playerAllies);
            allies.Remove(owner);

            Priorities priorities = Priorities.Multiply(GetCurrentPriorities(owner, allies, enemies, turnNumber), owner.priorities);

            ItemSlot item = GetBestItem(owner, priorities, 1.3f);
            if (item == null) return false;

            Creature target = GetBestTarget(owner, allies, enemies, item, priorities);
            if (target == null) return false;

            owner.UseItem(owner.items.IndexOf(item), target);
            return true;
        }

        protected Creature GetBestTarget(Creature owner, List<Creature> allies, List<Creature> enemies, ItemSlot item, Priorities priorities) {
            // get the current item priorities - this tells the creature roughly what the item will do
            Priorities itemPriorities = Priorities.Multiply(item.GetPriorities(), owner.priorities);

            // weight each target by how useful to the owners team the item will be
            float selfTarget =  Mathf.Max(itemPriorities.self.PositiveTotal(),    0) * item.GetTargetWeights().self;
            float allyTarget =  Mathf.Max(itemPriorities.allies.PositiveTotal(),  0) * item.GetTargetWeights().ally;
            float enemyTarget = Mathf.Max(itemPriorities.enemies.NegativeTotal(), 0) * item.GetTargetWeights().enemy;

            if (allies.Count == 0) allyTarget = 0;
            if (enemies.Count == 0) enemyTarget = 0;

            float totalWeight = selfTarget+allyTarget+enemyTarget;
            float random = Random.value*totalWeight;

            Creature target;
            // pick target based on weights
            if (random < selfTarget || totalWeight == 0) {
                target = owner;
            } else if (random < selfTarget+allyTarget) {
                // if the target is an ally, figure out the ally which is most useful to target with that item
                target = allies[0];
                float maxNeed = 0f;
                float maxMatch = 1f;
                foreach (Creature ally in allies) {
                    float healthPercent = (float)ally.health/ally.maxHealth;
                    // this is done with a slightly magic formula, with the idea that if the item heals and the creature is on low health, it needs healing etc
                    float need = 1f - (1f - itemPriorities.allies.heal*(1f-healthPercent)) * (1f - itemPriorities.allies.buff/((ally.triggers.Count*(1f-healthPercent))+1f));
                    if ((1f-healthPercent) * itemPriorities.allies.damage > 0.65f) need /= 2f;
                    if (need > maxNeed) {
                        maxNeed = need;
                        target = ally;
                        maxMatch = 1f;
                    } else if (need == maxNeed) {
                        // on the off chance creatures have equal needs, pick uniformally random
                        maxMatch++;
                        if (Random.value <= 1f/maxMatch) {
                            target = ally;
                        }
                    }
                }
            } else {
                target = enemies[0];
                float maxNeed = 0f;
                float maxMatch = 1f;
                // if the target is an enemy, figure out which one is the most useful
                foreach (Creature enemy in enemies) {
                    float healthPercent = (float)enemy.health/enemy.maxHealth;
                    // the formula for this one is basically just the same but backwards
                    float need = 1f - (1f - itemPriorities.enemies.damage*(1f-healthPercent)) * (1f - itemPriorities.enemies.debuff*Mathf.Max(healthPercent, (float)owner.health/owner.maxHealth));
                    if ((1f-healthPercent) * itemPriorities.enemies.heal > 0.65f) need /= 2f;
                    need += 1f - (1f - enemy.priorities.enemies.damage*priorities.allies.heal)*(1f - enemy.priorities.enemies.debuff*priorities.allies.buff);
                    if (need > maxNeed) {
                        maxNeed = need;
                        target = enemy;
                        maxMatch = 1f;
                    } else if (need == maxNeed) {
                        maxMatch++;
                        if (Random.value <= 1f/maxMatch) {
                            target = enemy;
                        }
                    }
                }
            }

            return target;
        }

        protected ItemSlot GetBestItem(Creature owner, Priorities priorities, float slope) {
            List<ItemSlot> items = GetFavouredItems(owner, priorities);
            return GetWeightedEarlyFromItemList(items, slope);
        }

        protected ItemSlot GetWeightedEarlyFromItemList(List<ItemSlot> items, float slope) {
            if (items.Count == 0) return null;

            //i think the weight for each index is equal to round(x+1)^(1-slope)?
            //this means slope > 1 will make earlier items are more likely
            float chanceReduction = 1f;
            ItemSlot current = null;
            foreach (ItemSlot item in items) {
                if (Random.value <= 1f/chanceReduction) {
                    current = item;
                }
                chanceReduction += slope;
            }

            return current;   
        }

        protected List<ItemSlot> GetFavouredItems(Creature owner, Priorities priorities) {
            // given the current turn priorities, rank all items by how useful they are and return them as a sorted list...

            List<ItemSlot> usableItems = GetUsableItems(owner);
            // if no items are useable return an (the) empty list
            if (usableItems.Count == 0) return usableItems;

            // get scores for each item
            List<(ItemSlot, float)> itemScores = new List<(ItemSlot, float)>();
            float[] itemPreferences = new float[usableItems.Count];
            for (int i = 0; i < itemPreferences.Length; i++) {
                ItemSlot item = usableItems[i];
                // this is done by multiplying the items "uses" (its priorities) by the current turns priorities - then totalling the scores
                Priorities itemPriorities = Priorities.Multiply(item.GetPriorities(), priorities);
                float score = itemPriorities.Total();
                // since current turn scores can go into negatives, this means some scores will go < 0 if an item would be actively bad to use
                itemScores.Add((item, score));
            }

            itemScores.OrderBy(s => s.Item2);

            List<ItemSlot> favouredItems = new List<ItemSlot>();

            bool wasPositive = false;
            float max = 1f;
            // given the sorted list, add to a new list if the score is above 0
            // if all scores are below 0, pick the "least bad"
            foreach ((ItemSlot,float) itemScore in itemScores) {
                if (itemScore.Item2 > 0) {
                    wasPositive = true;
                    favouredItems.Add(itemScore.Item1);
                } else if (wasPositive) {
                    break;
                } else {
                    if (max > 0 || max == itemScore.Item2) {
                        favouredItems.Add(itemScore.Item1);
                        max = itemScore.Item2;
                    } else {
                        break;
                    }
                }
            }

            if (favouredItems.Count > 0) {
                return favouredItems;
            }
            // if somehow nothing is favoured, return the usable items list
            return usableItems;
        }

        protected Priorities GetCurrentPriorities(Creature self, List<Creature> allies, List<Creature> enemies, int turnNumber) {
            Priorities priorities = new Priorities();
            // get the current turns priorities

            float selfHealthPercent = (float)self.health/self.maxHealth;
            priorities.self.heal = 1f - selfHealthPercent;
            priorities.self.damage = selfHealthPercent - 1f;
            priorities.self.buff = 1f/(self.triggers.Count+1f);
            priorities.self.debuff = selfHealthPercent - 1f;

            // ally priorities are the priority of the creature that needs each thing the most
            foreach (Creature creature in allies) {
                float creatureHealthPercent = (float)creature.health/creature.maxHealth;
                priorities.allies.heal = Mathf.Max(priorities.allies.heal, 1f - creatureHealthPercent);
                priorities.allies.damage = Mathf.Min(priorities.allies.heal, creatureHealthPercent - 1f);
                priorities.allies.buff = Mathf.Max(priorities.allies.buff, 1f/(self.triggers.Count+1f));
                priorities.allies.debuff = Mathf.Min(priorities.allies.debuff, (creatureHealthPercent/Mathf.Max(enemies.Count,1f))-1f);
            }

            // likewise for enemies
            foreach (Creature creature in enemies) {
                float creatureHealthPercent = (float)creature.health/creature.maxHealth;
                priorities.enemies.heal = Mathf.Min(priorities.enemies.heal, (creatureHealthPercent/(float)turnNumber)-1f);
                priorities.enemies.damage = Mathf.Max(priorities.enemies.damage, (2f-creatureHealthPercent)*(1f-selfHealthPercent));
                priorities.enemies.buff = Mathf.Min(priorities.enemies.buff, (selfHealthPercent*(1f-priorities.allies.heal))-1f);
                priorities.enemies.debuff = Mathf.Max(priorities.enemies.debuff, Mathf.Max(creatureHealthPercent, priorities.allies.heal)*selfHealthPercent);
            }

            return priorities;
        }

        protected List<ItemSlot> GetUsableItems(Creature owner) {
            List<ItemSlot> items = new List<ItemSlot>();
            foreach (ItemSlot item in owner.items) {
                if (item.CanUse(owner)) {
                    items.Add(item);
                }
            }
            return items;
        }
    }
}