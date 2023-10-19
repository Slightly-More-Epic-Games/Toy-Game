using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Context
{
    public enum Target {
        SOURCE,
        TARGET,
        SELF,
        ALL,
        RANDOM,
        ALL_NOT_SELF,
        RANDOM_NOT_SELF,
        ALL_OPPONENTS,
        RANDOM_OPPONENTS,
        ALL_ALLIES,
        RANDOM_ALLIES
    }

    public enum Action {
        ITEM_USED,
        ANY_ITEM_USED,
        DEAL_DAMAGE,
        GAIN_HEALTH,
        LOSE_HEALTH,
        GAIN_IMAGINATION,
        LOSE_IMAGINATION,
        TURN_START,
        TURN_END,
        ENCOUNTER_START,
        ENCOUNTER_END
    }

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
            case Target.SELF:
                targets.Add(owner);
                break;
            case Target.RANDOM_NOT_SELF:
            case Target.ALL_NOT_SELF:
                //
                break;
            case Target.RANDOM_OPPONENTS:
            case Target.ALL_OPPONENTS:
                //
                break;
            case Target.RANDOM_ALLIES:
            case Target.ALL_ALLIES:
                //
                break;
            case Target.RANDOM:
            case Target.ALL:
                //
                break;
            case Target.SOURCE:
                targets.Add(source);
                break;
            case Target.TARGET:
                targets.Add(target);
                break;
        };

        if (targetType == Target.RANDOM_NOT_SELF || targetType == Target.RANDOM_OPPONENTS || targetType == Target.RANDOM_ALLIES || targetType == Target.RANDOM) {
            return new List<Creature>(){targets[Random.Range(0, targets.Count)]};
        }

        return targets;
    }
}
