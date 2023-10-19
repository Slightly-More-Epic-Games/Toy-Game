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
                case Target.SOURCE:
                    targets.Add(source);
                    break;
                case Target.TARGET:
                    targets.Add(target);
                    break;
                case Target.SELF:
                    targets.Add(owner);
                    break;
                case Target.RANDOM:
                case Target.ALL:
                    targets.AddRange(Manager.instance.playerAllies);
                    targets.AddRange(Manager.instance.playerEnemies);
                    break;
                case Target.RANDOM_NOT_SELF:
                case Target.ALL_NOT_SELF:
                    targets.AddRange(Manager.instance.playerAllies);
                    targets.AddRange(Manager.instance.playerEnemies);
                    targets.Remove(owner);
                    break;
                case Target.RANDOM_OPPONENT:
                case Target.ALL_OPPONENTS:
                    if (Manager.instance.playerAllies.Contains(owner)) {
                        targets.AddRange(Manager.instance.playerEnemies);
                    } else {
                        targets.AddRange(Manager.instance.playerAllies);
                    }
                    break;
                case Target.RANDOM_ALLY:
                case Target.ALL_ALLIES:
                    if (Manager.instance.playerAllies.Contains(owner)) {
                        targets.AddRange(Manager.instance.playerAllies);
                    } else {
                        targets.AddRange(Manager.instance.playerEnemies);
                    }
                    break;
                case Target.LAST_ATTACKER:
                    if (owner.lastAttacker == null) return GetTargets(Target.RANDOM_OPPONENT, owner);
                    targets.Add(owner.lastAttacker);
                    break;
            };

            if (targetType == Target.RANDOM_NOT_SELF || targetType == Target.RANDOM_OPPONENT || targetType == Target.RANDOM_ALLY || targetType == Target.RANDOM) {
                return new List<Creature>(){targets[Random.Range(0, targets.Count)]};
            }

            return targets;
        }
    }
}