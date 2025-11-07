using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [SerializeField] GameObject shopUI;
    public GameObject[] selectionArrows; // Länge 5
    public GameObject[] infoCards;
    [SerializeField] List<ShopItem> items;
    [SerializeField] GameController gameController;

    private int currentIndex = 0;
    public bool isActive = false;

    private int selectedIndex = 0;


    public static ShopManager Instance { get; private set; }

    private void Start()
    {
        // shopUI.SetActive(false);
    }

    public void OpenShop()
    {
        isActive = true;
        shopUI.SetActive(true);
        currentIndex = 0;
        UpdateUI();
    }

    public void CloseShop()
    {
        isActive = false;
        shopUI.SetActive(false);
        gameController.CloseShop();
    }

    public void HandleUpdate()
    {
        if (!isActive) return;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            ChangeSelection(-1);
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            ChangeSelection(1);
        else if (Input.GetKeyDown(KeyCode.F))
            AttemptPurchase();

    }

    private void ChangeSelection(int direction)
    {
        //currentIndex = Mathf.Clamp(currentIndex + direction, 0, items.Count - 1);
        currentIndex = currentIndex + direction;
        if (currentIndex == 5) { currentIndex = 0; }
        if (currentIndex == -1) { currentIndex = 4; }
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (currentIndex != 4)
        {
            // Pfeil für aktuellen Index aktivieren
            // Alle Karten ausschalten
            foreach (GameObject card in infoCards)
                card.SetActive(false);

            // Karten nur anzeigen, wenn der Index < infoCards.Length
            infoCards[currentIndex].SetActive(true);

        }
        else { HideAllInfoCards(); }

        foreach (GameObject arrow in selectionArrows)
            arrow.SetActive(false);

        selectionArrows[currentIndex].SetActive(true);
    }

    private void HideAllInfoCards()
    {
        foreach (GameObject card in infoCards)
            card.SetActive(false);
    }

    private void AttemptPurchase()
    {
        if (currentIndex == 4)
        {
            CloseShop();
            return;
        }
        var item = items[currentIndex];

        if (item.isPurchased)
        {
            DialogManager.Instance.StartCoroutine(
                DialogManager.Instance.ShowDialog(new Dialog(new List<string> { "Sold Out!" }))
            );
            return;
        }

        var playerStats = PlayerStats.Instance;
        if (playerStats.money >= item.price)
        {
            PlayerStats player = FindObjectOfType<PlayerStats>();
            player.AddMoney(item.price * -1);
            playerStats.atk += item.atkBonus;
            playerStats.def += item.defBonus;
            item.isPurchased = true;

            DialogManager.Instance.StartCoroutine(
                DialogManager.Instance.ShowDialog(new Dialog(new List<string> { $"Gekauft!" }))
            );
        }
        else
        {
            DialogManager.Instance.StartCoroutine(
                DialogManager.Instance.ShowDialog(new Dialog(new List<string> { "Nicht genug Gold!" }))
            );
        }
    }
}
