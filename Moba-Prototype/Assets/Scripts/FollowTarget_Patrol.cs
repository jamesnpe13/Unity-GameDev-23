using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FollowTarget_Patrol : MonoBehaviour
{
   private NavMeshAgent agent;
   public Transform bottom;
   private bool isWalking = false;
   private Animator animator;
   public float walkSpeed = 3f;
   private float animationWalkSpeedMultiplier = 0.15f;
   private Vector3 targetDestination;
   bool isFollowingEnemy = false;
   private bool mouseRightClick = false;
   public Transform destination_1;
   Vector3[] waypoints = new Vector3[2];


   void Start()
   {
      bottom = transform.Find("Bottom");
      agent = GetComponent<NavMeshAgent>();
      animator = GetComponent<Animator>();
      destination_1 = GameObject.Find("Waypoint 1").transform;
      waypoints[0] = destination_1.transform.position;

   }

   void Update()
   {
      // patrol  
      int waypoint = 0;

      if (agent.remainingDistance <= 0 && agent.hasPath == false)
      {
         agent.destination = waypoints[0];
      }






      // functions
      moveToDestination();
      setIsWalking();
      setAgentMovementParameters();
      handleAnimatorController();

      // =====================================================

      void moveToDestination()
      {
         Debug.DrawRay(bottom.transform.position, agent.destination - bottom.transform.position, Color.green);
      }

      void setIsWalking()
      {
         // set isWalking
         if (agent.velocity.magnitude == 0f)
         {
            isWalking = false;
         }
         else
         {
            isWalking = true;
         }
      }



      void setAgentMovementParameters()
      {
         // set NavMeshAgent parameters
         agent.speed = walkSpeed;
      }

      void handleAnimatorController()
      {
         // isWalking
         if (isWalking)
         {
            animator.SetBool("isWalking", true);
         }
         else if (!isWalking)
         {
            animator.SetBool("isWalking", false);
         }

         // set walk animation speed
         animator.SetFloat("walkSpeed", agent.velocity.magnitude * animationWalkSpeedMultiplier);
      }
   }
}
