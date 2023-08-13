using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class finishCheckPoint : MonoBehaviour
{
    public GameObject prompt;

    public void show(GameObject Text)
    {
        prompt.SetActive(true);
    }

    public void hide(GameObject Text)
    {
        prompt.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        show(prompt);

    }

    public void LoadGameScene()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(2);

        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        hide(prompt);
    }
    // Start is called before the first frame update

    
    void Start()
    {
        hide(prompt);
    }

    // Update is called once per frame
    void Update()
    {
        LoadGameScene();
    }
}
