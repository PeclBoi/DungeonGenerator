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

        var otherObject = Physics.OverlapSphere(transform.GetComponent<Renderer>().bounds.center, 0.1f, mask).FirstOrDefault(c => c.gameObject != gameObject);
        if (otherObject != null)
        {
            Destroy(gameObject);
        }
    }


}
