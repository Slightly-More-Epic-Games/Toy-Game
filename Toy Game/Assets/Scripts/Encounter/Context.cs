using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Encounter {
    public class Context
    {
        public Action action;
        public Creature source;
        public Creature target;
        public int value;

        public Context(Action action, Creature source, Creature target, int value) {
            this.action = action;
            this.source = source;
            this.target = target;
            this.value = value;
        }

        public List<Creature> GetTargets(Target targetType, Creature owner) {
            List<Creature> targets = new List<Creature>();

            switch (targetType) {
                case Target.Source:
                    targets.Add(source);
                    break;
                case Target.Target:
                    targets.Add(target);
                    break;
                case Target.Self:
                    targets.Add(owner);
                    break;
                case Target.Random:
                case Target.All:
                    targets.AddRange(Manager.instance.playerAllies);
                    targets.AddRange(Manager.instance.playerEnemies);
                    break;
                case Target.NotSelfRandom:
                case Target.NotSelfAll:
                    targets.AddRange(Manager.instance.playerAllies);
                    targets.AddRange(Manager.instance.playerEnemies);
                    targets.Remove(owner);
                    break;
                case Target.OpponentsRandom:
                case Target.OpponentsAll:
                    if (Manager.instance.playerAllies.Contains(owner)) {
                        targets.AddRange(Manager.instance.playerEnemies);
                    } else {
                        targets.AddRange(Manager.instance.playerAllies);
                    }
                    break;
                case Target.AlliesRandom:
                case Target.AlliesAll:
                    if (Manager.instance.playerAllies.Contains(owner)) {
                        targets.AddRange(Manager.instance.playerAllies);
                    } else {
                        targets.AddRange(Manager.instance.playerEnemies);
                    }
                    break;
                case Target.LastAttacker:
                    if (owner.lastAttacker == null) return GetTargets(Target.OpponentsRandom, owner);
                    targets.Add(owner.lastAttacker);
                    break;
            };

            if (targetType == Target.NotSelfRandom || targetType == Target.OpponentsRandom || targetType == Target.AlliesRandom || targetType == Target.Random) {
                return new List<Creature>(){targets[Random.Range(0, targets.Count)]};
            }

            return targets;
        }
    }
}