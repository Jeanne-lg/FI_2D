using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    public GameObject GameOverMenu;
    public PlayerMovement playerMovement;

    private void OnEnable(){
        //DialogManager.OnEnd += EnableGameOverMenu;
        PlayerMovement.OnDeath += EnableGameOverMenu;
    }
    private void OnDisable(){
        DialogManager.OnEndDialog -= EnableGameOverMenu;
        PlayerMovement.OnDeath -= EnableGameOverMenu;
    }
    public void EnableGameOverMenu(){
        GameOverMenu.SetActive(true);
        playerMovement.canMove = false; // Bewegung deaktivieren
        Debug.Log(playerMovement.canMove);
    }
    public void DisableGameOverMenu(){
        GameOverMenu.SetActive(false);
        playerMovement.canMove = true; // Bewegung deaktivieren
        Debug.Log("Game Reset");
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }
}