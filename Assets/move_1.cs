using System.Collections;
using UnityEngine;
using Vuforia;

public class move_1 : MonoBehaviour
{
    public GameObject model;
    public ObserverBehaviour[] ImageTargets;
    public int currentTarget = 0; // Índice del marcador donde está parado actualmente
    public float speed = 1.0f;

    private bool isMoving = false;

    public void MoveToNextMarker()
    {
        // Solo permitimos el movimiento si no se está moviendo ya
        if (!isMoving)
        {
            StartCoroutine(MoveModel());
        }
    }

    private IEnumerator MoveModel()
    {
        isMoving = true;

        // Calculamos el siguiente índice en el ciclo (0 al 4)
        int nextIndex = (currentTarget + 1) % ImageTargets.Length;
        ObserverBehaviour target = ImageTargets[nextIndex];

        // Verificamos si el target existe y si Vuforia lo está viendo (Tracked)
        if (target == null || !IsTargetTracked(target))
        {
            Debug.LogWarning("El siguiente marcador (" + nextIndex + ") no está siendo detectado por la cámara.");
            isMoving = false;
            yield break;
        }

        Vector3 startPosition = model.transform.position;
        float journey = 0f;

        while (journey < 1f)
        {
            journey += Time.deltaTime * speed;
            
            // Actualizamos la posición final en cada frame por si mueves el marcador con la mano
            Vector3 endPosition = target.transform.position;
            
            model.transform.position = Vector3.Lerp(startPosition, endPosition, journey);
            yield return null;
        }

        // Aseguramos posición final exacta
        model.transform.position = target.transform.position;

        currentTarget = nextIndex;

        isMoving = false;
        Debug.Log("Llegamos al marcador: " + currentTarget);
    }

    // Función auxiliar para verificar el estado de rastreo
    private bool IsTargetTracked(ObserverBehaviour target)
    {
        var status = target.TargetStatus.Status;
        return status == Status.TRACKED || status == Status.EXTENDED_TRACKED;
    }
}