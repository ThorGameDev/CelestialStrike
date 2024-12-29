using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    public string Words;
    public TMPro.TMP_Text t;
    public float GrowTime;
    public float SmallScize;
    public float LargeScize;
    public float Low;
    public float High;
    public float HighTime;
    public float DisapearTime;
    public void Start()
    {
        t.text = Words;
        StartCoroutine(Life());
    }
    public IEnumerator Life()
    {
        Vector3 OriginPos = transform.position;
        for (float i = 0; i <= GrowTime; i+= Time.deltaTime)
        {
            this.transform.localScale = new Vector3(Mathf.Lerp(SmallScize,LargeScize,i/GrowTime), Mathf.Lerp(SmallScize, LargeScize, i/GrowTime), 1);
            this.transform.position = new Vector3(0, Mathf.Lerp(Low, LargeScize, i), 0) + OriginPos;
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(HighTime);
        for (float i = 0; i <= DisapearTime; i += Time.deltaTime)
        {
            this.transform.localScale = new Vector3(Mathf.Lerp(LargeScize, 0, i / DisapearTime), Mathf.Lerp(LargeScize, 0, i / DisapearTime), 1);
            yield return new WaitForEndOfFrame();
        }
        Destroy(this.gameObject);
    }
}
