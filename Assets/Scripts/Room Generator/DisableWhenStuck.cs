using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DisableWhenStuck : MonoBehaviour
{

    public LayerMask mask;

    public Validator validator;

    void Update()
    {

        if(validator.IsValid)
        {
            enabled = false;
            return;
        }

        var renderer = transform.GetComponent<Renderer>();

        if(renderer == null)
        {
            renderer = transform.GetComponentInChildren<Renderer>();
        }

        var otherObject = Physics.OverlapSphere(renderer.bounds.center, 0.1f, mask).FirstOrDefault(c => c.gameObject != gameObject);
        if (otherObject != null)
        {
            //Destroy(gameObject);
            gameObject.SetActive(false);
        }
    }


}
