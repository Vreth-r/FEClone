using Yarn.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// class to manage anything for the dialogue runner in the scene, as well anything really related to dialogue UI, gameplay, or animation wise
// this script will be messy i have no idea how to use yarn at this point lmao

public class YarnDialogueController : MonoBehaviour
{
    [Header("UI References")]
    public DialogueRunner dialogueRunner;

    public Image leftPortrait;
    public Image rightPortrait;

    [Header("Portraits")]
    public CharacterPortraitLibrary portraitLibrary;

    // start runs AFTER the dialogue starts, so the listeners dont even get added in time lmao, so you HAVE to do this in awake
    private void Awake()
    {
        leftPortrait.gameObject.SetActive(false);
        rightPortrait.gameObject.SetActive(false);
        dialogueRunner.onDialogueComplete.AddListener(OnDialogueComplete);
        dialogueRunner.onDialogueStart.AddListener(OnLineStart);
    }

    public void OnLineStart()
    {
        leftPortrait.gameObject.SetActive(true);
        rightPortrait.gameObject.SetActive(true);
        // im using this script to learn yarn so im keeping things i feel i need to know about later
    }

    // adding this method in editor makes it trigger an extra time with an empty param, dunno why.
    // this is procing after the first node and makes the portraits dissapear for the choices.
    public void OnDialogueComplete()
    {
        leftPortrait.gameObject.SetActive(false);
        rightPortrait.gameObject.SetActive(false);
        // add more later if needed 
    }

    [YarnCommand("setportrait")]
    public void SetPortrait()
    {
        dialogueRunner.VariableStorage.TryGetValue("$speaker", out string speaker);
        dialogueRunner.VariableStorage.TryGetValue("$expression", out string expression);
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
            dialogueRunner.Continue(); // this doesnt exist because well fuck me right
        } */
}