using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public Text continueText;
    public Text scoreText;

    private bool beatBestTime = false;
    private float timeElapsed = 0f;
    private float bestTime = 0f;
    private float blinkTime = 0f;
    private float blink = 0f;
    private bool gameStarted = false;
    private TimeManager timeManager;
    private GameObject player;
    private GameObject floor;
    private Spawner spawner;

    private void Awake()
    {
        floor = GameObject.Find("Foreground");
        spawner = GameObject.Find("Spawner").GetComponent<Spawner>();
        timeManager = GetComponent<TimeManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        var floorHeight = floor.transform.localScale.y;

        var pos = floor.transform.position;
        pos.x = 0;
        pos.y = -((Screen.height / PixelPerfectCamera.pixelsToUnits) / 2) + floorHeight / 2;
        floor.transform.position = pos;

        spawner.active = false;

        Time.timeScale = 0f;

        continueText.text = "PRESS ANY BUTTON TO START";

        bestTime = PlayerPrefs.GetFloat("BestTime");
    }

    // Update is called once per frame
    void Update()
    {
        if(!gameStarted && Time.timeScale == 0)
        {
            if(Input.anyKeyDown)
            {
                timeManager.ManipulateTime(1, 1f);
                ResetGame();
            }
        }

        if(!gameStarted)
        {
            if (++blinkTime % 40 == 0)
            {
                blink = blink > 0 ? 0f : 1f;
            }

            continueText.canvasRenderer.SetAlpha(blink);

            var textColor = beatBestTime ? "#FF0" : "#FFF";

            scoreText.text = "TIME: " + FormatTime(timeElapsed) + "\n<color=" + textColor + ">BEST: " + FormatTime(bestTime) + "</color>";
        }
        else
        {
            timeElapsed += Time.deltaTime;
            scoreText.text = "TIME: " + FormatTime(timeElapsed);
        }
    }

    void OnPlayerKilled()
    {
        spawner.active = false;

        var playerDestroyScript = player.GetComponent<DestroyOffscreen>();
        playerDestroyScript.DescroyCallback -= OnPlayerKilled;

        player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        timeManager.ManipulateTime(0, 5.5f);

        gameStarted = false;

        continueText.text = "PRESS ANY BUTTON TO RESTART";

        if (timeElapsed > bestTime)
        {
            bestTime = timeElapsed;
            PlayerPrefs.SetFloat("BestTime", bestTime);
            beatBestTime = true;
        }
    }

    void ResetGame()
    {
        spawner.active = true;

        player = GameObjectUtil.Instantiate(playerPrefab, new Vector3(0, (Screen.height / PixelPerfectCamera.pixelsToUnits) / 2, 0));

        var playerDestroyScript = player.GetComponent<DestroyOffscreen>();
        playerDestroyScript.DescroyCallback += OnPlayerKilled;

        gameStarted = true;

        continueText.canvasRenderer.SetAlpha(0);

        timeElapsed = 0f;
    }

    string FormatTime(float value)
    {
        TimeSpan t = TimeSpan.FromSeconds(value);

        return string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
    }
}
