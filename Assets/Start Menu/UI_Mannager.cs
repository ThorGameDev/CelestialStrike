using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class UI_Mannager : MonoBehaviour
{
    bool Loading = false;
    public void Play()
    {
        if (Loading) { return; }
        else { Loading = true; }
        StartCoroutine(FadeOut());
    }
    public Transform CanvasOBJ;
    public GameObject Fader;
    public IEnumerator FadeOut()
    {
        Instantiate(Fader).transform.SetParent(CanvasOBJ);
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("World Map");
    }
    public void Quit()
    {
        if (Loading) { return; }
        else { Loading = true; }
        Application.Quit();
    }
}
