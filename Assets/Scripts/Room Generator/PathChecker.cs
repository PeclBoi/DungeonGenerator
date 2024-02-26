using UnityEngine;
using UnityEngine.AI;

public class PathChecker
{
    private NavMeshAgent _agent;
    private NavMeshPath _navMeshPath;

    public PathChecker(NavMeshAgent agent)
    {
        _agent = agent;
        _navMeshPath = new NavMeshPath();
    }

    private void MoveNavMeshAgent(Vector3 position)
    {
        _agent.enabled = false;
        _agent.transform.position = position;
        _agent.enabled = true;
    }
    public bool IsPathClear(Vector3 startPos, Vector3 destinationPos)
    {
        MoveNavMeshAgent(startPos);
        _agent.CalculatePath(destinationPos, _navMeshPath);
        _agent.destination = destinationPos;
        if (_navMeshPath.status != NavMeshPathStatus.PathComplete)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
