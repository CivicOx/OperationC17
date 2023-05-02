using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextScript : MonoBehaviour
{
    /*This script changes the next of the new round animation*/
    
    public bool changeText;
    public TextMeshProUGUI textObj;
   
    // Update is called once per frame
    void Update()
    {
        if(changeText) {
            textObj.text = "FIGHT!";
            changeText = false;
        }
    }
}
