using UnityEngine;

/**
 * This component patrols between given points, chases a given target object when it sees it, and rotates from time to time.
 */
[RequireComponent(typeof(Patroller))]
[RequireComponent(typeof(Chaser))]
[RequireComponent(typeof(LocationGenerator))]
public class EnemyControllerStateMachine : StateMachine
{
    [SerializeField] float radiusToWatch = 5f;

    [Tooltip("The probability to change location in each frame 1/n.")]
    [SerializeField] int probabilityToChangeLocation = 10000;

    private Chaser chaser;
    private Patroller patroller;

    private LocationGenerator locationGenerator;

    private float DistanceToTarget()
    {
        return Vector3.Distance(transform.position, chaser.TargetObjectPosition());
    }

    private void Awake()
    {
        chaser = GetComponent<Chaser>();
        patroller = GetComponent<Patroller>();
        locationGenerator = GetComponent<LocationGenerator>();
        base
        .AddState(patroller)     // This would be the first active state.
        .AddState(chaser)
        .AddState(locationGenerator)
        .AddTransition(patroller, () => DistanceToTarget() <= radiusToWatch, chaser)
        .AddTransition(chaser, () => DistanceToTarget() > radiusToWatch, patroller)
        .AddTransition(patroller, () => Random.Range(1, probabilityToChangeLocation) == 5, locationGenerator)
        .AddTransition(locationGenerator, () => locationGenerator.IsDone(), patroller);

        ;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radiusToWatch);
    }
}
