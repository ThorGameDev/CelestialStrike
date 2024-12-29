using UnityEngine;

public class UniversalScript : MonoBehaviour
{
    private bool InQuit = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !InQuit)
        {
            InQuit = true;
            Application.Quit();
        }
    }
}
