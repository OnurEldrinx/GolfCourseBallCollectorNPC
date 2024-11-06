using UnityEngine;

public class GolfBall : MonoBehaviour
{
    [SerializeField] private GolfBallData data;
    public Priority Priority { get; private set; }
    public int Point { get; private set; }

    private MeshRenderer _meshRenderer;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        
        _meshRenderer.material = data.material;
        Priority = data.priority;
        Point = data.point;
    }

    
}
