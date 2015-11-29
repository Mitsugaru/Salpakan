using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour
{

    public GameObject TutorialPanel;

    private bool showHelp = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            TutorialPanel.SetActive(!showHelp);
            showHelp = !showHelp;
        }
    }
}
