using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement_NavMeshAgent : MonoBehaviour
{
   private NavMeshAgent agent;
   public Transform bottom;
   private bool isWalking = false;
   private Animator animator;
   public float walkSpeed = 6f;
   private float animationWalkSpeedMultiplier = 0.15f;

   void Start()
   {
      agent = GetComponent<NavMeshAgent>();
      animator = GetComponent<Animator>();
   }

   void Update()
   {
      Plane dirPlane = new Plane(Vector3.up, bottom.transform.position);
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      Vector3 hitPoint = bottom.transform.position;
      float enter = 0f;

      // get mouse position in world
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

         if (Input.GetMouseButton(1))
         {
            agent.destination = hitPoint;
         }

         // clears destination if agent cannot move
         
         if (Input.GetMouseButton(1) == false)
         {
            if (agent.velocity.magnitude <= stationaryThreshold)
            {
               agent.ResetPath();

               // Debug.Log("Path cleared");
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
