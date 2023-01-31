using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
   public GameObject target;
   public Vector3 offset;
   public float smoothSpeed = 0.03f;

   void Start()
   {
      offset = new Vector3(6.51000023f, 8.43999958f, -5.13999987f);
      target = GameObject.FindGameObjectWithTag("Player");
   }

   void LateUpdate()
   {
      Vector3 targetPosition = target.transform.position + offset;
      Vector3 smoothPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);

      transform.position = smoothPosition;
   }


}
