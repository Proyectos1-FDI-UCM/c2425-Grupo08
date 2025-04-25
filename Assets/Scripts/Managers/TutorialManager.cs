using System.Collections.Generic;
using UnityEngine;

public class TerminalManager : MonoBehaviour
{
    [Tooltip("Lista de terminales en orden")]
    [SerializeField] private List<Terminal> terminales = new List<Terminal>();

    [Tooltip("Prefab del enemigo que se activará")]
    [SerializeField] private GameObject enemy1PhantomAnglerfish;
    [Header("Jugador")]
    public PlayerMovement playerMovement;
    private void Start()
    {
        enemy1PhantomAnglerfish.SetActive(false);
        playerMovement.enabled= false;
        for (int i = 0; i < terminales.Count; i++)
        {
            int index = i;
            terminales[i].OnMessageComplete += () => TerminalFinalizado(index);
        }
    }

    private void TerminalFinalizado(int index)
    {
        if (index == 0)
        {
            playerMovement.enabled = true;
        }
        // Solo hacemos algo especial al finalizar el tercero (índice 2)
        if (index == 2 && enemy1PhantomAnglerfish != null)
        {
            enemy1PhantomAnglerfish.SetActive(true);
        }
    }
}
