using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Moba_Movement : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform bottom;
    public float walkSpeed = 6f;
    public bool isWalking = false;
    private Vector3 targetDestination;
    private bool mouseRightClick = false;
    [HideInInspector] private Vector3 worldMousePos;
    [SerializeField] private float impedanceThreshold = 2f;
    [SerializeField] private float playerToPlayerDistance = 1.5f;
    private bool playerLockon = false;
    private Transform enemyToFollow = null;
    private Transform oldTarget = null;
    bool isWaiting = false;
    [SerializeField] float stationaryWaitTime = 3f;
    private float waitTimer = 0f;




    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        bottom = transform.Find("Bottom");
    }

    void Update()
    {
        setAgentMovementParameters();
        handleMouseClicks();
        getWorldMousePosition();
        handleMovementImpedance();

        // if mouse is held down
        if (Input.GetMouseButton(1))
        {
            // initial click detect
            if (Input.GetMouseButtonDown(1))
            {
                if (enemyClicked() != null)
                {
                    configTarget(true, false);
                    oldTarget = enemyToFollow;
                    enemyToFollow = enemyClicked();
                    configTarget(false, true);
                }
                else if (enemyClicked() == null)
                {
                    configTarget(true, false);
                    enemyToFollow = null;
                }
            }

            // follow mouse while mouse right click down

            if (enemyClicked() == null)
            {
                targetDestination = worldMousePos;
                agent.destination = targetDestination;
            }


        }

        // follow enemy

        if (enemyToFollow != null)
        {
            float distanceP2P = (transform.position - enemyToFollow.transform.position).magnitude;

            bool enemyWithinProximity()
            {
                if (distanceP2P <= playerToPlayerDistance)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            // stop walking if too close to enemy

            if (enemyWithinProximity())
            {
                Debug.Log("within proximity");
                isWaiting = true;
                agent.ResetPath();
            }


            if (isWaiting)
            {
                if ((oldTarget == null && enemyToFollow != null) || oldTarget == enemyToFollow)
                {
                    delayedFollow();
                }

                if ((oldTarget != enemyToFollow) && oldTarget != null && enemyToFollow != null)
                {
                    followTarget();
                }
            }
            else
            {
                followTarget();
            }


            void delayedFollow()
            {
                waitTimer += Time.deltaTime;

                if (waitTimer >= stationaryWaitTime)
                {
                    waitTimer = 0;
                    followTarget();
                }

            }


            void followTarget()
            {

                targetDestination = enemyToFollow.transform.position;
                agent.destination = targetDestination;
                isWaiting = false;
            }

            // reset counter
            if (Input.GetMouseButtonDown(1) && (enemyToFollow != oldTarget))
            {
                waitTimer = 0;
            }
        }
        else if (enemyToFollow == null)
        {
            isWaiting = false;
        }

        Debug.Log(oldTarget);
        Debug.Log(enemyToFollow);
        Debug.Log(isWaiting);
        Debug.Log(enemyToFollow == oldTarget);
        Debug.Log(waitTimer);


    }

    void handleMouseClicks()
    {
        //  click handler

        if (Input.GetMouseButton(1))
        {
            mouseRightClick = true;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            mouseRightClick = false;
        }
    }

    void getWorldMousePosition()
    {
        // get  position on plane

        Plane dirPlane = new Plane(Vector3.up, bottom.transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        worldMousePos = bottom.transform.position;
        float enter = 0f;

        if (dirPlane.Raycast(ray, out enter))
        {
            worldMousePos = ray.GetPoint(enter);
            Vector3 dir = worldMousePos - bottom.transform.position;
            Vector3 direction = dir;
            direction.Normalize();

            Debug.DrawRay(bottom.transform.position, dir, Color.white);
            Debug.DrawRay(bottom.transform.position, agent.destination - bottom.transform.position, Color.green);
        }
    }

    private Transform enemyClicked()
    {
        Ray screenToWorldRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(screenToWorldRay, out RaycastHit screenToWorldHit) && screenToWorldHit.transform.tag == "Enemy")
        {
            return screenToWorldHit.transform;
        }
        else
        {
            return null;
        }
    }

    void configTarget(bool resetPath, bool outlines)
    {
        if (resetPath)
        {
            // reset path
            agent.ResetPath();
        }

        if (enemyToFollow != null)
        {
            if (outlines)
            {
                // enable outlines   
                enemyToFollow.GetComponent<Outline>().enabled = true;
            }
            else
            {
                // disable outlines     
                enemyToFollow.GetComponent<Outline>().enabled = false;
            }
        }
    }

    void handleMovementImpedance()
    {
        if (agent.velocity.magnitude < impedanceThreshold)
        {
            agent.ResetPath();
        }
    }

    void setAgentMovementParameters()
    {
        // set NavMeshAgent parameters
        agent.speed = walkSpeed;
    }


}
