using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    private NavMeshAgent _agent;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        if (_agent == null)
        {
            Debug.LogError("EnemyBehaviour: ¡El enemigo necesita un NavMeshAgent! Añádelo al GameObject.");
        }
    }
}
