﻿using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    bool gameHasEnded = false;
    bool instantReplay = false;

    public float restartDelay = 1f;
    public float replayStartTime;

    public GameObject completeLevelUI;
    public GameObject commandLogUI;
    public GameObject replayUI;

    public GameObject cannons;

    public Color playerColor;

    GameObject player;

    private void OnEnable()
    {
        PlayerCollision.OnHitObstalce += EndGame;
        PlayerMovement.OnMilestone += ChangeColor;
        PlayerMovement.OnCannonReach += FireConfetti;
        EndTrigger.OnRickRoll += RickRoll;
    }

    void RickRoll()
    {
        EndTrigger.OnRickRoll -= RickRoll;
        Application.OpenURL("https://www.youtube.com/watch?v=dQw4w9WgXcQ");
    }

    void FireConfetti()
    {
        PlayerMovement.OnCannonReach -= FireConfetti;
        cannons.SetActive(true);
    }

    private void OnDisable()
    {
        PlayerCollision.OnHitObstalce -= EndGame;
        PlayerMovement.OnMilestone -= ChangeColor;
        PlayerMovement.OnCannonReach -= FireConfetti;
        EndTrigger.OnRickRoll -= RickRoll;
    }

    void Start()
    {
        player = FindObjectOfType<PlayerMovement>().gameObject;
        playerColor = player.GetComponent<Renderer>().material.color;
        replayUI.SetActive(false);
        cannons.SetActive(false);

        if (CommandLog.commands.Count > 0)
        {
            instantReplay = true;
            replayStartTime = Time.timeSinceLevelLoad;
        }
    }

    void Update()
    {
        if (instantReplay)
        {
            replayUI.SetActive(true);
            RunInstantReplay();
        }
    }

    public void CompleteLevel()
    {
        instantReplay = false;
        completeLevelUI.SetActive(true);
        replayUI.SetActive(false);
        commandLogUI.SetActive(false);
    }

    public void EndGame(Collision collisionInfo)
    {
        player.GetComponent<PlayerMovement>().enabled = false;
        PlayerCollision.OnHitObstalce -= EndGame;

        if (!gameHasEnded)
        {
            gameHasEnded = true;
            Invoke("Restart", restartDelay);
        }
    }

    public void ChangeColor()
    {
        PlayerMovement.OnMilestone -= ChangeColor;
        player.GetComponent<Renderer>().material.color = Color.blue;
    }

    void RunInstantReplay()
    {
        if(CommandLog.commands.Count == 0)
        {
            instantReplay = false;
            return;
        }

        Command command = CommandLog.commands.Peek();
        if(Time.timeSinceLevelLoad >= command.timeStamp)
        {
            command = CommandLog.commands.Dequeue();
            command._player = player.GetComponent<Rigidbody>();
            Invoker invoker = new Invoker();
            invoker.disableLog = true;
            invoker.SetCommand(command);
            invoker.ExecuteCommand();
        }
    }

    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
