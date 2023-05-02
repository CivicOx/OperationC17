using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    /*This script is used once the player hits the start button to load the game*/
    
    public void ToGame() {
        SceneManager.LoadScene("GameScene");
    }
}
