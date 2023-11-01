using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject standaloneButtons;
    [SerializeField] private GameObject webglButtons;

    private void Awake() {
        #if UNITY_WEBGL 
        webglButtons.SetActive(true);
        standaloneButtons.SetActive(false);
        #else
        webglButtons.SetActive(false);
        standaloneButtons.SetActive(true);
        #endif
    }

    public void LoadGame() {
        SceneManager.LoadScene("Game");
    }

    public void CloseGame() {
        Application.Quit();
    }
}
