using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class Game : MonoBehaviour
{
    [SerializeField] private PlayerCreature[] classes;
    [SerializeField] private AutoCreature[] enemies;

    [System.NonSerialized] public PlayerCreature player;

    private string currentScene = null;

    public static Game instance;

    private Queue<Context> eventsToProcess = new Queue<Context>();

    [System.NonSerialized] public List<Creature> playerAllies = new List<Creature>();
    [System.NonSerialized] public List<Creature> playerEnemies = new List<Creature>();

    private Creature currentTurn;
    private int turnNumber;

    void Start() {
        instance = this;
        player = Instantiate(classes[0]);
        LoadGameScene(GameScene.MAP);

        playerAllies.Add(player);

        playerEnemies.Add(Instantiate(enemies[Random.Range(0, enemies.Length)]));
    }

    public void ProcessEvents() {
        while (eventsToProcess.Count > 0) {
            Context context = eventsToProcess.Dequeue();
            foreach (Creature creature in playerAllies) {
                creature.OnEvent(context);
            }
            foreach (Creature creature in playerEnemies) {
                creature.OnEvent(context);
            }

            if (context.action == Context.Action.TURN_END) {
                TurnEnd(context.target);
            }
        }
        foreach (Creature creature in playerAllies) {
            creature.EventFinished();
        }
        foreach (Creature creature in playerEnemies) {
            creature.EventFinished();
        }
    }

    private void TurnEnd(Creature newCurrent) {
        currentTurn = newCurrent;
        turnNumber++;
        playerAllies.RemoveAll(x => x.IsDead());
        playerEnemies.RemoveAll(x => x.IsDead());

        if (player.IsDead()) {
            Debug.Log("GAME OVER");
        }

        if (playerEnemies.Count == 0) {
            Debug.Log("ENCOUNTER WON");
        }
    }

    public void AddEventToProcess(Context context) {
        if (context.action == Context.Action.TURN_END) {
            context.source = currentTurn;
            context.value = turnNumber;
            context.target = GetNextCreatureInTurnOrder(currentTurn);
            eventsToProcess.Enqueue(context);
            eventsToProcess.Enqueue(new Context(Context.Action.TURN_START, context.source, context.target, turnNumber+1));
        } else if (context.action == Context.Action.TURN_START) {
            if (currentTurn == context.target) return;
            eventsToProcess.Enqueue(new Context(Context.Action.TURN_END, currentTurn, context.target, turnNumber));
            context.value = turnNumber+1;
            eventsToProcess.Enqueue(context);
        } else {
            eventsToProcess.Enqueue(context);
        }
    }

    private Creature GetNextCreatureInTurnOrder(Creature current) {
        int index = playerAllies.Contains(current) ? playerAllies.IndexOf(current) : playerEnemies.IndexOf(current)+playerAllies.Count;
        index = (index + 1)%(playerAllies.Count+playerEnemies.Count);
        return index >= playerAllies.Count ? playerEnemies[index-playerAllies.Count] : playerAllies[index];
    }

    public void Quit() {
        if (currentScene != null) {
            SceneManager.UnloadSceneAsync(currentScene);
        }

        SceneManager.LoadScene("MainMenu");
    }

    public void LoadSceneIntDontUseThisMethod(int scene) {
        if (scene == 1) LoadGameScene(GameScene.ENCOUNTER);
        else LoadGameScene(GameScene.MAP);
    }

    public void LoadGameScene(GameScene scene) {
        string newScene = scene switch {
            GameScene.ENCOUNTER => "Encounter",
            GameScene.MAP => "Map",
            _ => "Map",
        };

        if (currentScene == newScene) return;

        if (currentScene != null) {
            SceneManager.UnloadSceneAsync(currentScene);
        }

        SceneManager.LoadScene(newScene, LoadSceneMode.Additive);
        currentScene = newScene;
    }

    public enum GameScene {
        ENCOUNTER,
        MAP
    }
}
