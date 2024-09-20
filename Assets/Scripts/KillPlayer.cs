﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class KillPlayer : MonoBehaviour {
    public string nextSceneName;
    public float delay = 5f;

    // Fade Animation after killed
    public GameObject fadeout;
    private bool playerInsideTrigger = false;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            playerInsideTrigger = true;
            fadeout.SetActive(true);
            Invoke("LoadNextScene", delay);
        }
    }

    private void LoadNextScene() {
        if (playerInsideTrigger) {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
