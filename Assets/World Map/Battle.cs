using UnityEngine.Tilemaps;
using UnityEngine;
[CreateAssetMenu(fileName = "Battle", menuName = "ScriptableObjects/Battle", order = 2)]
public class Battle : ScriptableObject
{
    public Tilemap Board;
    public int BoardScale;
    public Vector3Int[] FoeSpawns;
    public Vector3Int[] PlayerSpawns;
}