using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class TerminalInfo
{
    public Transform spawnPoint;
    [TextArea(2, 5)]
    public string message;
}

public class TutorialManager : MonoBehaviour
{

    public static TutorialManager Instance { get; private set; }

    [Header("Player")]
    [SerializeField] private GameObject player;

    [Header("Phantom Fish")]
    [SerializeField] private GameObject phantomFish;

    [Header("Terminal Settings")]
    [SerializeField] private GameObject terminalPrefab;
    [SerializeField] private List<TerminalInfo> terminalsInfo;

    private List<GameObject> _spawnedTerminals = new List<GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

 

    void Start()
    {
        for (int i = 0; i < terminalsInfo.Count; i++)
        {
            int index = i;

            GameObject terminalInstance = Instantiate(
                terminalPrefab,
                terminalsInfo[index].spawnPoint.position,
                terminalsInfo[index].spawnPoint.rotation
            );

            Terminal terminalComponent = terminalInstance.GetComponent<Terminal>();
            if (terminalComponent != null)
            {
                terminalComponent.SetMessage(terminalsInfo[index].message);
                terminalComponent.OnMessageComplete += () =>
                {
                    EnablePlayerMovement();
                };
            }

            _spawnedTerminals.Add(terminalInstance);
        }

        // Terminal 6 empieza desactivado
        if (_spawnedTerminals.Count >= 6)
        {
            _spawnedTerminals[5].SetActive(false);
        }
    }

    public void OnLevelCompleted()
    {
        if (_spawnedTerminals.Count >= 4)
            _spawnedTerminals[3].SetActive(false); // Terminal 4

        if (_spawnedTerminals.Count >= 6)
            _spawnedTerminals[5].SetActive(true); // Terminal 6
    }

    private void EnablePlayerMovement()
    {
        if (player != null)
        {
            PlayerMovement movement = player.GetComponent<PlayerMovement>();
            if (movement != null)
                movement.enabled = true;
        }
    }
}
