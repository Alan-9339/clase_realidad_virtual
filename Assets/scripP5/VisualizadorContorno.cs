using UnityEngine;
using Vuforia;

[RequireComponent(typeof(ObserverBehaviour))]
public class VisualizadorContorno : MonoBehaviour
{
    private ObserverBehaviour mObserverBehaviour;
    private LineRenderer lineRenderer;

    [Header("Configuración Visual")]
    public Color colorContorno = Color.green;
    public float grosorLinea = 0.02f;

    void Start()
    {
        mObserverBehaviour = GetComponent<ObserverBehaviour>();

        // Nos suscribimos al evento de cambio de estado de Vuforia
        if (mObserverBehaviour)
        {
            mObserverBehaviour.OnTargetStatusChanged += OnStatusChanged;
        }

        ConfigurarLineRenderer();
        ActualizarDimensiones();
        
        // Iniciamos oculto hasta que se detecte el target
        lineRenderer.enabled = false;
    }

    private void OnStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        // Si el target está siendo rastreado o está en rastreo extendido
        if (status.Status == Status.TRACKED || status.Status == Status.EXTENDED_TRACKED)
        {
            lineRenderer.enabled = true;
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }

    private void ConfigurarLineRenderer()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        if (lineRenderer == null) lineRenderer = gameObject.AddComponent<LineRenderer>();

        lineRenderer.startWidth = grosorLinea;
        lineRenderer.endWidth = grosorLinea;
        lineRenderer.useWorldSpace = false; // Para que se mueva con el Target
        lineRenderer.loop = true;
        lineRenderer.positionCount = 4;
        
        // Usamos un material simple (puedes crear uno Unlit para que brille más)
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = colorContorno;
        lineRenderer.endColor = colorContorno;
    }

    private void ActualizarDimensiones()
    {
        // Obtenemos el tamaño real del Image Target configurado en Vuforia
        ImageTargetBehaviour itb = GetComponent<ImageTargetBehaviour>();
        if (itb != null)
        {
            Vector2 size = itb.GetSize();
            float halfWidth = size.x / 2f;
            float halfHeight = size.y / 2f;

            // Definimos las 4 esquinas del plano (en el eje X y Z, Y suele ser 0)
            lineRenderer.SetPosition(0, new Vector3(-halfWidth, 0.01f, -halfHeight));
            lineRenderer.SetPosition(1, new Vector3(halfWidth, 0.01f, -halfHeight));
            lineRenderer.SetPosition(2, new Vector3(halfWidth, 0.01f, halfHeight));
            lineRenderer.SetPosition(3, new Vector3(-halfWidth, 0.01f, halfHeight));
        }
    }
}