using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{ 
    public GameObject Target;
    public ActionBar AB;
    public float Speed;
    public bool ChasingPlayer;
    public float DistanceStop;
    public float DistanceStart;
    public float LookSpeed;
    public void FixedUpdate()
    {
        if (Target == null)
        {
            return;
        }
        Vector2 ViewPos = Target.transform.position;
        if(AB.State == 3)
        {
            Vector2 Incriment = Camera.main.ScreenToWorldPoint(Input.mousePosition) - this.transform.position;
            float Dist = Vector2.Distance(Incriment, Vector2.zero);
            float CameraSC = Camera.main.orthographicSize;
            if (Dist > CameraSC)
            {
                Incriment = Incriment.normalized * CameraSC;
            }
            ViewPos += Incriment * LookSpeed;
        }
        if (Vector2.Distance(ViewPos, this.transform.position) > DistanceStart)
        {
            ChasingPlayer = true;
        }
        else if (Vector2.Distance(ViewPos, this.transform.position) < DistanceStop)
        {
            ChasingPlayer = false;
        }
        if (ChasingPlayer == true)
        {
            this.transform.position = new Vector3(Mathf.Lerp(transform.position.x, ViewPos.x, Time.fixedDeltaTime * Speed), Mathf.Lerp(transform.position.y, ViewPos.y, Time.fixedDeltaTime * Speed), -10);
        }
    }
}
