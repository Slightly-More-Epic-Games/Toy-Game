using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Items;
using UnityEngine;

namespace Encounter {
    public class AutoController : CreatureController
    {
        public bool isAlly;

        public override void UpdateTurn(Creature owner, int turnNumber) {
            UseBestItem(owner, turnNumber);
            owner.EndTurn();
        }

        protected void UseBestItem(Creature owner, int turnNumber) {
            List<Creature> allies = new List<Creature>(isAlly ? Manager.instance.playerAllies : Manager.instance.playerEnemies);
            List<Creature> enemies = new List<Creature>(isAlly ? Manager.instance.playerEnemies : Manager.instance.playerAllies);
            allies.Remove(owner);

            Debug.Log((isAlly ? "ally" : "enemy")+"\""+owner+"\" is trying to use best item...");

            Priorities priorities = Priorities.Multiply(GetCurrentPriorities(owner, allies, enemies, turnNumber), owner.priorities);

            Item item = GetBestItem(owner, allies, enemies, priorities, 1.3f);
            Debug.Log("the best item is: "+item);
            if (item == null) return;

            Creature target = GetBestTarget(owner, allies, enemies, item, priorities);
            Debug.Log("the best target for that item is: "+target);
            if (target == null) return;

            owner.UseItem(owner.items.IndexOf(item), target);
        }

        protected Creature GetBestTarget(Creature owner, List<Creature> allies, List<Creature> enemies, Item item, Priorities priorities) {
            Priorities itemPriorities = Priorities.Multiply(item.priorities, owner.priorities);

            float selfTarget = Mathf.Max(itemPriorities.self.PositiveTotal(), 0) * item.targetWeights.self;
            float allyTarget = Mathf.Max(itemPriorities.allies.PositiveTotal(), 0) * item.targetWeights.ally;
            float enemyTarget = Mathf.Max(itemPriorities.enemies.NegativeTotal(), 0) * item.targetWeights.enemy;

            if (allies.Count == 0) allyTarget = 0;
            if (enemies.Count == 0) enemyTarget = 0;

            float totalWeight = selfTarget+allyTarget+enemyTarget;
            float random = Random.value*totalWeight;

            Creature target;
            if (random < selfTarget || totalWeight == 0) {
                target = owner;
            } else if (random < selfTarget+allyTarget) {
                target = allies[0];
                float maxNeed = 0f;
                float maxMatch = 1f;
                foreach (Creature ally in allies) {
                    float healthPercent = (float)ally.health/ally.maxHealth;
                    float need = 1f - (1f - itemPriorities.allies.heal*(1f-healthPercent)) * (1f - itemPriorities.allies.buff/((ally.triggers.Count*(1f-healthPercent))+1f));
                    if ((1f-healthPercent) * itemPriorities.allies.damage > 0.65f) need /= 2f;
                    if (need > maxNeed) {
                        maxNeed = need;
                        target = ally;
                        maxMatch = 1f;
                    } else if (need == maxNeed) {
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
                foreach (Creature enemy in enemies) {
                    float healthPercent = (float)enemy.health/enemy.maxHealth;
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

        protected Item GetBestItem(Creature owner, List<Creature> allies, List<Creature> enemies, Priorities priorities, float slope) {
            List<Item> items = GetFavouredItems(owner, allies, enemies, priorities);
            return GetWeightedEarlyFromItemList(items, slope);
        }

        protected Item GetWeightedEarlyFromItemList(List<Item> items, float slope) {
            if (items.Count == 0) return null;

            //i think the weight for each index is equal to round(x+1)^(1-slope)?
            //this means slope > 1 will make earlier items are more likely
            float chanceReduction = 1f;
            Item current = null;
            foreach (Item item in items) {
                if (Random.value <= 1f/chanceReduction) {
                    current = item;
                }
                chanceReduction += slope;
            }

            return current;   
        }

        protected List<Item> GetFavouredItems(Creature owner, List<Creature> allies, List<Creature> enemies, Priorities priorities) {
            List<Item> usableItems = GetUsableItems(owner);

            if (usableItems.Count == 0) return usableItems;

            List<(Item, float)> itemScores = new List<(Item, float)>();
            float[] itemPreferences = new float[usableItems.Count];
            for (int i = 0; i < itemPreferences.Length; i++) {
                Item item = usableItems[i];
                Priorities itemPriorities = Priorities.Multiply(item.priorities, priorities);
                float score = itemPriorities.Total();
                itemScores.Add((item, score));
            }

            itemScores.OrderBy(s => s.Item2);

            List<Item> favouredItems = new List<Item>();

            bool wasPositive = false;
            float max = 1f;
            foreach ((Item,float) itemScore in itemScores) {
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
            return usableItems;
        }

        protected Priorities GetCurrentPriorities(Creature self, List<Creature> allies, List<Creature> enemies, int turnNumber) {
            Priorities priorities = new Priorities();

            float selfHealthPercent = (float)self.health/self.maxHealth;
            priorities.self.heal = 1f - selfHealthPercent;
            priorities.self.damage = selfHealthPercent - 1f;
            priorities.self.buff = 1f/(self.triggers.Count+1f);
            priorities.self.debuff = selfHealthPercent - 1f;

            foreach (Creature creature in allies) {
                float creatureHealthPercent = (float)creature.health/creature.maxHealth;
                priorities.allies.heal = Mathf.Max(priorities.allies.heal, 1f - creatureHealthPercent);
                priorities.allies.damage = Mathf.Min(priorities.allies.heal, creatureHealthPercent - 1f);
                priorities.allies.buff = Mathf.Max(priorities.allies.buff, 1f/(self.triggers.Count+1f));
                priorities.allies.debuff = Mathf.Min(priorities.allies.debuff, (creatureHealthPercent/Mathf.Max(enemies.Count,1f))-1f);
            }

            foreach (Creature creature in enemies) {
                float creatureHealthPercent = (float)creature.health/creature.maxHealth;
                priorities.enemies.heal = Mathf.Min(priorities.enemies.heal, (creatureHealthPercent/(float)turnNumber)-1f);
                priorities.enemies.damage = Mathf.Max(priorities.enemies.damage, (2f-creatureHealthPercent)*(1f-selfHealthPercent));
                priorities.enemies.buff = Mathf.Min(priorities.enemies.buff, (selfHealthPercent*(1f-priorities.allies.heal))-1f);
                priorities.enemies.debuff = Mathf.Max(priorities.enemies.debuff, Mathf.Max(creatureHealthPercent, priorities.allies.heal)*selfHealthPercent);
            }

            return priorities;
        }

        protected List<Item> GetUsableItems(Creature owner) {
            List<Item> items = new List<Item>();
            foreach (Item item in owner.items) {
                if (item.CanUse(owner)) {
                    items.Add(item);
                }
            }
            return items;
        }
    }
}