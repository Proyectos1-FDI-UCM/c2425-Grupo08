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
    [Header("Player")]
    [SerializeField] private GameObject player;

    [Header("Terminal Settings")]
    [SerializeField] private GameObject terminalPrefab;
    [SerializeField] private List<TerminalInfo> terminalsInfo;

    private List<GameObject> _spawnedTerminals = new List<GameObject>();

    void Start()
    {
        // Desactiva el movimiento del jugador al inicio
        if (player != null)
        {
            PlayerMovement movement = player.GetComponent<PlayerMovement>();
            if (movement != null)
                movement.enabled = false;
        }

        bool first = true;

        foreach (TerminalInfo info in terminalsInfo)
        {
            GameObject terminalInstance = Instantiate(terminalPrefab, info.spawnPoint.position, info.spawnPoint.rotation);
            _spawnedTerminals.Add(terminalInstance);

            Terminal terminalComponent = terminalInstance.GetComponent<Terminal>();
            if (terminalComponent != null)
            {
                terminalComponent.SetMessage(info.message);
                terminalComponent.Write(info.message);

                if (first)
                {
                    terminalComponent.OnMessageComplete += () =>
                    {
                        if (player != null)
                        {
                            PlayerMovement movement = player.GetComponent<PlayerMovement>();
                            if (movement != null)
                                movement.enabled = true;
                        }
                    };

                    first = false;
                }
            }
        }
    }
}
