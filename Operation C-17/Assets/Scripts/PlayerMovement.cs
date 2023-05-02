using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    /*This script is used to control the player movement, animations, and communicates to other scripts
    such as the PlayerLogic script in regards to what is happening within the scene. This script features
    most of the fundamental mechanics of the game and works directly with player input*/
    
    public CharacterController controller;

    public float speed = 6f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    public bool canBeHit = true;

    public float dashCooldown = 0f;
    Vector3 dashVector;
    public bool isDashing;

    bool isGrounded;
    public bool isInAir;

    public bool isRunning;
    public bool isSprinting;

    public bool isBlocking;
    public float blockTime = 0f;

    public float hitCooldown = 0f;

    public bool isPunching;
    public float punchCooldown = 0f;
    Vector3 punchVector;
    public bool shootRay;
    public float punched;
    
    bool dashAnim;
    bool fallAnim;
    bool switchPunch;

    public Animator playerAnimator;

    Vector3 velocity;

    // Update is called once per frame
    void Update()
    {
        //movement vector
        Vector3 move = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");
        
        //punch
        if(Input.GetMouseButtonDown(0) && !isDashing && isGrounded && !isPunching && !isBlocking && hitCooldown <= 0f) {
            isPunching = true;
            playerAnimator.SetTrigger("isPunching");
            punchCooldown = .66f;
        }

        //while punching
        if(punchCooldown > 0f) {
            //decrease punch cooldown
            punchCooldown -= Time.deltaTime;
            if(punchCooldown < .2f && Input.GetMouseButtonDown(0) && !switchPunch) {
                switchPunch = true;
            }
            //stop punching
            if(punchCooldown <= 0f) {
                punchCooldown = 0f;
                if(!switchPunch) {
                    playerAnimator.SetTrigger("stopPunching");
                    isPunching = false;
                } else {
                    playerAnimator.SetTrigger("switchPunch");
                    switchPunch = false;
                    punchCooldown = .66f;
                }
            }
        }

        RaycastHit hit;

        if(shootRay) {
            if(Physics.Raycast(transform.position, transform.forward, out hit, 1.5f) && punched <= 0f) {
                if(hit.transform.gameObject.tag == "Agent") {
                    hit.transform.gameObject.GetComponent<AgentNavMesh>().Hit(30f);
                }
                punched = .5f;
            }
        }

        if(punched > 0f) {
            punched -= Time.deltaTime;
        }

        if(hitCooldown > 0f) {
            hitCooldown -= Time.deltaTime;
        }

        //dash input
        if(Input.GetButtonDown("Dash") && dashCooldown == 0f && move != Vector3.zero && isGrounded && !isPunching && !isBlocking && hitCooldown <= 0f) {
            dashCooldown = .7f;
            dashVector = move * 20;
            dashAnim = true;
        }

        //block input
        if(Input.GetButtonDown("Block") && punchCooldown <= 0f && isGrounded && !isDashing && !isBlocking && hitCooldown <= 0f) {
            isBlocking = true;
            blockTime += .01f;
            playerAnimator.SetTrigger("startBlocking");
        }
        
        if(Input.GetButtonUp("Block") && isBlocking) {
            isBlocking = false;
            blockTime = 0f;
            playerAnimator.SetTrigger("stopBlocking");
        }

        if(blockTime > 0f)
            blockTime += Time.deltaTime;


        //use isGrounded for isInAir
        isInAir = !isGrounded;
        
        //ground detection
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        
        //sprint input
        if(Input.GetButton("Sprint")) {
            speed = 10f;
        } else {
            speed = 6f;
        }

        //check dash
        if(dashCooldown > 0f) {
            //dashing (invincible)
            if(dashCooldown < .4f) {
                controller.Move(dashVector * Time.deltaTime);
            }
            dashCooldown -= Time.deltaTime;
            canBeHit = false;
            isDashing = true;
        } else {
            //not dashing (vulnerable)
            dashCooldown = 0f;
            canBeHit = true;
            isDashing = false;
        }

        //jump if grounded
        if(Input.GetButtonDown("Jump") && isGrounded && !isPunching && !isBlocking && hitCooldown <= 0f) {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            fallAnim = true;
        }

        //default velocity if grounded
        if(isGrounded && velocity.y < 0)
            velocity.y = -2f;

        //x-z movement
        if(canBeHit) {
            controller.Move(move * speed * Time.deltaTime);
            //set running/sprinting to false if not moving
            if(move == Vector3.zero) {
                isRunning = false;
                isSprinting = false;
            } else if(speed < 8f) {
                //running
                isRunning = true;
                isSprinting = false;
            } else {
                //sprinting
                isSprinting = true;
                isRunning = false;
            }
        }

        //overide running/sprinting if dashing
        if(!canBeHit) {
            isRunning = false;
            isSprinting = false;
        }

        //y movement w/ acceleration
        velocity.y += gravity * 2 * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);


        //animations
        playerAnimator.SetBool("isRunning", isRunning);
        playerAnimator.SetBool("isSprinting", isSprinting);
        playerAnimator.SetBool("isGrounded", isGrounded);
        playerAnimator.SetBool("isBlocking", isBlocking);

        if(isInAir && fallAnim) {
            playerAnimator.SetTrigger("isFalling");
            fallAnim = false;
        }

        if(isDashing && dashAnim) {
            playerAnimator.SetTrigger("isDashing");
            dashAnim = false;
        }
    }
}
