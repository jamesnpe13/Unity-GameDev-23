using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement_NavMeshAgent_Raycast : MonoBehaviour
{
   private NavMeshAgent agent;
   public Transform bottom;
   private bool isWalking = false;
   private Animator animator;
   public float walkSpeed = 6f;
   private float animationWalkSpeedMultiplier = 0.15f;
   private Vector3 targetDestination;
   bool isFollowingEnemy = false;
   public Transform enemyToFollow = null;
   private bool mouseRightClick = false;

   void Start()
   {
      bottom = transform.Find("Bottom");
      agent = GetComponent<NavMeshAgent>();
      animator = GetComponent<Animator>();
      targetDestination = bottom.transform.position;
   }

   void Update()
   {
      // mouse click switch
      if (Input.GetMouseButtonDown(1))
      {
         mouseRightClick = true;
      }
      else if (Input.GetMouseButtonUp(1))
      {
         mouseRightClick = false;
      }




      // get mouse position in world
      Plane dirPlane = new Plane(Vector3.up, bottom.transform.position);
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      Vector3 hitPoint = bottom.transform.position;
      float enter = 0f;

      if (dirPlane.Raycast(ray, out enter))
      {
         hitPoint = ray.GetPoint(enter);
         Vector3 dir = hitPoint - bottom.transform.position;
         Vector3 direction = dir;
         direction.Normalize();

         Debug.DrawRay(bottom.transform.position, dir, Color.white);
      }

      // functions
      moveToDestination();
      lookAtDestination();
      setIsWalking();
      setAgentMovementParameters();
      handleAnimatorController();

      // =====================================================

      void moveToDestination()
      {
         float stationaryThreshold = 2f;


         // follow enemy on right click
         Ray screenToWorldRay = Camera.main.ScreenPointToRay(Input.mousePosition);

         if (mouseRightClick && Physics.Raycast(screenToWorldRay, out RaycastHit screenToWorldHit))
         {
            if (screenToWorldHit.transform.tag == "Enemy")
            {
               if (enemyToFollow != null)
               {
                  enemyToFollow.GetComponent<Outline>().enabled = false;
               }

               enemyToFollow = screenToWorldHit.transform;
               isFollowingEnemy = true;
               mouseRightClick = false;
            }
         }

         if (isFollowingEnemy)
         {
            enemyToFollow.GetComponent<Outline>().enabled = true;
            targetDestination = enemyToFollow.transform.position;
         }

         // set destination to mouse position or nearest obstacle
         Ray nearestObstacleRay = new Ray(bottom.transform.position, hitPoint - bottom.transform.position);

         if (mouseRightClick)
         {
            isFollowingEnemy = false;
            if (enemyToFollow != null)
            {
               enemyToFollow.GetComponent<Outline>().enabled = false;
               enemyToFollow = null;
            }


            if (Physics.Raycast(nearestObstacleRay, out RaycastHit hit))
            {
               isFollowingEnemy = false;
               float distanceToMouse = (hitPoint - bottom.transform.position).magnitude;
               float distanceToObstacle = (hit.point - bottom.transform.position).magnitude;

               if (distanceToObstacle < distanceToMouse)
               {
                  targetDestination = hit.point;
               }
               else
               {
                  targetDestination = hitPoint;
               }
            }
            else
            {
               targetDestination = hitPoint;
            }


         }
         agent.destination = targetDestination;
         Debug.Log(enemyToFollow);
         Debug.DrawRay(bottom.transform.position, agent.destination - bottom.transform.position, Color.green);

         // clears destination if agent cannot move

         if (Input.GetMouseButton(1) == false)
         {
            if (agent.velocity.magnitude <= stationaryThreshold)
            {
               agent.ResetPath();
            }

         }
      }

      void lookAtDestination()
      {

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
