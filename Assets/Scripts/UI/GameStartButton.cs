using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStartButton : MonoBehaviour {

    private bool isStart = false;

	public void StartGame()
    {
        if (isStart) return;
        isStart = true;
        SceneManager.LoadScene("Scenes/Tutorial");
    }
}
