using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Danger : MonoBehaviour
{
    public Image image; 
    public float pulseSpeed = 1f; 
    public float minAlpha = 0.2f; 
    public float maxAlpha = 1f;

    public GameObject danger;
    private void OnEnable(){
        PlayerMovement.OnDanger += EnableDangerZone;
    }
    private void OnDisable(){
        PlayerMovement.OnDanger -= EnableDangerZone;
    }
    public void EnableDangerZone(){
        danger.SetActive(true);
        Update();
    }

    private bool fadingOut = true; // Richtung des Pulsierens
    private void Update(){
        if (image != null)
        {
            // Aktuelle Farbe holen
            Color color = image.color;

            // Alpha-Wert berechnen
            if (fadingOut)
            {
                color.a -= pulseSpeed * Time.deltaTime; // Reduziere Alpha
                if (color.a <= minAlpha) fadingOut = false; // Richtung umkehren
            }
            else
            {
                color.a += pulseSpeed * Time.deltaTime; // Erhöhe Alpha
                if (color.a >= maxAlpha) fadingOut = true; // Richtung umkehren
            }

            // Neue Farbe setzen
            image.color = color;
        }
    }
    
}
