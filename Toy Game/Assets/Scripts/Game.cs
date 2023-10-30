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
        instance = this;
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
        Time.timeScale = paused ? 0 : 1;
    }

    public void Quit() {
        Time.timeScale = 1;

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
            GameScene.Shop => "Shop",
            _ => "Map",
        };

        if (Map.Manager.instance != null) Map.Manager.instance.SetManagerActive(newScene == "Map");

        if (currentScene == newScene) return;

        if (currentScene != null) {
            SceneManager.UnloadSceneAsync(currentScene);
        }
        
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
