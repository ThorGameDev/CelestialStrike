using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ActionBar : MonoBehaviour
{
    public RectTransform ThisRectTransform;
    public Player_Module CurrentUnit;
    public GameObject Text;
    public GameObject ButtonObj;
    public int State = -1;
    private Player_Module InternalUnit;
    private bool InternalMovement;
    // Update is called once per frame
    void Update()
    {
        ThisRectTransform.SetPositionAndRotation(new Vector3(ThisRectTransform.position.x, (ThisRectTransform.rect.height / 2), 0), Quaternion.identity);
        transform.localPosition = new Vector3(transform.localPosition.x,(ThisRectTransform.rect.height / 2)-300,0);
        if (InternalUnit != CurrentUnit)
        {
            InternalUnit = CurrentUnit;
            State = -1;
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
        if(CurrentUnit == null)
        {
            return;
        }
        if (CurrentUnit.InMovement)
        {
            if(InternalMovement == false)
            {
                for (int i = transform.childCount - 1; i >= 0; i--)
                {
                    Destroy(transform.GetChild(i).gameObject);
                }
                InternalMovement = true;
            }
            return;
        }
        InternalMovement = false;
        if(CurrentUnit.State != State)
        {
            State = CurrentUnit.State;
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
            if (State == 0)
            {
                GameObject obj = Instantiate(Text);
                obj.GetComponent<TMP_Text>().text = "Action!";
                obj.transform.SetParent(transform);

                obj = Instantiate(ButtonObj);
                obj.GetComponent<Button>().onClick.AddListener(() => CurrentUnit.ChangeState(1));
                obj.GetComponentInChildren<TMP_Text>().text = "Move";
                obj.transform.SetParent(transform);

                obj = Instantiate(ButtonObj);
                obj.GetComponent<Button>().onClick.AddListener(() => CurrentUnit.ChangeState(2));
                obj.GetComponentInChildren<TMP_Text>().text = "Attack";
                obj.transform.SetParent(transform);

                obj = Instantiate(ButtonObj);
                obj.GetComponent<Button>().onClick.AddListener(() => CurrentUnit.ChangeState(3));
                obj.GetComponentInChildren<TMP_Text>().text = "Look";
                obj.transform.SetParent(transform);
            }
            if(State == 1)
            {
                GameObject obj = Instantiate(Text);
                obj.GetComponent<TMP_Text>().text = "Move!";
                obj.transform.SetParent(transform);

                obj = Instantiate(ButtonObj);
                obj.GetComponent<Button>().onClick.AddListener(() => CurrentUnit.ChangeState(0));
                obj.GetComponentInChildren<TMP_Text>().text = "Cancel";
                obj.transform.SetParent(transform);
            }
            if(State == 2)
            {
                GameObject obj = Instantiate(Text);
                obj.GetComponent<TMP_Text>().text = "Attack!";
                obj.transform.SetParent(transform);
                for (int i = 0; i < CurrentUnit.Attacks.Length; i++)
                {
                    obj = Instantiate(ButtonObj);
                    int Choice = i;
                    obj.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        if (CurrentUnit != null)
                        {
                            CurrentUnit.UseAttack(Choice);
                        }
                    });
                    obj.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        if (CurrentUnit != null)
                        {
                            CurrentUnit.ChangeState(0);
                        }
                    } );
                    obj.GetComponentInChildren<TMP_Text>().text = CurrentUnit.Attacks[i].name;
                    obj.transform.SetParent(transform);
                }

                obj = Instantiate(ButtonObj);
                obj.GetComponent<Button>().onClick.AddListener(() => CurrentUnit.ChangeState(0));
                obj.GetComponentInChildren<TMP_Text>().text = "Cancel";
                obj.transform.SetParent(transform);
            }
            if (State == 3)
            {
                GameObject obj = Instantiate(Text);
                obj.GetComponent<TMP_Text>().text = "Looking!";
                obj.transform.SetParent(transform);

                obj = Instantiate(ButtonObj);
                obj.GetComponent<Button>().onClick.AddListener(() => CurrentUnit.ChangeState(0));
                obj.GetComponentInChildren<TMP_Text>().text = "Stop";
                obj.transform.SetParent(transform);
            }
        }
    }
}
