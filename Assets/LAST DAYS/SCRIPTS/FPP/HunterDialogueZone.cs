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
    public bool disableMacheteDuringDialogue = true;
    public float autoHideAfterSeconds = 0f;
    public float zombieSpawnDuration = 90f;

    private PlayerInteract currentPlayer;
    private GunSystem[] playerGunSystems;
    private AimSystem[] playerAimSystems;
    private MacheteSystem[] playerMacheteSystems;
    private FPSController playerController;
    private int currentLineIndex;
    private Coroutine dialogueCoroutine;
    private Coroutine hideCoroutine;
    private Coroutine typewriterCoroutine;
    private Coroutine zombieSpawnCoroutine;
    private bool isTyping;
    private bool dialogueEndedNaturally;

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

        if (zombieSpawnCoroutine != null)
        {
            StopCoroutine(zombieSpawnCoroutine);
            zombieSpawnCoroutine = null;
        }

        dialogueEndedNaturally = false;

        CollectPlayerSystems();

        if (disableGunDuringDialogue)
            SetGunControl(false);

        if (disableAimDuringDialogue)
            SetAimControl(false);

        if (disableMacheteDuringDialogue)
            SetMacheteControl(false);

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

        if (disableMacheteDuringDialogue)
            SetMacheteControl(true);

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
            dialogueEndedNaturally = true;
            ScheduleZombieSpawn();
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
        playerMacheteSystems = currentPlayer.GetComponentsInChildren<MacheteSystem>(true);
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

    void SetMacheteControl(bool enabled)
    {
        if (playerMacheteSystems == null || playerMacheteSystems.Length == 0)
            return;

        foreach (MacheteSystem machete in playerMacheteSystems)
        {
            if (machete != null)
                machete.enabled = enabled;
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

        if (!dialogueEndedNaturally)
        {
            dialogueEndedNaturally = true;
            ScheduleZombieSpawn();
        }
    }

    System.Collections.IEnumerator AutoHideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StopDialogue();
    }

    void ScheduleZombieSpawn()
    {
        if (zombieSpawnCoroutine != null)
            return;

        if (GameManager.Instance == null)
        {
            Debug.LogWarning("HunterDialogueZone: GameManager instance not found for zombie spawn.");
            return;
        }

        if (GameManager.Instance.ambientSpawner == null)
        {
            Debug.LogWarning("HunterDialogueZone: ambientSpawner reference missing in GameManager.");
            return;
        }

        GameManager.Instance.ambientSpawner.spawnInterval = 2f; // Set spawn interval to 2 seconds
        Debug.Log("HunterDialogueZone: Starting zombie spawn with " + GameManager.Instance.ambientSpawner.spawnInterval + " second interval.");
        GameManager.Instance.ambientSpawner.StartSpawn();

        Debug.Log("HunterDialogueZone: Zombie spawning started after dialogue completion.");
        zombieSpawnCoroutine = StartCoroutine(StopZombieSpawnAfterDelay(zombieSpawnDuration));
    }

    IEnumerator StopZombieSpawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (GameManager.Instance != null && GameManager.Instance.ambientSpawner != null)
        {
            GameManager.Instance.ambientSpawner.StopSpawn();
            Debug.Log("HunterDialogueZone: Zombie spawning stopped after 90 seconds.");
        }

        //nếu ko còn zombie thì hiện UI nhiệm vụ
        //Tìm tất cả zombie còn lại trong scene
        GameObject[] remainingZombies = GameObject.FindGameObjectsWithTag("Enemy");
        if (remainingZombies.Length == 0)
        {
            //hiện point kế
            MissionSystem.Instance.CompleteCurrentMission();

            //hiện UI nhiệm vụ kế
            GameManager.Instance.UIMission(3);
        }
        zombieSpawnCoroutine = null;
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
