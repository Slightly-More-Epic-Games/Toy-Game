using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Encounter {
    // priorities are a set of values that inform auto controllers
    // on creatures most priorities should be 1 - it represents how much the creature "cares" about the specific metric
    // on items the priorities should be how good the item is at doing things given the intended target
    // so if an item deals 3 damage at the cost of 1 health, it is good at dealing damge (so a enemy damage of 1) - and it is alright at damaging self (so a self damage of 0.5)
    // and then the current turns priority is how much each metric is needed - if an ally is on low health then "ally damage" might have a priority of -1 - so if an item is good at dealing ally damage when used on its intended target, it will get scored badly

    // the actual data structure is simple though, and doesnt really need commenting

    [System.Serializable]
    public class Priorities
    {
        public PrioritySet self = null;
        public PrioritySet allies = null;
        public PrioritySet enemies = null;

        public Priorities() {
            this.self = new PrioritySet(0f, 0f, 0f, 0f);
            this.allies = new PrioritySet(0f, 0f, 0f, 0f);
            this.enemies = new PrioritySet(0f, 0f, 0f, 0f);
        }

        public Priorities(PrioritySet self, PrioritySet allies, PrioritySet enemies) {
            this.self = self;
            this.allies = allies;
            this.enemies = enemies;
        }

        public static Priorities Multiply(Priorities a, Priorities b) {
            return new Priorities(PrioritySet.Multiply(a.self, b.self), PrioritySet.Multiply(a.allies, b.allies), PrioritySet.Multiply(a.enemies, b.enemies));
        }

        public float Total() {
            return self.Total() + allies.Total() + enemies.Total();
        }
    }

    [System.Serializable]
    public class PrioritySet {
        public float heal = 1f;
        public float damage = 1f;
        public float buff = 1f;
        public float debuff = 1f;

        public PrioritySet() {}

        public PrioritySet(float heal, float damage, float buff, float debuff) {
            this.heal = heal;
            this.damage = damage;
            this.buff = buff;
            this.debuff = debuff;
        }

        public static PrioritySet Multiply(PrioritySet a, PrioritySet b) {
            return new PrioritySet(a.heal*b.heal, a.damage*b.damage, a.buff*b.buff, a.debuff*b.debuff);
        }

        public float Total() {
            return heal+damage+buff+debuff;
        }

        public float PositiveTotal() {
            return heal+buff;
        }

        public float NegativeTotal() {
            return damage+debuff;
        }
    }
}