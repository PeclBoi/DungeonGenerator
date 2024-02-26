using Unity.AI.Navigation;
using UnityEngine;

public class GenerateNavMesh : MonoBehaviour
{

    public NavMeshSurface navMeshSurface;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.B))
            navMeshSurface.BuildNavMesh();

    }
}
