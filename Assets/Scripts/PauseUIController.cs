using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseUIController : MonoBehaviour
{
    public GameObject pauseUI;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            gameObject.SetActive(false);
            pauseUI.SetActive(true);
        }
    }

    public void clickContinue()
    {
        gameObject.SetActive(true);
        pauseUI.SetActive(false);
    }

    public void clickEnd()
    {
        SceneManager.LoadScene(0);
    }

    public void clickSetting()
    {

    }
}
