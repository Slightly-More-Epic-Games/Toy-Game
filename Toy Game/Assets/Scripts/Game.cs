using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    [SerializeField] private PlayerData[] classes;

    [System.NonSerialized] public PlayerData playerData;

    private string currentScene = null;

    public static Game instance;

    private Queue<Context> eventsToProcess = new Queue<Context>();

    public List<Creature> playerAllies = new List<Creature>();
    public List<Creature> playerEnemies = new List<Creature>();

    void Start() {
        instance = this;
        playerData = Instantiate(classes[0]);
        LoadGameScene(GameScene.MAP);

        playerAllies.Add(playerData);
        //wow a change
        //wow another change!
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
        }
    }

    public void AddEventToProcess(Context context) {
        eventsToProcess.Enqueue(context);
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
