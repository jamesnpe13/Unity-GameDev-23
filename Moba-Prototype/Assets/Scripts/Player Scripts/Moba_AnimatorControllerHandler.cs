using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Moba_AnimatorControllerHandler : MonoBehaviour
{
   private NavMeshAgent agent;
   private bool isWalking;
   private Animator animator;
   [SerializeField] private float animationWalkSpeedMultiplier = 0.15f;

   void Start()
   {
      agent = GetComponent<NavMeshAgent>();
      animator = GetComponent<Animator>();
   }

   void Update()
   {
      setIsWalking();
      handleAnimatorController();
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
