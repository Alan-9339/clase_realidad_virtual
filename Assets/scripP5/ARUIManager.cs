using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ARUIManager : MonoBehaviour
{
    public TextMeshProUGUI displayTexto;

    [System.Serializable]
    public class Mensaje
    {
        public string key;
        [TextArea(2, 4)]
        public string texto;
    }

    public Mensaje[] mensajes;

    private Dictionary<string, string> diccionario;

    void Awake()
    {
        diccionario = new Dictionary<string, string>();

        foreach (var m in mensajes)
        {
            if (!diccionario.ContainsKey(m.key))
                diccionario.Add(m.key, m.texto);
        }
    }

    public void MostrarNarrativa(string key)
    {
        if (displayTexto == null) return;

        if (diccionario.ContainsKey(key))
        {
            displayTexto.text = diccionario[key];
        }
        else
        {
            displayTexto.text = "[Mensaje no definido: " + key + "]";
        }
    }

    public void MostrarTextoDirecto(string texto)
    {
        if (displayTexto != null)
            displayTexto.text = texto;
    }
}