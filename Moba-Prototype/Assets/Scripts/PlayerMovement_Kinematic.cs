using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement_Kinematic : MonoBehaviour
{
   public Transform bottom;
   private Vector3 mousePos;
   private Vector3 direction;
   public float movementSpeed = 7f;
   private Rigidbody rb;

   void Start()
   {
      rb = GetComponent<Rigidbody>();
   }

   void Update()
   {
      Plane navPlane = new Plane(Vector3.up, bottom.transform.position);
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      float enter = 0f;
      Vector3 hitPoint = bottom.transform.position; ;

      if (navPlane.Raycast(ray, out enter))
      {
         hitPoint = ray.GetPoint(enter);
         mousePos = hitPoint;
         direction = hitPoint - bottom.transform.position;
         direction.Normalize();
      }

      // look at mouse
      lookAtMouse();

      //  click and move
      if (Input.GetMouseButton(1))
      {
         movePlayer();
      }

      Debug.DrawRay(bottom.transform.position, direction, Color.white);
   }

   void FixedUpdate()
   {

   }

   void movePlayer()
   {
      transform.Translate(direction * Time.deltaTime * movementSpeed, Space.World);
   }

   void lookAtMouse()
   {
      transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
   }


}


