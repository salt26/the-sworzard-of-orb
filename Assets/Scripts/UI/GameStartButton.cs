using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStartButton : MonoBehaviour {

    public Text korText;
    public Text engText;

    private bool isStart = false;

    void Awake()
    {
        korText.text = "이어하기";
        engText.text = "Continue Game";
        try
        {
            FileStream fs = new FileStream("Data.dat", FileMode.Open);
            fs.Close();
        }
        catch (FileNotFoundException)
        {
            korText.text = "게임 시작";
            engText.text = "Game Start";
        }
    }

    public void StartGame()
    {
        if (isStart) return;
        isStart = true;
        try
        {
            FileStream fs = new FileStream("Data.dat", FileMode.Open);
            fs.Close();
            SceneManager.LoadScene("Scenes/Town");
        }
        catch (FileNotFoundException)
        {
            SceneManager.LoadScene("Scenes/Tutorial");
        }
    }
}
