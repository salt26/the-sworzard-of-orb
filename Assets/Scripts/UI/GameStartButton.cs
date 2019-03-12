using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
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
            FileStream fs = new FileStream(@"Data.dat", FileMode.Open);
            fs.Close();
        }
        catch (System.Exception e)
        {
            if (e is FileNotFoundException || e is IsolatedStorageException)
            {
                korText.text = "게임 시작";
                engText.text = "Game Start";
            }
            else
            {
                engText.text = e.GetType().ToString() + "\n" + e.Message;
                throw;
            }
        }
    }

    public void StartGame()
    {
        if (isStart) return;
        isStart = true;
        try
        {
            FileStream fs = new FileStream(@"Data.dat", FileMode.Open);
            fs.Close();
            SceneManager.LoadScene("Scenes/Town");
        }
        catch (System.Exception e)
        {
            if (e is FileNotFoundException || e is IsolatedStorageException)
                SceneManager.LoadScene("Scenes/Tutorial");
            else throw;
        }
    }
}
