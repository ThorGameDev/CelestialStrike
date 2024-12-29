using UnityEngine.UI;
using UnityEngine;

public class FadeIn : MonoBehaviour
{
    public Image Box;
    public float A = 1;
    public float speed;
    public bool Inverse;
    void Update()
    {
        if (!Inverse)
        {
            A -= Time.deltaTime * speed;
            Box.color = new Color(0, 0, 0, A);
            if (A <= 0)
            {
                Destroy(this.gameObject);
            }
        }
        else
        {
            A += Time.deltaTime * speed;
            Box.color = new Color(0, 0, 0, A);
            if (A >= 1)
            {
                //speed = 0;
            }
        }
    }
}
