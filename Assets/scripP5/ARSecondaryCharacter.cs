using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ARSecondaryCharacter : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject secondaryPrefab;
    public TextMeshProUGUI textoSecundario;

    public ARNarrativeManager narrativeManager;
    public Transform[] imageTargets;

    private GameObject instanciaActual;
    private int indiceAsignado;

    // --- INICIALIZAR ---
    public void Inicializar(int indiceMochila, int indiceEspada)
    {
        int index = ObtenerIndiceValido(indiceMochila, indiceEspada);

        indiceAsignado = index;

        Transform target = imageTargets[index];

        instanciaActual = Instantiate(secondaryPrefab, target);
        instanciaActual.transform.localPosition = Vector3.zero;
        instanciaActual.transform.localRotation = Quaternion.identity;

        MostrarDialogoInicial();
    }

    int ObtenerIndiceValido(int mochila, int espada)
    {
        List<int> indicesValidos = new List<int>();

        for (int i = 0; i < imageTargets.Length; i++)
        {
            if (i == 0) continue; // evitar inicio
            if (i == mochila) continue;
            if (i == espada) continue;

            indicesValidos.Add(i);
        }

        return indicesValidos[Random.Range(0, indicesValidos.Count)];
    }

    // --- DIÁLOGOS ---

    public void MostrarDialogoInicial()
    {
        if (textoSecundario != null)
            textoSecundario.text = "Te espero mientras buscas tus cosas";
    }

    public void EvaluarEstado()
    {
        if (textoSecundario == null || narrativeManager == null) return;

        if (!narrativeManager.tieneMochila)
        {
            textoSecundario.text = "Te espero mientras buscas tus cosas";
        }
        else if (narrativeManager.tieneMochila && !narrativeManager.tieneEspada)
        {
            textoSecundario.text = "Te falta algo importante...";
        }
        else if (narrativeManager.tieneMochila && narrativeManager.tieneEspada)
        {
            textoSecundario.text = " ";
        }
    }
}