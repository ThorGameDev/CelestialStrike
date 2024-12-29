using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AI_Module : Unit
{
    public bool InTurn;
    int[,] Spots;
    public override void Turn()
    {
        base.Turn();
        StartCoroutine(UseBestAction());
        InTurn = true;
    }
    public override void EndTurn()
    {
        base.EndTurn();
        InTurn = false;
    }
    public IEnumerator UseBestAction()
    {
        yield return new WaitForEndOfFrame();

        while (Actions >= 0 && InTurn)
        {
            //Action 1
            while (InMovement) { yield return new WaitForEndOfFrame(); }
            ContemplateAvaliableSpots();
            CalculateMove(out Vector2Int BestSpot);
            if (!IsTraversable(BestSpot)) { Debug.Log("Isnt Traversable"); }
            if (InRange)
            {
                if (BestSpot == Position)
                {
                    yield return new WaitForSeconds(0.2f);
                    CalculateAttack(out int choice);
                    UseAttack(choice);
                    yield return new WaitForSeconds(1.3f);
                }
                else
                {
                    Move(BestSpot);
                    while (InMovement) { yield return new WaitForEndOfFrame(); }
                }
            }
            else
            {
                CalculateCloseIn(out Vector2Int CloseSpot);
                if (!IsTraversable(CloseSpot)) { Debug.Log("Close Isnt Traversable"); }
                if (CloseSpot == Position)
                {
                    yield return new WaitForSeconds(0.2f);
                    CalculateCloseInAct(out int choice);
                    UseAttack(choice);
                    yield return new WaitForSeconds(1.3f);
                }
                else
                {
                    
                    Move(CloseSpot);
                    while (InMovement) { yield return new WaitForEndOfFrame(); }
                }
            }

            yield return new WaitForEndOfFrame();
        }
    }
    public void CalculateMove(out Vector2Int BestSpot)
    {
        Vector2Int spot = Position;
        float ExpectedDamage = -999;
        InRange = false;
        foreach (Vector2Int Spot in AvalibleSpots)
        {
            for (int i = 0; i < Attacks.Length; i++)
            {
                int ThisAttackDamage = 0;
                foreach (Vector2Int DamageSpot in Attacks[i].Damage)
                {
                    if (RetrieveUnit(DamageSpot + Spot) != null)
                    {
                        if (RetrieveUnit(DamageSpot + Spot).Aliance == Aliance)
                        {
                            ThisAttackDamage -= Attacks[i].Power * 2;
                        }
                        else
                        {
                            ThisAttackDamage += Attacks[i].Power;
                            InRange = true;
                        }
                    }
                }
                if (ThisAttackDamage > ExpectedDamage || (Spot == Position && ThisAttackDamage == ExpectedDamage))
                {
                    spot = Spot;
                    ExpectedDamage = ThisAttackDamage;
                    if(spot == Position)
                    {
                        ExpectedDamage += 0.5f;
                    }
                }
            }
            
        }
        if(ExpectedDamage <= 0.7f)
        {
            InRange = false;
        }
        BestSpot = spot;
    }
    public void CalculateAttack(out int attack)
    {
        int best = 0;
        float ExpectedDamage = -999;
        InRange = false;
        for (int i = 0; i < Attacks.Length; i++)
        {
            int ThisAttackDamage = 0;
            foreach (Vector2Int DamageSpot in Attacks[i].Damage)
            {
                if (RetrieveUnit(DamageSpot + Position) != null)
                {
                    if (RetrieveUnit(DamageSpot + Position).Aliance == Aliance)
                    {
                        ThisAttackDamage -= Attacks[i].Power * 2;
                    }
                    else
                    {
                        InRange = true;
                        ThisAttackDamage += Attacks[i].Power;
                    }
                }
            }
            if (ThisAttackDamage >= ExpectedDamage)
            {
                best = i;
                ExpectedDamage = ThisAttackDamage;
                ExpectedDamage += UnityEngine.Random.Range(-0.1f, 0.1f);
            }
        }
        attack = best;
    }
    public void CalculateCloseIn(out Vector2Int BestSpot)
    {
        Vector2Int spot = Position;
        float distance = 9999999999;
        Unit closest = this;
        foreach (Unit u in Owner.units)
        {
            if(u.Aliance == Aliance) { continue; }
            float dist = Mathf.Abs(u.Position.x - Position.x) + Mathf.Abs(u.Position.y - Position.y);
            if(dist <= distance)
            {
                closest = u;
                distance = dist;
            }
        }
        distance = 9999999999;
        foreach (Vector2Int Spot in AvalibleSpots)
        {
            float dist = Mathf.Abs(closest.Position.x - Spot.x) + Mathf.Abs(closest.Position.y - Spot.y);
            if (dist <= distance)
            {
                spot = Spot;
                distance = dist;
                if(spot == Position)
                {
                    distance -= 0.3f;
                }
            }
        }
        BestSpot = spot;
    }
    public void CalculateCloseInAct(out int attack)
    {
        int best = 0;
        int Create = -9999;
        for (int i = 0; i < Attacks.Length; i++)
        {
            int Quality = Attacks[i].CreateLand.Length - Attacks[1].DestroyLand.Length;
            if (Quality > Create)
            {
                best = i;
                Create = Quality;
            }
        }
        attack = best;
    }
    public List<Vector2Int> AvalibleSpots;
    public bool InRange;
    int[,] AvaliableSpotsGrid;
    private void ContemplateAvaliableSpots()
    {
        AvaliableSpotsGrid = new int[Owner.BoardScale, Owner.BoardScale];
        AvalibleSpots = new List<Vector2Int>() { };
        AddSpotToList(Position, 1);
    }
    private void AddSpotToList(Vector2Int Pos, int dist)
    {
        Vector2Int ArrayPos = new Vector2Int(Pos.x + (Owner.HalfScale), Pos.y + (Owner.HalfScale));
        if (dist <= MoveDistance && (AvaliableSpotsGrid[ArrayPos.x, ArrayPos.y] == 0 || AvaliableSpotsGrid[ArrayPos.x, ArrayPos.y] > dist))
        {
            AvaliableSpotsGrid[ArrayPos.x, ArrayPos.y] = dist;
            AvalibleSpots = AvalibleSpots.Append(Pos).ToList();
            Vector2Int UP = Pos + Vector2Int.up;
            if (IsTraversable(UP))
            {
                AddSpotToList(UP, dist + 1);
            }
            Vector2Int DN = Pos + Vector2Int.down;
            if (IsTraversable(DN))
            {
                AddSpotToList(DN, dist + 1);
            }
            Vector2Int LF = Pos + Vector2Int.left;
            if (IsTraversable(LF))
            {
                AddSpotToList(LF, dist + 1);
            }
            Vector2Int RT = Pos + Vector2Int.right;
            if (IsTraversable(RT))
            {
                AddSpotToList(RT, dist + 1);
            }
        }
    }
}
