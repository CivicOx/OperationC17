using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class AgentNavMesh : MonoBehaviour
{
    /*This is the main script for the enemy objects, to which is relates to how the enemy navigates the play
    area via a navmesh, works with enemy health, and animates the enemy objects based on what is happening
    in the game*/
    
    public NavMeshAgent navMeshAgent;
    public Transform playerPosition;
    public float walkRadius = 10f;
    public Slider healthSlider;

    public float walkSpeed = 1.5f;
    public bool walking = false;

    public float runSpeed = 3.5f;
    public bool running = false;
    public bool fighting = false;
    public bool dodging = false;
    public float stumbling;
    public float health = 100f;
    public bool punchMoment;
    public float hitCooldown = 0f;

    public float punchReset = .8f;
    public float dodgeReset = .7f;
    public float critTime = .3f;

    public float dodgeTime = 0f;
    public float dodgeCooldown = 0f;
    
    public float punchCooldown = 0f;

    public Animator agentAnimator;
    public Animator playerAnimator;

    public PlayerMovement playerScript;
    public PlayerLogic logicScript;
    public GameLogic gameScript;

    NavMeshHit navHit;

    private float moveCounter;
    
    // Update is called once per frame
    void Update()
    {
        GameObject playerObject = GameObject.Find("Player");
        playerPosition = playerObject.GetComponent<Transform>();
        playerAnimator = playerObject.GetComponent<Animator>();
        playerScript = playerObject.GetComponent<PlayerMovement>();
        logicScript = playerObject.GetComponent<PlayerLogic>();
        gameScript = GameObject.Find("GameLogic").GetComponent<GameLogic>();
        
        if(moveCounter < 0f && !fighting && stumbling <= 0f) {
            moveCounter = Random.Range(5f, 10f);

            //get random position
            Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
            randomDirection += transform.position;
            NavMesh.SamplePosition(randomDirection, out navHit, walkRadius, 1);

            //set destination to random position
            navMeshAgent.destination = navHit.position;
        }

        if(stumbling > 0f) {
            stumbling -= Time.deltaTime;
            transform.position = transform.position + transform.forward * -2f * Time.deltaTime;
            navMeshAgent.destination = transform.position;
        }
        
        //check dodge condition
        if(playerScript.punchCooldown > .5 && punchCooldown <= 0f) {
            dodging = true;        
        } else {
            dodging = false;
        }

        //check for punch
        if(stumbling <= 0f) {
            if(punchCooldown > 0f) {
                //currently on cooldown
                punchCooldown -= Time.deltaTime;
            //in range to punch
            } else if(Vector3.Distance(transform.position, playerPosition.position) < 2f && dodgeCooldown <= 0f) {
                //punch
                agentAnimator.SetTrigger("Punch");
                punchCooldown = punchReset;
            }
        }

        //agent punching
        if(punchMoment && hitCooldown <= 0f) {
            hitCooldown = .2f;
            if(Vector3.Distance(transform.position, playerPosition.position) < 2.3f) {
                Debug.Log("test");
                if(playerScript.isBlocking) {
                    //punch blocked
                    if(playerScript.blockTime < critTime) {
                        //crit
                        playerAnimator.SetTrigger("isCrit");
                        playerScript.isPunching = true;
                        playerScript.punchCooldown = .66f;
                        Hit(20f);
                    } else {
                        //blocked
                        logicScript.PlayerHit(5f);
                    }
                } else {
                    //not blocked, full damage
                    logicScript.PlayerHit(15f);
                }
            }
        }

        if(hitCooldown > 0f) {
            hitCooldown -= Time.deltaTime;
        }
        
        //update walk counter every frame
        moveCounter -= Time.deltaTime;

        if(Vector3.Distance(transform.position, playerPosition.position) < 10f && stumbling <= 0f) {
            fighting = true;
            running = true;
            //run after player or dodge
            if(Vector3.Distance(transform.position, playerPosition.position) < 3f && dodging) {
                //dodge
                if(dodgeCooldown <= 0f) {
                    agentAnimator.SetTrigger("Dodge");
                    dodgeCooldown = dodgeReset;
                }
            } else {
                //run after player
                if(Vector3.Distance(transform.position, playerPosition.position) < 1.5f) {
                    navMeshAgent.destination = transform.position;
                    transform.LookAt(playerPosition);
                    transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, transform.eulerAngles.z);
                } else {
                    navMeshAgent.destination = playerPosition.position;
                    navMeshAgent.speed = runSpeed;
                }
            }

            //fighting crap

        } else {
            fighting = false;
            running = false;
        }

        if(dodgeCooldown > 0f) {
            dodgeCooldown -= Time.deltaTime;
        }

        //check if agent is walking based on if they are at the destination position
        if(Vector3.Distance(transform.position, navHit.position) < 0.1 || fighting) {
            walking = false;
        } else {
            walking = true;
        }

        agentAnimator.SetBool("Running", running);

        agentAnimator.SetBool("Walking", walking);
        if(walking)
            navMeshAgent.speed = walkSpeed;

    }

    public void Hit(float damage) {
        if(dodgeCooldown <= 0f && stumbling <= 0f) {
            health -= damage;
            healthSlider.value = health;
            if(health <= 0f) {
                gameScript.dealthCount++;
                GameObject.Destroy(gameObject);
            }
            Debug.Log("Agent Hit: " + damage);
            if(health <= 0f) {
                //dead
            } else if(damage >= 20f) {
                 //stumble
                agentAnimator.SetTrigger("Stumble");
                stumbling = .5f;
            }
        }
        
    }
}
