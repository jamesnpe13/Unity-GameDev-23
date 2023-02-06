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
   private bool isTraveling = false;
   private enum target
   {
      mouse,
      newEnemy,
      sameEnemy,
      standby
   }
   [SerializeField] private target tg;
   private enum proximityState
   {
      withinProximity,
      leavingProximity,
      notWithinProximity,
      standby
   }
   private proximityState ps;


   void Start()
   {
      agent = GetComponent<NavMeshAgent>();
      bottom = transform.Find("Bottom");
      tg = target.standby;
      ps = proximityState.standby;

   }

   void Update()
   {
      setAgentMovementParameters();
      getWorldMousePosition();
      // handleMovementImpedance();

      // movement and targeting states
      switch (tg)
      {
         case target.mouse:
            if (Input.GetMouseButton(1))
            {
               targetDestination = worldMousePos;
            }
            move();

            if (oldTarget != null) oldTarget.GetComponent<Outline>().enabled = false;
            if (newTarget != null) newTarget.GetComponent<Outline>().enabled = false;
            break;

         case target.newEnemy:
            targetDestination = newTarget.transform.position;
            move();

            if (oldTarget != null) oldTarget.GetComponent<Outline>().enabled = false;
            if (newTarget != null) newTarget.GetComponent<Outline>().enabled = true;
            break;

         case target.sameEnemy:
            targetDestination = newTarget.transform.position;
            move();

            if (oldTarget != null) oldTarget.GetComponent<Outline>().enabled = false;
            if (newTarget != null) newTarget.GetComponent<Outline>().enabled = true;
            break;

         case target.standby:
            agent.ResetPath();

            if (oldTarget != null) oldTarget.GetComponent<Outline>().enabled = false;
            if (newTarget != null) newTarget.GetComponent<Outline>().enabled = false;
            break;
      }

      if (Input.GetMouseButtonDown(1))
      {
         oldTarget = newTarget;

         if (enemyClicked() == null)
         {
            newTarget = null;
            tg = target.mouse;
         }
         else if (enemyClicked() != null)
         {
            newTarget = enemyClicked();

            if (enemyClicked() == oldTarget)
            {
               tg = target.sameEnemy;
            }
            else if (enemyClicked() != oldTarget)
            {
               if (oldTarget != null)
               {
                  tg = target.newEnemy;
               }
            }
            Debug.Log(tg);
         }
      }

      void move()
      {
         agent.destination = targetDestination;

         // proximity check and delay resume

         if (tg == target.mouse)
         {
            ps = proximityState.standby;
         }
         else if (tg == target.newEnemy)
         {
            if ((newTarget.transform.position - transform.position).magnitude <= playerToPlayerDistance)
            {
               ps = proximityState.withinProximity;
               tg = target.sameEnemy;
            }

            else if ((newTarget.transform.position - transform.position).magnitude > playerToPlayerDistance)
            {
               ps = proximityState.notWithinProximity;
               tg = target.sameEnemy;
            }
         }
         else if (tg == target.sameEnemy)
         {
            if ((newTarget.transform.position - transform.position).magnitude <= playerToPlayerDistance)
            {
               ps = proximityState.withinProximity;
            }

            else if ((newTarget.transform.position - transform.position).magnitude > playerToPlayerDistance)
            {
               if (agent.isStopped)
               {
                  ps = proximityState.leavingProximity;
               }
               else if (!agent.isStopped)
               {
                  ps = proximityState.notWithinProximity;
               }
            }
         }

         switch (ps)
         {
            case proximityState.standby:
               agent.isStopped = false;
               break;

            case proximityState.withinProximity:
               agent.isStopped = true;
               break;

            case proximityState.leavingProximity:

               // delay

               waitTimer += Time.deltaTime;

               if (waitTimer >= stationaryWaitTime)
               {
                  waitTimer = 0;
                  agent.isStopped = false;
               }

               break;

            case proximityState.notWithinProximity:
               agent.isStopped = false;
               break;
         }
      }

      // Debug.Log(tg);
      // Debug.Log(ps);
   }




   Transform enemyClicked()
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

   void customFunction()
   {
   }

}

