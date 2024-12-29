using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Unit : MonoBehaviour
{
    protected GameManager Owner;
    public Vector2Int Position;
    public Attack[] Attacks;
    public int TotalActions = 2;
    public int Actions = 2;
    public int Health;
    public int MoveDistance;
    public int Speed;
    public int Aliance;
    private bool InVanishment;
    Vector2Int[] BestPath;
    Vector2Int FinalTarget;
    int[,] Spots;
    int[,] SearchableSpots;
    public GameObject DamageTextObject;
    public bool InMovement = false;
    public GameObject AttackEffect;
    private void Update()
    {
        UnitUpdate();
    }
    private void Start()
    {
        Speed *= 100;
        Speed += UnityEngine.Random.Range(-49,49);
        Owner = FindObjectOfType<GameManager>();
        this.transform.position = new Vector2(Position.x + 0.5f,Position.y + 0.5f);
        UnitStart();
    }
    public virtual void Turn()
    {
        Actions = TotalActions;
    }
    public virtual void EndTurn()
    {
    }
    public virtual void UnitUpdate()
    {

    }
    public virtual void UnitStart()
    {

    }
    public void Move(Vector2Int Target)
    {
        if (!IsTraversable(Target) || InMovement)
        {
           return;
        }
        Owner.OrderUnitsOnBoard();
        GetPath(Target);
        if (BestPath.Length != 0)
        {
            BestPath = BestPath.Skip(1).ToArray();
            StartCoroutine(Transist(Position));
        }
    }
    private IEnumerator Transist(Vector2Int Target)
    {
        InMovement = true;
        foreach (Vector2Int v in BestPath)
        {
            Position = v;
            this.transform.position = new Vector2(Position.x + 0.5f, Position.y + 0.5f);
            yield return new WaitForSeconds(0.2f);
        }
        Owner.OrderUnitsOnBoard();
        InMovement = false;
        Action();
    }
    private void GetPath(Vector2Int Target)
    {
        Spots = new int[Owner.BoardScale,Owner.BoardScale];
        FinalTarget = Target;
        BestPath = new Vector2Int[] { };
        SearchSpots(Position, new Vector2Int[] { Position });
    }
    private void SearchSpots(Vector2Int Pos, Vector2Int[] Path)
    {
        if(Pos == FinalTarget)
        {
            if(BestPath.Length == 0)
            {
                BestPath = Path;
            }
            else if(BestPath.Length > Path.Length)
            {
                BestPath = Path;
            }
            return;
        }
        Vector2Int ArrayPos = new Vector2Int(Pos.x + (Owner.HalfScale), Pos.y + (Owner.HalfScale));
        if (Path.Length <= MoveDistance && (Spots[ArrayPos.x, ArrayPos.y] == 0 || Spots[ArrayPos.x, ArrayPos.y] > Path.Length) )
        {
            Spots[ArrayPos.x, ArrayPos.y] = Path.Length;
            Vector2Int UP = Pos + Vector2Int.up;
            if(IsTraversable(UP))
            {
                Vector2Int[] Path2 = Path.Append(UP).ToArray();
                SearchSpots(UP,Path2);
            }
            Vector2Int DN = Pos + Vector2Int.down;
            if (IsTraversable(DN))
            {
                Vector2Int[] Path2 = Path.Append(DN).ToArray();
                SearchSpots(DN, Path2);
            }
            Vector2Int LF = Pos + Vector2Int.left;
            if (IsTraversable(LF))
            {
                Vector2Int[] Path2 = Path.Append(LF).ToArray();
                SearchSpots(LF, Path2);
            }
            Vector2Int RT = Pos + Vector2Int.right;
            if (IsTraversable(RT))
            {
                Vector2Int[] Path2 = Path.Append(RT).ToArray();
                SearchSpots(RT, Path2);
            }
        }
    }
    protected bool IsTraversable(Vector2Int Spot)
    {
        Vector3Int Spot3 = new Vector3Int(Spot.x,Spot.y,0);
        if (Owner.tilemap.GetTile(Spot3) != null)
        {
            if(Owner.HalfScale <= Spot.x || -Owner.HalfScale >= Spot.x || Owner.HalfScale <= Spot.y || -Owner.HalfScale >= Spot.y)
            {
                return false;
            }
            Vector2Int ArrayPos = new Vector2Int(Spot.x + (Owner.HalfScale), Spot.y + (Owner.HalfScale));
            if (Owner.BoardUnits[ArrayPos.x,ArrayPos.y] != null && Owner.BoardUnits[ArrayPos.x, ArrayPos.y] != this) { return false; }
            return true;
        }
        else
        {
            return false;
        }
    }
    public void Vanish()
    {
        InVanishment = true;
        StartCoroutine(Vanishing());
    }

    private IEnumerator Vanishing()
    {
        SpriteRenderer SR = this.GetComponent<SpriteRenderer>();
        float time = 1.3f;
        for(float i = 0; i <= time; i+= Time.deltaTime)
        {
            float col = Mathf.Lerp(1,0,i/time);
            SR.color = new Color(1,1,1,col);
            yield return new WaitForEndOfFrame();
        }
    }
    public void UseAttack(int ID)
    {
        if (InVanishment) { return; }
        Attack Choice = Attacks[ID];
        foreach(Vector2Int spot in Choice.DestroyLand)
        {
            if (Owner.tilemap.GetTile(new Vector3Int(spot.x + Position.x, spot.y + Position.y, 0)) != null && RetrieveUnit(spot + Position) == null)
            {
                Owner.tilemap.SetTile(new Vector3Int(spot.x + Position.x, spot.y + Position.y, 0), null);
                GameObject obj = Instantiate(Owner.MatterAlterEffect);
                obj.transform.position = new Vector3(spot.x + Position.x + 0.5f, spot.y + Position.y + 0.5f, 0);
            }
        }
        foreach (Vector2Int spot in Choice.CreateLand)
        {
            if (Owner.tilemap.GetTile(new Vector3Int(spot.x + Position.x, spot.y + Position.y, 0)) != Owner.Land && RetrieveUnit(spot + Position) == null && InBounds(spot+Position))
            {
                Owner.tilemap.SetTile(new Vector3Int(spot.x + Position.x, spot.y + Position.y, 0), Owner.Land);
                GameObject obj = Instantiate(Owner.MatterAlterEffect);
                obj.transform.position = new Vector3(spot.x + Position.x + 0.5f, spot.y + Position.y + 0.5f, 0);
            }
        }
        foreach (Vector2Int spot in Choice.Damage)
        {
            GameObject obj = Instantiate(AttackEffect);
            obj.transform.position = new Vector3(spot.x + Position.x + 0.5f, spot.y + Position.y + 0.5f, 0);
            if (RetrieveUnit(spot + Position) != null)
            {
                RetrieveUnit(spot + Position).TakeDamage((int)(Attacks[ID].Power * UnityEngine.Random.Range(0.9f,1.5f)));
            }
        }
        Action();
    }
    private bool InBounds(Vector2Int Pos)
    {
        if (Owner.HalfScale <= Pos.x || -Owner.HalfScale >= Pos.x || Owner.HalfScale <= Pos.y || -Owner.HalfScale >= Pos.y)
        {
            return false;
        }
        return true;
    }
    protected Unit RetrieveUnit(Vector2Int Pos)
    {
        if (Owner.HalfScale <= Pos.x || -Owner.HalfScale >= Pos.x || Owner.HalfScale <= Pos.y || -Owner.HalfScale >= Pos.y)
        {
            return null;
        }
        Vector2Int ArrayPos = new Vector2Int(Pos.x + (Owner.HalfScale), Pos.y + (Owner.HalfScale));
        return Owner.BoardUnits[ArrayPos.x, ArrayPos.y];
    }
    protected void DisplayPossibleSpots()
    {
        SearchableSpots = new int[Owner.BoardScale, Owner.BoardScale];
        SearchSpotsDisplay(Position, new Vector2Int[] { Position });
    }
    protected void HilightUnit()
    {
        Owner.DisplaySpots.SetTile(new Vector3Int(Position.x, Position.y, 0), Owner.DisplayDot);
    }
    protected void ClearDisplay()
    {
        Owner.DisplaySpots.ClearAllTiles();
    }
    private void SearchSpotsDisplay(Vector2Int Pos, Vector2Int[] Path)
    {
        Vector2Int ArrayPos = new Vector2Int(Pos.x + (SearchableSpots.GetLength(0) / 2), Pos.y + (SearchableSpots.GetLength(1) / 2));
        if (Path.Length <= MoveDistance + 1 && (SearchableSpots[ArrayPos.x, ArrayPos.y] == 0 || SearchableSpots[ArrayPos.x, ArrayPos.y] > Path.Length))
        {
            SearchableSpots[ArrayPos.x, ArrayPos.y] = Path.Length;
            Owner.DisplaySpots.SetTile(new Vector3Int(Pos.x,Pos.y, 0), Owner.DisplayDot);
            Vector2Int UP = Pos + Vector2Int.up;
            if (IsTraversable(UP))
            {
                Vector2Int[] Path2 = Path.Append(UP).ToArray();
                SearchSpotsDisplay(UP, Path2);
            }
            Vector2Int DN = Pos + Vector2Int.down;
            if (IsTraversable(DN))
            {
                Vector2Int[] Path2 = Path.Append(DN).ToArray();
                SearchSpotsDisplay(DN, Path2);
            }
            Vector2Int LF = Pos + Vector2Int.left;
            if (IsTraversable(LF))
            {
                Vector2Int[] Path2 = Path.Append(LF).ToArray();
                SearchSpotsDisplay(LF, Path2);
            }
            Vector2Int RT = Pos + Vector2Int.right;
            if (IsTraversable(RT))
            {
                Vector2Int[] Path2 = Path.Append(RT).ToArray();
                SearchSpotsDisplay(RT, Path2);
            }
        }
    }
    public void TakeDamage(int Amount)
    {
        GameObject obj = Instantiate(DamageTextObject);
        obj.transform.position = this.transform.position;
        obj.GetComponent<DamageText>().Words = $"-{Amount.ToString()}";
        Health -= Amount;
        if(Health <= 0)
        {
            Owner.units.Remove(this);
            Owner.PlayerUnits.Remove(this);
            Owner.FoeUnits.Remove(this);
            Owner.OrderUnitsOnBoard();
            Destroy(this.gameObject);
        }
    }
    public void Action()
    {
        if (Actions <= 0)
        {
            EndTurn();
            Owner.NextTurn();
        }
        Actions -= 1;
    }
}