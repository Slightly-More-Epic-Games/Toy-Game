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

    //todo: put this into map manager
    public Map.Node node;

    void Start() {
        instance = this;
        player = Instantiate(classes[0]);
        player.Initialise();
        //todo: cost should increase by the players spawncost each level
        node.Initialise(2f);
        LoadGameScene(GameScene.Map);
    }

    public void Quit() {
        if (currentScene != null) {
            SceneManager.UnloadSceneAsync(currentScene);
        }

        Map.Manager.instance?.DestroyManager();
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadSceneIntDontUseThisMethod(int scene) {
        if (scene == 1) LoadGameScene(GameScene.Encounter);
        else LoadGameScene(GameScene.Map);
    }

    public void LoadGameScene(GameScene scene) {
        string newScene = scene switch {
            GameScene.Encounter => "Encounter",
            GameScene.Map => "Map",
            _ => "Map",
        };

        Map.Manager.instance?.SetManagerActive(newScene == "Map");

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
        Map
    }
}
