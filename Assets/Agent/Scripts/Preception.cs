using UnityEngine;

public abstract class Perception : MonoBehaviour
{
    [SerializeField] internal string info;
    [SerializeField] protected LayerMask layerMask = Physics.AllLayers;
    [SerializeField] protected string tagName;
    [SerializeField] protected float maxDistance = 5;
    [SerializeField, Range (0, 180)] protected float viewAngle;

    public abstract GameObject[] GetGameObjects();
}
