using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { FreeRoam, Dialog, Shop }

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerMovement playerController;
    [SerializeField] DialogManager dialogManager;
    [SerializeField] ShopManager shopManager;

    public static GameController Instance { get; private set; }
    GameState state;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        dialogManager.OnShowDialog += () => { state = GameState.Dialog; };
        dialogManager.OnHideDialog += () => { state = GameState.FreeRoam; };
        //dialogManager.OnShopDialogEnd += () => { OpenShop(); };
    }
    private void Update()
    {
        if (state == GameState.FreeRoam)
        {
            playerController.HandleUpdate();
        }
        else if (state == GameState.Dialog){
            if (shopManager.isActive)
            {
                shopManager.HandleUpdate();
            }
            else
            {
                DialogManager.Instance.HandleUpdate();
            }
        }
    }

    public void OpenShop()
    {
        shopManager.OpenShop();
    }

    public void CloseShop()
    {
        shopManager.CloseShop();
        state = GameState.FreeRoam;
    }
}
