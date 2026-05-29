using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HunterDialogueZone : MonoBehaviour, IInteractable
{
    [Header("Dialogue UI")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public Button nextButton;

    [Header("Dialogue Content")]
    [TextArea]
    public string[] dialogueLines ;
    public bool useSpeakerLabels = true;
    public string[] dialogueSpeakers ;

    [Header("Settings")]
    public bool hideOnExit = true;
    public bool autoAdvance = true;
    public float autoAdvanceDelay = 3f;
    public bool useNextButton = false;
    public bool useTypewriter = true;
    public float typewriterSpeed = 0.03f;
    public bool disableGunDuringDialogue = true;
    public bool disableAimDuringDialogue = true;
    public float autoHideAfterSeconds = 0f;

    private PlayerInteract currentPlayer;
    private GunSystem[] playerGunSystems;
    private AimSystem[] playerAimSystems;
    private FPSController playerController;
    private int currentLineIndex;
    private Coroutine dialogueCoroutine;
    private Coroutine hideCoroutine;
    private Coroutine typewriterCoroutine;
    private bool isTyping;

    void Awake()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        PlayerInteract player = other.GetComponentInParent<PlayerInteract>();
        if (player != null)
        {
            currentPlayer = player;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!hideOnExit)
            return;

        if (currentPlayer != null && other.GetComponentInParent<PlayerInteract>() == currentPlayer)
        {
            StopDialogue();
            currentPlayer = null;
        }
    }

    public void Interact(PlayerInteract player)
    {
        currentPlayer = player;
        StartDialogue();
        Debug.Log("Interacted with Hunter Dialogue Zone");
    }

    public void StartDialogue()
    {
        if (dialoguePanel == null || dialogueText == null)
            return;

        if (dialogueCoroutine != null)
        {
            StopCoroutine(dialogueCoroutine);
            dialogueCoroutine = null;
        }

        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }

        CollectPlayerSystems();

        if (disableGunDuringDialogue)
            SetGunControl(false);

        if (disableAimDuringDialogue)
            SetAimControl(false);

        SetCameraControl(false);

        currentLineIndex = 0;
        dialoguePanel.SetActive(true);
        EnableCursor();

        if (useNextButton && nextButton != null)
        {
            nextButton.onClick.RemoveListener(AdvanceDialogue);
            nextButton.onClick.AddListener(AdvanceDialogue);
            ShowCurrentLine();
        }
        else if (autoAdvance)
        {
            dialogueCoroutine = StartCoroutine(PlayDialogueSequence());
        }
        else
        {
            ShowCurrentLine();
        }
    }

    public void StopDialogue()
    {
        if (dialoguePanel == null)
            return;

        if (dialogueCoroutine != null)
        {
            StopCoroutine(dialogueCoroutine);
            dialogueCoroutine = null;
        }

        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }

        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = null;
            isTyping = false;
        }

        if (nextButton != null)
            nextButton.onClick.RemoveListener(AdvanceDialogue);

        dialoguePanel.SetActive(false);

        if (disableGunDuringDialogue)
            SetGunControl(true);

        if (disableAimDuringDialogue)
            SetAimControl(true);

        SetCameraControl(true);
        RestoreCursor();
    }

    void ShowCurrentLine()
    {
        if (dialogueLines == null || dialogueLines.Length == 0)
            return;

        currentLineIndex = Mathf.Clamp(currentLineIndex, 0, dialogueLines.Length - 1);
        string line = dialogueLines[currentLineIndex];
        string speaker = string.Empty;

        if (useSpeakerLabels && dialogueSpeakers != null && currentLineIndex < dialogueSpeakers.Length)
            speaker = dialogueSpeakers[currentLineIndex];

        string formattedLine = string.IsNullOrEmpty(speaker)
            ? line
            : $"<b>{speaker}:</b> {line}";

        if (useTypewriter)
        {
            StartTypewriter(formattedLine);
        }
        else
        {
            dialogueText.text = formattedLine;
        }
    }

    void StartTypewriter(string line)
    {
        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = null;
        }

        typewriterCoroutine = StartCoroutine(TypeText(line));
    }

    IEnumerator TypeText(string line)
    {
        isTyping = true;
        dialogueText.text = string.Empty;

        foreach (char letter in line)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typewriterSpeed);
        }

        isTyping = false;
        typewriterCoroutine = null;
    }

    void FinishTyping()
    {
        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = null;
        }

        if (dialogueLines != null && dialogueLines.Length > 0)
        {
            string line = dialogueLines[currentLineIndex];
            string speaker = string.Empty;

            if (useSpeakerLabels && dialogueSpeakers != null && currentLineIndex < dialogueSpeakers.Length)
                speaker = dialogueSpeakers[currentLineIndex];

            dialogueText.text = string.IsNullOrEmpty(speaker)
                ? line
                : $"<b>{speaker}:</b> {line}";
        }

        isTyping = false;
    }

    public void AdvanceDialogue()
    {
        if (dialogueLines == null || dialogueLines.Length == 0)
            return;

        if (isTyping)
        {
            FinishTyping();
            return;
        }

        currentLineIndex++;

        if (currentLineIndex >= dialogueLines.Length)
        {
            StopDialogue();
            return;
        }

        ShowCurrentLine();
    }

    void CollectPlayerSystems()
    {
        if (currentPlayer == null)
            return;

        playerGunSystems = currentPlayer.GetComponentsInChildren<GunSystem>(true);
        playerAimSystems = currentPlayer.GetComponentsInChildren<AimSystem>(true);
        playerController = currentPlayer.GetComponentInParent<FPSController>();
    }

    void SetGunControl(bool enabled)
    {
        if (playerGunSystems == null || playerGunSystems.Length == 0)
            return;

        foreach (GunSystem gun in playerGunSystems)
        {
            if (gun != null)
                gun.enabled = enabled;
        }
    }

    void SetAimControl(bool enabled)
    {
        if (playerAimSystems == null || playerAimSystems.Length == 0)
            return;

        foreach (AimSystem aim in playerAimSystems)
        {
            if (aim != null)
                aim.enabled = enabled;
        }
    }

    void SetCameraControl(bool enabled)
    {
        if (playerController == null)
            return;

        playerController.SetLookEnabled(enabled);
    }

    System.Collections.IEnumerator PlayDialogueSequence()
    {
        while (currentLineIndex < dialogueLines.Length)
        {
            ShowCurrentLine();

            if (useTypewriter)
                yield return new WaitUntil(() => !isTyping);

            if (currentLineIndex >= dialogueLines.Length - 1)
                break;

            yield return new WaitForSeconds(autoAdvanceDelay);
            currentLineIndex++;
        }

        if (autoHideAfterSeconds > 0f)
        {
            hideCoroutine = StartCoroutine(AutoHideAfterDelay(autoHideAfterSeconds));
        }
    }

    System.Collections.IEnumerator AutoHideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StopDialogue();
    }

    void EnableCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void RestoreCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
