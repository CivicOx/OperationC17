using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerLogic : MonoBehaviour
{
    /*This script is mainly used in relation to player health, to which it determines the status of the
    player and will update health and change the scene once the player dies*/
    
    public float health = 100f;
    public Animator playerAnimator;
    public Animator canvasAnimator;
    public PlayerMovement playerScript;
    public Slider healthSlider;
    public float healthTimer;

    // Update is called once per frame
    void Update()
    {
        canvasAnimator.SetBool("IsLow", health <= 20f);
        healthTimer += Time.deltaTime;
        //update health every 5 seconds
        if(healthTimer > 5f) {
            if(health < 100f) {
                if(health < 95f) {
                    health += 5f;
                } else {
                    health = 100f;
                }
            }
            //reset health timer + change health slider
            healthTimer = 0f;
            healthSlider.value = health;
        }

    }

    public void PlayerHit(float damage) {
        //player takes damage
        health -= damage;
        Debug.Log("test");
        healthSlider.value = health;
        canvasAnimator.SetTrigger("Canvas_Shake");
        //player dead
        if(health <= 0f) {
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene("DeadMenu");
        }
        //player took heavy hit
        if(damage > 10f)  {
            playerAnimator.SetTrigger("isHit");
            playerScript.hitCooldown = 1f;
        }
    }
}
