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

    public HoverInfo hoverInfo;

    [SerializeField] private AudioSource music;

    [SerializeField] private GameObject pauseScreen;

    void Start() {
        // this means that other classes can get a reference to Game without needing to be given one
        instance = this;

        // the game code supports multiple player classes (eg warrior, wizard etc)
        // these can have different max health, starting items, sprites etc
        // ...but we didnt end up doing this
        player = Instantiate(classes[0]);
        player.Initialise();
        
        LoadGameScene(GameScene.Map);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            TogglePause();
        }
    }

    public void TogglePause() {
        bool paused = !pauseScreen.activeSelf;
        
        pauseScreen.SetActive(paused);
        // pausing the game sets the timescale to 0
        // this freezes animations
        Time.timeScale = paused ? 0 : 1;
    }

    public void Quit() {
        // make sure time unpauses
        Time.timeScale = 1;

        if (currentScene != null) {
            SceneManager.UnloadSceneAsync(currentScene);
        }

        if (Map.Manager.instance != null) Map.Manager.instance.DestroyManager();
        // because the scene isnt loading additively, unity automatically unloads the Game scene
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadGameScene(GameScene scene) {
        // loads the specified scene and automatically unloads the last one
        
        string newScene = scene switch {
            GameScene.Encounter => "Encounter",
            GameScene.Map => "Map",
            GameScene.GameOver => "GameOver",
            GameScene.EncounterWon => "EncounterWon",
            GameScene.Shop => "Shop",
            _ => "Map",
        };

        // when returning to the map it needs to be updated (since the map scene doesnt fully unload)
        // likewise when leaving the map it needs to be hidden
        if (Map.Manager.instance != null) Map.Manager.instance.SetManagerActive(newScene == "Map");

        if (currentScene == newScene) return;

        if (currentScene != null) {
            SceneManager.UnloadSceneAsync(currentScene);
        }
        
        // clear up some things that cause issues if theyre not cleared when scenes load
        hoverInfo.HoverExit();
        Encounter.Manager.instance = null;


        SceneManager.LoadScene(newScene, LoadSceneMode.Additive);
        currentScene = newScene;
    }

    public void Play(AudioClip audioClip) {
        music.clip = audioClip;
        music.Play();
    }

    public enum GameScene {
        Encounter,
        Map,
        GameOver,
        EncounterWon,
        Shop
    }
}
