using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outline_Script : MonoBehaviour
{
   [SerializeField] private Material outlineMaterial;
   [SerializeField] private float outlineScaleFactor;
   [SerializeField] private Color outlineColor;
   private Renderer outlineRenderer;

   void Start()
   {
      outlineRenderer = CreateOutline(outlineMaterial, outlineScaleFactor, outlineColor);
      outlineRenderer.enabled = true;
   }

   Renderer CreateOutline(Material outlineMat, float scaleFactor, Color color)
   {
      GameObject outlineObject = Instantiate(this.gameObject, transform.position, transform.rotation, transform);
      Renderer rend = outlineObject.GetComponent<Renderer>();

      rend.material = outlineMat;
      rend.material.SetColor("_OutlineColor", color);
      rend.material.SetFloat("_Scale", scaleFactor);
      rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

      outlineObject.GetComponent<Outline_Script>();
      outlineObject.GetComponent<Collider>();

      rend.enabled = false;

      return rend;
   }
}
