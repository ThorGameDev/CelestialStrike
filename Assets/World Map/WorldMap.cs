using UnityEngine;
using TMPro;
using System.Collections;

public class WorldMap : MonoBehaviour
{
    public Battle[] Battles;
    public int CurrentBattle;
    bool Loading = false;
    public SpriteRenderer Diamond;
    public Color Cleared;
    public Color Locked;
    public Color Uncleared;
    public TMP_Text title;
    public void Start()
    {
        ColorGem();
    }
    void Update()
    {
        if (Loading) { return; }
        if (Input.GetKeyDown(KeyCode.LeftArrow)) { Left(); }
        if (Input.GetKeyDown(KeyCode.RightArrow)) { Right(); }
        if (Input.GetKeyDown(KeyCode.Space)){ Confirm(); }
    }
    public void Left()
    {
        if (Loading) { return; }
        if (CurrentBattle == 0)
        {
            CurrentBattle = Battles.Length - 1;
        }
        else
        {
            CurrentBattle -= 1;
        }
        ColorGem();
    }
    public void Right()
    {
        if (Loading) { return; }
        if (CurrentBattle == Battles.Length - 1)
        {
            CurrentBattle = 0;
        }
        else
        {
            CurrentBattle += 1;
        }
        ColorGem();
    }
    private void ColorGem()
    {
        if (Loading) { return; }
        int Current = PlayerPrefs.GetInt("CurrentBattle");
        if(Current < CurrentBattle)
        {
            Diamond.color = Locked;
        }
        else if(Current == CurrentBattle)
        {
            Diamond.color = Uncleared;
        }
        else
        {
            Diamond.color = Cleared;
        }
        title.text = Battles[CurrentBattle].name;
    }
    public void Confirm()
    {
        if (Loading) { return; }
        if (PlayerPrefs.GetInt("CurrentBattle") < CurrentBattle)
        {
            return;
        }
        StaticVariables.currentBattle = Battles[CurrentBattle];
        StaticVariables.BattleID = CurrentBattle;
        StartCoroutine(FadeOut());
        Loading = true;
    }
    public Transform CanvasOBJ;
    public GameObject Fader;
    public IEnumerator FadeOut()
    {
        Instantiate(Fader).transform.SetParent(CanvasOBJ);
        yield return new WaitForSeconds(2);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Battle");
    }
}
