using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValidatorTrigger : MonoBehaviour
{

    [SerializeField] private Validator validator;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ValidatorTrigger trigger))
        {
            if (validator.framesTillValid > 0)
            {
                validator.StopCoroutine();
                validator.IsValid = validator.framesTillValid < trigger.validator.framesTillValid;
                //transform.root.gameObject.SetActive(validator.framesTillValid < trigger.validator.framesTillValid);

                if (validator.IsValid)
                {
                    Destroy(other.transform.root.gameObject);
                }
                else
                {
                    Destroy(transform.root.gameObject);
                }
            }
            else
            {
                validator.StopCoroutine();
                trigger.validator.IsValid = true;
                Destroy(other.transform.root.gameObject);
                //other.transform.root.gameObject.SetActive(false);
            }
        }
    }
}
