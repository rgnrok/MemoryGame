using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartGameButton : MonoBehaviour {

    public Toggle playOnTime;
    private Button button;

    void Awake() {
        button = GetComponent<Button>();
    }

    public void StartGame() {
        button.interactable = false;
        PlayerPrefs.SetInt("playOnTime", playOnTime.isOn ? 1 : 0);
        SceneManager.LoadScene("GameCanvas");
    }

}

