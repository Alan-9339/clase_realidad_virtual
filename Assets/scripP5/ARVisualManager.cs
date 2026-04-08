using UnityEngine;

public class ARVisualManager : MonoBehaviour
{
    public Renderer modelRenderer;

    public void CambiarColor(Color nuevoColor)
    {
        if (modelRenderer != null)
        {
            modelRenderer.material.color = nuevoColor;
        }
    }
    public void CambiarColorAleatorio()
    {
        Color colorRandom = new Color(
            Random.value, 
            Random.value, 
            Random.value
        );

        CambiarColor(colorRandom);
    }

    public void CambiarColorPorIndice(int index)
    {
        CambiarColorAleatorio();
    }
}