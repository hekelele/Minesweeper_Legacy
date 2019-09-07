using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoScene : MonoBehaviour
{
    public float jumpTime = 6f;

    private void Start()
    {
        jumpTime += Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Time.time > jumpTime)
        {
            SceneManager.LoadScene("game");
        }
    }
}
