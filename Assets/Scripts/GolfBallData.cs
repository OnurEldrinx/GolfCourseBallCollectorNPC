using UnityEngine;

[CreateAssetMenu(fileName = "GolfBallData", menuName = "ScriptableObjects/GolfBallSO", order = 1)]
public class GolfBallData : ScriptableObject
{
    public Priority priority;
    public Material material;
    public int point;
}
public enum Priority
{
    Low,
    Medium,
    High
}