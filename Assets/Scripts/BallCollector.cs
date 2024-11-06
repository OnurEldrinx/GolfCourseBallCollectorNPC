using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BallCollectorNPC.AI;
using UnityEngine;
using UnityEngine.AI;
using BehaviourTree;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class BallCollector : MonoBehaviour
{

    #region SerializedFields

    [SerializeField] private Transform golfCart;
    [SerializeField] private float movementThreshold;
    [SerializeField] private float health;
    [SerializeField] private float healthPercentageToRush;
    [SerializeField] public GolfBall collectedBall;//Debug
    [SerializeField] public GolfBall targetBall;//Debug

    #endregion
    
    #region PrivateFields

    private NavMeshAgent _agent;
    private Animator _animator;
    private Vector2 _rootMotionVelocity;
    private Vector2 _rootMotionSmoothDeltaPosition;
    private BehaviourTree.BehaviourTree _behaviourTree;
    private List<GolfBall> _allBalls;
    private float _initialHealth;

    #endregion
    
    #region AnimatorStringHashes

    private static readonly int Move = Animator.StringToHash("Move");
    private static readonly int MovementBlend = Animator.StringToHash("MovementBlend");
    private static readonly int Collect = Animator.StringToHash("Collect");

    #endregion
    

    public bool CollectAnimationPlaying { get; set; }
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        
        _animator.applyRootMotion = true;
        _agent.updatePosition = false;
        _agent.updateRotation = true;

        _allBalls = FindObjectsOfType<GolfBall>().ToList();

        _initialHealth = health;
    }

    private async void Start()
    {

        await Task.Delay(2000);
        
        _behaviourTree = new BehaviourTree.BehaviourTree("Ball Collector NPC");

        PrioritySelector actions = new PrioritySelector("Ball Collector AI");

        Sequence deliverySequence = new Sequence("Delivery Sequence",100);
        deliverySequence.AddChild(new Leaf("Still Collecting",new Condition(() => !CollectAnimationPlaying)));
        deliverySequence.AddChild(new Leaf("Holding A Ball?",new Condition(HoldingABall)));
        deliverySequence.AddChild(new Leaf("Return to Cart",new ReturnToGolfCart(this)));

        Sequence enoughHealthSequence = new Sequence("Enough Health Sequence",75);
        enoughHealthSequence.AddChild(new Leaf("Is Health Enough?",new Condition(CheckHealth)));
        enoughHealthSequence.AddChild(new Leaf("Collect Closest Ball,Highest Priority",new CollectClosestBallWithHighestPriority(this)));

        Leaf collectClosestBall = new Leaf("Collect Closest",new CollectClosestBall(this),50);
        
        
        actions.AddChild(deliverySequence);
        actions.AddChild(enoughHealthSequence);
        actions.AddChild(collectClosestBall);
        
        _behaviourTree.AddChild(actions);

    }

    private void OnAnimatorMove()
    {
        var rootPosition = _animator.rootPosition;
        rootPosition.y = _agent.nextPosition.y;
        transform.position = rootPosition;
        _agent.nextPosition = rootPosition;
    }

    private void Update()
    {
        _behaviourTree?.Process();
        SyncAnimatorWithMovement();
        health -= Time.deltaTime;
    }

    private void SyncAnimatorWithMovement()
    {
        var t = transform;
        var currentDeltaPosition = _agent.nextPosition - t.position;
        currentDeltaPosition.y = 0;

        var deltaX = Vector3.Dot(t.right, currentDeltaPosition);
        var deltaY = Vector3.Dot(t.forward, currentDeltaPosition);

        var newDeltaPosition = new Vector2(deltaX,deltaY);
        var s = Mathf.Min(1,Time.deltaTime/10f);

        _rootMotionSmoothDeltaPosition = Vector2.Lerp(_rootMotionSmoothDeltaPosition, newDeltaPosition, s);
        _rootMotionVelocity = _rootMotionSmoothDeltaPosition / Time.deltaTime;

        if (_agent.remainingDistance <= _agent.stoppingDistance)
        {
            _rootMotionVelocity = Vector2.Lerp(Vector2.zero, _rootMotionVelocity, _agent.remainingDistance / _agent.stoppingDistance);
        }

        bool move = _rootMotionVelocity.magnitude > movementThreshold && _agent.remainingDistance > _agent.stoppingDistance;
        
        _animator.SetBool(Move,move);
        _animator.SetFloat(MovementBlend,_rootMotionVelocity.magnitude);

        var deltaPositionMagnitude = currentDeltaPosition.magnitude;

        if (deltaPositionMagnitude > _agent.radius/2f)
        {
            transform.position = Vector3.Lerp(_animator.rootPosition, _agent.nextPosition,s);
        }
        

    }

    public void ReturnToGolfCart()
    {
        _agent.SetDestination(golfCart.position);
    }

    public float DistanceToGolfCart()
    {
        return Vector3.Distance(transform.position, golfCart.position);
    }

    public float DistanceTo(Transform t)
    {
        return Vector3.Distance(transform.position, t.position);
    }
    

    public GolfBall ClosestBallWithHighestPriority()
    {
        List<GolfBall> sorted = _allBalls.OrderByDescending(b => b.Priority).ThenBy(b => Vector3.Distance(transform.position, b.transform.position)).ToList();
        return sorted[0];
    }

    public GolfBall ClosestBall()
    {
        List<GolfBall> sorted = _allBalls.OrderBy(b => Vector3.Distance(transform.position, b.transform.position)).ToList();
        return sorted[0];
    }

    public void SetAgentDestination(Vector3 destination)
    {
        _agent.SetDestination(destination);
    }

    private bool HoldingABall()
    {
        return collectedBall is not null;
    }

    private bool CheckHealth()
    {
        return health > _initialHealth * healthPercentageToRush;
    }

    public void ResetAgentPath()
    {
        _agent.ResetPath();
    }

    public void BallCollected(GolfBall golfBall)
    {
        _allBalls.Remove(golfBall);
    }

    public void CollectAnimation()
    {
        _animator.SetTrigger(Collect);       
    }

    
    
}
