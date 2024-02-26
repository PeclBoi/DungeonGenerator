using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValidatorTrigger : MonoBehaviour
{

    [SerializeField] private Validator validator;

    private RoomPlacer roomPlacer;

    private void Start()
    {
        roomPlacer = RoomPlacer.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (validator.IsValid) { return; }

        if (other.TryGetComponent(out ValidatorTrigger trigger))
        {
            validator.IsValid = validator.framesTillValid < trigger.validator.framesTillValid;

            if (validator.IsValid) { return; }

            validator.StopCoroutine();
            roomPlacer.DeregisterRoom(validator.room);
            Destroy(transform.root.gameObject);
        }
    }
}
