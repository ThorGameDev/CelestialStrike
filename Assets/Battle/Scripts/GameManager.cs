using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    private Battle currentBattle;
    [HideInInspector] public List<Unit> units;
    [HideInInspector] public List<Unit> PlayerUnits;
    [HideInInspector] public List<Unit> FoeUnits;
    public CameraBehavior Camera;
    public Tilemap tilemap;
    public Tile Land;
    public Tilemap DisplaySpots;
    public Tile DisplayDot;
    public int BoardScale;
    [HideInInspector]public int HalfScale;
    [HideInInspector]public Unit[,] BoardUnits;
    [Header("Prefabs")]
    public Unit PlayerPrefab;
    const float TimeBetweenDestroy = 0.2f;
    public Unit FoePrefab;
    public GameObject MatterAlterEffect;
    public GameObject[] DestroyList;
    public void Start()
    {
        currentBattle = StaticVariables.currentBattle;
        BoardScale = currentBattle.BoardScale;
        HalfScale = BoardScale / 2;
        for (int y = HalfScale; y >= -HalfScale; y--)
        {
            for (int x = HalfScale; x >= -HalfScale; x--)
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), currentBattle.Board.GetTile(new Vector3Int(x, y, 0)));
            }
        }
        foreach (Vector3Int Spot in currentBattle.PlayerSpawns)
        {
            GameObject Player = Instantiate(PlayerPrefab.gameObject);
            Unit u = Player.GetComponent<Unit>();
            u.Position = new Vector2Int(Spot.x,Spot.y);
            u.Speed = Spot.z;
            units.Add(u);
            PlayerUnits.Add(u);
        }
        foreach (Vector3Int Spot in currentBattle.FoeSpawns)
        {
            GameObject Foe = Instantiate(FoePrefab.gameObject);
            Unit u = Foe.GetComponent<Unit>();
            u.Position = new Vector2Int(Spot.x, Spot.y);
            u.Speed = Spot.z;
            units.Add(u);
            FoeUnits.Add(u);
        }
        StartCoroutine(Game());
    }
    public IEnumerator Game()
    {
        yield return new WaitForSeconds(1f);
        units = units.OrderBy(s => -s.Speed).ToList();
        int i = 0;
        while (PlayerUnits.Count > 0 && FoeUnits.Count > 0)
        {
            if (i >= units.Count) { break; }
            Unit unit = units[i];
            if (unit == null) { continue; }
            OrderUnitsOnBoard();
            unit.Turn();
            InTurn = true;
            Camera.Target = unit.gameObject;
            while (InTurn)
            {
                if (!(PlayerUnits.Count > 0 && FoeUnits.Count > 0))
                {
                    while (unit.Actions > 0)
                    {
                        unit.Action();
                    }
                    goto OutLoop;
                }
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForSeconds(0.3f);
            i = GetIndex(units, unit) + 1;
            if(i > units.Count - 1)
            {
                i = 0;
            }
            #region
            /*
            for (int i = 0; i <= units.Count - 1; i++)
            {
                if (i >= units.Count) { break; }
                Unit unit = units[i];
                if (unit == null) { continue; }
                OrderUnitsOnBoard();
                unit.Turn();
                InTurn = true;
                Camera.Target = unit.gameObject;
                while (InTurn)
                {
                    if (!(PlayerUnits.Count > 0 && FoeUnits.Count > 0))
                    {
                        while (unit.Actions > 0)
                        {
                            unit.Action();
                        }
                        goto OutLoop;
                    }
                    yield return new WaitForEndOfFrame();
                }
                yield return new WaitForSeconds(0.3f);
            }
            */
            #endregion
        }
        goto OutLoop;
        OutLoop:
        if(PlayerUnits.Count > FoeUnits.Count)
        {
            if(PlayerPrefs.GetInt("CurrentBattle") == StaticVariables.BattleID)
            {
                PlayerPrefs.SetInt("CurrentBattle", PlayerPrefs.GetInt("CurrentBattle")+1);
            }
        }
        foreach(Unit u in units)
        {
            u.Vanish();
        }
        DisplaySpots.ClearAllTiles();
        StartCoroutine(DestroyAll());
        DisplaySpots.ClearAllTiles();
        yield return new WaitForSeconds(BoardScale*TimeBetweenDestroy+1);
        DisplaySpots.ClearAllTiles();
        StartCoroutine(FadeOut());
        DisplaySpots.ClearAllTiles();
    }
    public IEnumerator DestroyAll()
    {
        this.DisplaySpots.ClearAllTiles();
        foreach(GameObject g in DestroyList)
        {
            Destroy(g);
        }
        for(int y = HalfScale; y >= -HalfScale; y--)
        {
            for (int x = HalfScale; x >= -HalfScale; x--)
            {
                if (tilemap.GetTile(new Vector3Int(x, y, 0)) != null)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), null);
                    GameObject obj = Instantiate(MatterAlterEffect);
                    obj.transform.position = new Vector3(x + 0.5f, y + 0.5f, 0);
                }
            }
            yield return new WaitForSeconds(TimeBetweenDestroy);
        }
    }
    public void OrderUnitsOnBoard()
    {
        BoardUnits = new Unit[BoardScale, BoardScale];
        foreach (Unit u in units)
        {
            Vector2Int ArrayPos = new Vector2Int(u.Position.x + (BoardUnits.GetLength(0) / 2), u.Position.y + (BoardUnits.GetLength(1) / 2));
            BoardUnits[ArrayPos.x, ArrayPos.y] = u;
        }
    }
    public int GetIndex(List<Unit> List, Unit Value)
    {
        for(int i = 0; i < List.Count; i++)
        {
            if(List[i] == Value)
            {
                return (i);
            }
        }
        Debug.Log("Hi");
        return (-1);
    }
    public bool InTurn = false;
    public void NextTurn()
    {
        InTurn = false;
    }
    public Transform CanvasOBJ;
    public GameObject Fader;
    public IEnumerator FadeOut()
    {
        Instantiate(Fader).transform.SetParent(CanvasOBJ);
        yield return new WaitForSeconds(2);
        UnityEngine.SceneManagement.SceneManager.LoadScene("World Map");
    }
}
