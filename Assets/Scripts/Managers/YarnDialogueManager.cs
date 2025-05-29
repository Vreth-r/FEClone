using Yarn.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class YarnDialogueController : MonoBehaviour
{
    [Header("UI References")]
    public DialogueRunner dialogueRunner;

    public Image leftPortrait;
    public Image rightPortrait;

    [Header("Portraits")]
    public CharacterPortraitLibrary portraitLibrary;

    private void Start()
    {
        dialogueRunner.onNodeComplete.AddListener(OnDialogueComplete);
        // Bind UI to Yarn script
        dialogueRunner.onDialogueStart.AddListener(OnLineStart);
    }

    private void OnLineStart()
    {
        return; // im using this script to learn yarn so im keeping things i feel i need to know about later
    }

    public void OnDialogueComplete(string nodeName)
    {
        Debug.Log($"Dialogue node '{nodeName}' complete.");
        // add more later if needed 
    }

    [YarnCommand("setportrait")]
    public void SetPortrait(string speaker, string expression)
    {
        Sprite sprite = portraitLibrary.GetPortrait(speaker, expression);
        dialogueRunner.VariableStorage.TryGetValue("$side", out string side);
        if (side == "left")
        {
            leftPortrait.sprite = sprite;
            leftPortrait.color = Color.white;
        }
        else
        {
            rightPortrait.sprite = sprite;
            rightPortrait.color = Color.white;
        }
    }

/*
    public void OnContinueClicked()
    {
        dialogueRunner.Continue();
    } */
}