using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuStuff : MonoBehaviour {
    public string targetSceneName;

    public void B_LoadScene() {
        SceneManager.LoadScene(targetSceneName);
    }

    public void B_QuitGame() {
        Application.Quit();
    }
}
