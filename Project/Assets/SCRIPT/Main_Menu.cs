using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_Menu : MonoBehaviour {

	public void PlayGame ()
    {
        SceneManager.LoadScene("GAME");
    }

    public void EndGame()
    {
        SceneManager.LoadScene("SCHERMATE_MENU");
    }
}
