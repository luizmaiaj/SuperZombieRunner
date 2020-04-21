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
        timeElapsed += Time.deltaTime;

        FormatScore(); // update the score after updating the time elapsed

        BlinkContinueText(); // blink the continue text (or not)

        if (!gameStarted && Time.timeScale == 0 && Input.anyKeyDown)
        {
            timeManager.ManipulateTime(1, 1f);
            ResetGame();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit ();
#endif
        }
    }

    void BlinkContinueText()
    {
        if (gameStarted)
        {
            continueText.canvasRenderer.SetAlpha(0f); // make continue text disappear
            return;
        }

        continueText.canvasRenderer.SetAlpha((++blinkTime % 80) / 79); // alpha is between 0 and 1
    }

    void FormatScore()
    {
        if (gameStarted)
        {
            scoreText.text = "TIME: " + FormatTime(timeElapsed);
            return;
        }

        string textColor = beatBestTime ? "#FF0" : "#FFF";

        scoreText.text = "TIME: " + FormatTime(timeElapsed) + "\n<color=" + textColor + ">BEST: " + FormatTime(bestTime) + "</color>";
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
