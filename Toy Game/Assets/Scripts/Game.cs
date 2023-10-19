using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    [SerializeField] private PlayerCreature[] classes;
    [SerializeField] private AutoCreature[] enemies;

    [System.NonSerialized] public PlayerCreature player;

    private string currentScene = null;

    public static Game instance;

    [System.NonSerialized] public Map.Node node;

    void Start() {
        instance = this;
        player = Instantiate(classes[0]);
        node = new Map.Encounter {
            enemies = new List<Creature>() { enemies[Random.Range(0, enemies.Length)] }
        };
        LoadGameScene(GameScene.MAP);
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

        Map.Manager.instance = null;
        Encounter.Manager.instance = null;

        SceneManager.LoadScene(newScene, LoadSceneMode.Additive);
        currentScene = newScene;
    }

    public enum GameScene {
        ENCOUNTER,
        MAP
    }
}
