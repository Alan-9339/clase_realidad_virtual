using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    [Header("Modelos para Color")]
    public GameObject brazoDerecho;
    public GameObject mochila;
    public GameObject gorro;

    [Header("Accesorios (Cambio de Modelo)")]
    public GameObject[] accesorios; 

    [Header("Configuración")]
    public Material colorMaterial;

    void Awake()
    {
        DesactivarTodosLosAccesorios();
    }

    public void ChangeColor_BTN()
    {
        Color randomColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        int indiceAleatorio = Random.Range(0, 3);

        GameObject objetoSeleccionado = null;
        switch (indiceAleatorio)
        {
            case 0: objetoSeleccionado = brazoDerecho; break;
            case 1: objetoSeleccionado = mochila; break;
            case 2: objetoSeleccionado = gorro; break;
        }

        if (objetoSeleccionado != null)
            objetoSeleccionado.GetComponent<Renderer>().material.color = randomColor;
    }

    public void ChangeAccessory_BTN()
    {
        if (accesorios.Length == 0) return;

        DesactivarTodosLosAccesorios();

        int indiceAleatorio = Random.Range(0, accesorios.Length);
        if (accesorios[indiceAleatorio] != null)
        {
            accesorios[indiceAleatorio].SetActive(true);
        }
    }

    private void DesactivarTodosLosAccesorios()
    {
        foreach (GameObject acc in accesorios)
        {
            if (acc != null) acc.SetActive(false);
        }
    }
}