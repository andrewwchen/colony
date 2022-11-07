using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Block : MonoBehaviour
{
    public Ray ray;
    void Update()
    {
        
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null)
            {
                var rend = hit.collider.GetComponent<Renderer>();
                rend.material.SetColor("_Color", Color.red);

            }
        }
    }
}





