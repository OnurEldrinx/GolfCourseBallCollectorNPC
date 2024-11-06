using UnityEngine;

public class GolfBall : MonoBehaviour
{
    [SerializeField] private GolfBallData data;
    public Priority Priority { get; private set; }
    public int Point { get; private set; }

    private MeshRenderer _meshRenderer;

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        
        _meshRenderer = GetComponent<MeshRenderer>();
        
        _meshRenderer.material = data.material;
        Priority = data.priority;
        Point = data.point;
    }

    public void UseGravity(bool value)
    {
        _rb.useGravity = value;
    }
    
}
