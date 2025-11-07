using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    //[SerializeField] GameObject shopUI;
    //[SerializeField] GameObject Healthbar;
    [SerializeField] Text dialogText;
    [SerializeField] int lettersPerSecond;
    [SerializeField] GameObject Healthbar;

    public event Action OnShowDialog;
    public event Action OnHideDialog;
    public static event Action OnEndDialog;

    bool isShopDialog = false;

    public static DialogManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    Dialog dialog;
    int currentLine = 0;
    bool isTyping;

    public IEnumerator ShowDialog(Dialog dialog, bool isShopDialog = false)
    {
        yield return new WaitForEndOfFrame();
        OnShowDialog?.Invoke();
        this.dialog = dialog;
        this.isShopDialog = isShopDialog;
        dialogBox.SetActive(true);
        Healthbar.SetActive(false);
        currentLine = 0;
        StartCoroutine(TypeDialog(dialog.Lines[currentLine]));
    }

    public void HandleUpdate()
    {
        if (Input.GetKeyUp(KeyCode.F) && !isTyping)
        {
            ++currentLine;
            if (currentLine < dialog.Lines.Count)
            {
                StartCoroutine(TypeDialog(dialog.Lines[currentLine]));
            }
            else
            {


                //currentLine=0;
                //OnHideDialog?.Invoke();
                //OnEnd?.Invoke();
                //Debug.Log("ende");
                //shopUI.SetActive(true);
                //OnHideDialog?.Invoke(); // Dialog Ende
                //OnEndDialog?.Invoke();


                if (isShopDialog)
                {
                    GameController.Instance.OpenShop();
                }
                else
                {
                    closeDialog();
                }
            }
        }
    }
    
    public void closeDialog()
    {
        dialogBox.SetActive(false);
        OnHideDialog?.Invoke();
        Healthbar.SetActive(true);
    }

    public IEnumerator TypeDialog(string line)
    {
        isTyping = true;
        dialogText.text = "";
        foreach (var letter in line.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
        isTyping = false;
    }
}


