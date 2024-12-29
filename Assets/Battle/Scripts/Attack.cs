using UnityEngine;
[CreateAssetMenu(fileName = "Attack", menuName = "ScriptableObjects/Attack", order = 1)]
public class Attack : ScriptableObject
{
    public int Power = 10;
    public Vector2Int[] CreateLand;
    public Vector2Int[] DestroyLand;
    public Vector2Int[] Damage;
}
