using UnityEngine;

public class ARNarrativeManager : MonoBehaviour
{
    public enum EstadoNarrativa
    {
        Inicio,
        BuscandoMochila,
        BuscandoEspada,
        BuscandoNPC,
        Final
    }

    [Header("Referencias")]
    public ARUIManager uiManager;

    [Header("Estado")]
    public EstadoNarrativa estadoActual = EstadoNarrativa.Inicio;

    public bool tieneMochila = false;
    public bool tieneEspada = false;

    [Header("Indices de Objetos")]
    public int indiceMochila;
    public int indiceEspada;

    private int lastIndex = -1;
    public void IniciarNarrativa()
    {
        estadoActual = EstadoNarrativa.BuscandoMochila;
        uiManager.MostrarNarrativa("inicio");
    }

    public void EvaluarTarget(int index)
    {
        if (index == lastIndex) return;
        lastIndex = index;

        switch (estadoActual)
        {
            case EstadoNarrativa.BuscandoMochila:
                EvaluarMochila(index);
                break;

            case EstadoNarrativa.BuscandoEspada:
                EvaluarEspada(index);
                break;

            case EstadoNarrativa.BuscandoNPC:
                EvaluarNPC(index);
                break;

            case EstadoNarrativa.Final:
                uiManager.MostrarNarrativa("final");
                break;
        }
    }

    void EvaluarMochila(int index)
    {
        if (index == indiceMochila)
        {
            tieneMochila = true;
            estadoActual = EstadoNarrativa.BuscandoEspada;

            uiManager.MostrarNarrativa("mochila_encontrada");
        }
        else
        {
            uiManager.MostrarNarrativa("nada");
        }
    }

    void EvaluarEspada(int index)
    {
        if (index == indiceEspada)
        {
            tieneEspada = true;
            estadoActual = EstadoNarrativa.BuscandoNPC;

            uiManager.MostrarNarrativa("espada_ok");
        }
        else
        {
            uiManager.MostrarNarrativa("buscando_espada");
        }
    }

    void EvaluarNPC(int index)
    {
        estadoActual = EstadoNarrativa.Final;
        uiManager.MostrarNarrativa("encuentro_final");
    }

    // --- CONTROL ---
    public bool PuedeContinuar()
    {
        return estadoActual != EstadoNarrativa.Final;
    }

    // --- RESET ---
    public void ResetNarrativa()
    {
        tieneMochila = false;
        tieneEspada = false;
        lastIndex = -1;

        estadoActual = EstadoNarrativa.Inicio;

        IniciarNarrativa();
    }
}