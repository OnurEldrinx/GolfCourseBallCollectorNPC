using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BallCollectorNPC.AI;
using UnityEngine;
using UnityEngine.AI;
using BehaviourTree;
using TMPro;
using UnityEngine.UI;

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
    [SerializeField] private Transform leftHandSocket; //to place the collected ball
    [SerializeField] private int score;//Debug
    [SerializeField] private Image healthBarFill;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI healthText;
    #endregion
    
    #region PrivateFields

    private NavMeshAgent _agent;
    private Animator _animator;
    private Vector2 _rootMotionVelocity;
    private Vector2 _rootMotionSmoothDeltaPosition;
    private BehaviourTree.BehaviourTree _behaviourTree;
    private List<GolfBall> _allBalls;
    private float _initialHealth;
    private bool _fail;
    private bool _win;
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
        scoreText.text = "0";
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
        if (!_win && !_fail)
        {
            _behaviourTree?.Process();
        }
        SyncAnimatorWithMovement();
        UpdateHealth();
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

    private void UpdateHealth()
    {
        if (_win || _fail)
        {
            return;
        }
        
        if (health <= 0)
        {
            health = 0;
            _fail = true;
            _agent.ResetPath();
        }
        else
        {
            health -= Time.deltaTime;
        }

        healthBarFill.fillAmount -= Time.deltaTime/_initialHealth;
        healthText.text = health.ToString("F0");
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

    public void AttachBallToHand()
    {
        print("Attaching Ball");
        collectedBall.UseGravity(false);
        var t = collectedBall.transform;
        t.parent = leftHandSocket;
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.Euler(Vector3.zero);
    }

    public void DropBall()
    {
        UpdateScore(collectedBall.Point);
        collectedBall.UseGravity(true);
        collectedBall.transform.parent = null;
        collectedBall = null;
        if (_allBalls.Count == 0)
        {
            _win = true;
        }
    }

    private void UpdateScore(int value)
    {
        score += value;
        scoreText.text = score.ToString();
    }
    
}
