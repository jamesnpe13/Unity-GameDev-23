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
   private Transform newTarget = null;
   private Transform oldTarget = null;
   bool isWaiting = false;
   [SerializeField] float stationaryWaitTime = 3f;
   private float waitTimer = 0f;
   private bool selectionReset = true;
   private bool mouseHeld = false;
   private bool isFollowing = false;

   void Start()
   {
      agent = GetComponent<NavMeshAgent>();
      bottom = transform.Find("Bottom");
   }

   void Update()
   {
      setAgentMovementParameters();
      // handleMouseClicks();
      getWorldMousePosition();
      // handleMovementImpedance();

      if (selectionReset)
      {
         // check target
         if (Input.GetMouseButtonDown(1))
         {
            if (enemyClicked() != null)
            {
               selectionBehavior(true, false, true);
               oldTarget = newTarget;
               newTarget = enemyClicked();
               targetOutline();

            }
            else if (enemyClicked() == null)
            {
               selectionBehavior(false, true, false);
            }

            Debug.Log("OLD: " + oldTarget);
            Debug.Log("NEW: " + newTarget);
         }
         // follow enemy
         if (isFollowing)
         {
            targetDestination = newTarget.transform.position;
            agentMoving(true, newTarget.transform.position);
         }
      }
      else if (!selectionReset)
      {
         // follow mouse
         if (mouseHeld)
         {
            oldTarget = newTarget;
            newTarget = enemyClicked();

            if (enemyClicked() == null) {
               targetOutline();
            }

            agentMoving(true, worldMousePos);

            if (Input.GetMouseButtonUp(1))
            {
               selectionBehavior(true, false, false);
            }
         }
         else if (!mouseHeld)
         {
            agentMoving(true, worldMousePos);
            selectionBehavior(true, false, false);
         }
      }

      // if (Input.GetMouseButtonUp(1))
      // {
      //    selectionBehavior(true, false, false);
      // }

      void selectionBehavior(bool _selectionReset, bool _mouseHeld, bool _isFollowing)
      {
         selectionReset = _selectionReset;
         mouseHeld = _mouseHeld;
         isFollowing = _isFollowing;
      }

      void agentMoving(bool isMoving, Vector3 target)
      {
         if (isMoving)
         {
            agent.destination = target;
         }
         else if (!isMoving)
         {
            agent.ResetPath();
         }
      }

      void targetOutline()
      {
         if (oldTarget != null)
         {
            oldTarget.GetComponent<Outline>().enabled = false;
         }

         if (newTarget != null)
         {
            newTarget.GetComponent<Outline>().enabled = true;
         }


         if (oldTarget == newTarget && oldTarget != null && newTarget != null)
         {
            newTarget.GetComponent<Outline>().enabled = true;
         }


      }

      // follow enemy

      if (newTarget != null)
      {


         bool enemyWithinProximity(Transform target)
         {
            float distanceP2P = (transform.position - target.transform.position).magnitude;

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





         void stop()
         {
            agent.isStopped = true;
         }

         void delay()
         {
            waitTimer += Time.deltaTime;

            if (waitTimer >= stationaryWaitTime)
            {
               waitTimer = 0;
               agent.isStopped = false;
            }
         }

         void go()
         {
            agent.isStopped = false;
         }









         // reset counter
         if (Input.GetMouseButtonDown(1) && (newTarget != oldTarget))
         {
            waitTimer = 0;
         }
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
