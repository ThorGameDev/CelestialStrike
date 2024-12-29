using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bobble : MonoBehaviour
{
    public float StartY;
    public float Incriment;
    public float Distance;
    public float Speed;
    float RandomOfset;
    // Start is called before the first frame update
    void Start()
    {
        StartY = this.transform.position.y;
        RandomOfset = UnityEngine.Random.Range(0,100);
    }
    void Update()
    {
        Incriment = Mathf.Sin((Time.time + RandomOfset) / Speed) * Distance;
        transform.position = new Vector3(transform.position.x,StartY + Incriment,0);
    }
}
