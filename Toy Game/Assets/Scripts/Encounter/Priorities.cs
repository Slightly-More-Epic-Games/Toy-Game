using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Encounter {
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