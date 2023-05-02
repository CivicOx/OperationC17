using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndMenu : MonoBehaviour
{
    /*This script is called when the buttons at the end screen are hit, to which it updates the scene
    based on the selected button*/

    public void ToGame() {
        SceneManager.LoadScene("GameScene");
    }

    public void ToBeginning() {
        SceneManager.LoadScene("IntroMenu");
    }
}
