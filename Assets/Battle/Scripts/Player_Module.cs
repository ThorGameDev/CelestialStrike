using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Player_Module : Unit
{
    private bool InTurn;
    private ActionBar AB;
    public override void UnitStart()
    {
        base.UnitStart();
        AB = FindObjectOfType<ActionBar>();
    }
    public override void Turn()
    {
        base.Turn();
        InTurn = true;
        AB.CurrentUnit = this;
        ChangeState(0);
        HilightUnit();
    }
    public override void EndTurn()
    {
        base.EndTurn();
        InTurn = false;
        AB.CurrentUnit = null;
        ClearDisplay();
    }
    public override void UnitUpdate()
    {
        if (InTurn && !InMovement)
        {
            if(State == 1)
            {
                Vector2 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2Int MouseTile = new Vector2Int(Mathf.RoundToInt(MousePos.x - 0.5f), Mathf.RoundToInt(MousePos.y - 0.5f));
                if (Input.GetMouseButton(0))
                {
                    Move(MouseTile);
                    if (InMovement)
                    {
                        ChangeState(0);
                    }
                }
            }
        }
    }
    public void ChangeState(int choice)
    {
        if(choice == 1)
        {
            DisplayPossibleSpots();
        }
        else
        {
            ClearDisplay();

        }
        if (!InMovement)
        {
            HilightUnit();
        }
        State = choice;
    }
    [Tooltip("0:Choice\n" +
        "1:Move\n" +
        "2:Atack")]
    public int State; 
}
