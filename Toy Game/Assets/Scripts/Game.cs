using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    [SerializeField] private PlayerCreature[] classes;

    [System.NonSerialized] public PlayerCreature player;

    private string currentScene = null;

    public static Game instance;

    void Start() {
        instance = this;
        player = Instantiate(classes[0]);
        player.Initialise();
        LoadGameScene(GameScene.Map);
    }

    public void Quit() {
        if (currentScene != null) {
            SceneManager.UnloadSceneAsync(currentScene);
        }

        if (Map.Manager.instance != null) Map.Manager.instance.DestroyManager();
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadGameScene(GameScene scene) {
        string newScene = scene switch {
            GameScene.Encounter => "Encounter",
            GameScene.Map => "Map",
            GameScene.GameOver => "GameOver",
            GameScene.EncounterWon => "EncounterWon",
            _ => "Map",
        };

        if (Map.Manager.instance != null) Map.Manager.instance.SetManagerActive(newScene == "Map");

        if (currentScene == newScene) return;

        if (currentScene != null) {
            SceneManager.UnloadSceneAsync(currentScene);
        }

        Encounter.Manager.instance = null;

        SceneManager.LoadScene(newScene, LoadSceneMode.Additive);
        currentScene = newScene;
    }

    public enum GameScene {
        Encounter,
        Map,
        GameOver,
        EncounterWon
    }
}
