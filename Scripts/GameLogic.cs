using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameLogic : MonoBehaviour
{
    /*This script acts as the brains of the game by establishing waves, spawning the enemies based on what round
    they are on, determing when the rounds end, and communicating to other scenes at the end of the game*/
    
    public GameObject agentPrefab;
    public int dealthCount = 0;
    public int roundCount = 0;
    public int dealthCap = 0;
    public float roundCooldown = 0f;
    public Animator canvasAnimator;
    public TextMeshProUGUI textObject;

    // Start is called before the first frame update
    void Start()
    {
        //first round
        newRound();
    }

    // Update is called once per frame
    void Update()
    {
        //cooldown has started
        if(roundCooldown > 0f) {
            //increment cooldown timer
            roundCooldown += Time.deltaTime;
            //checks if cooldown time is over
            if(roundCooldown > 5f) {
                //spawn enemies based on round number
                for(int i=0; i<=roundCount/2; i++) {
                    Instantiate(agentPrefab, new Vector3(Random.Range(-7f, 7f), 2f, Random.Range(-7f, 7f)), Quaternion.identity);
                    dealthCap++;
                }
                //reset cooldown
                roundCooldown = 0f;
            }
        }

        //the round is over
        if(dealthCount == dealthCap && dealthCount > 0 && dealthCap > 0) {
            //reset variables and create new round
            newRound();
            dealthCount = 0;
            dealthCap = 0;
        }
    }

    public void newRound() {
        //create text animation
        canvasAnimator.SetTrigger("newRound");
        roundCount++;
        textObject.text = "Round " + roundCount;
        
        //start the round cooldown time of 5 seconds
        roundCooldown += Time.deltaTime;
    }
}
