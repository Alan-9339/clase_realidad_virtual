using System.Collections;
using UnityEngine;
using Vuforia;

public class Move : MonoBehaviour
{
    public GameObject model;
    public ObserverBehaviour[] ImageTargets;
    public int currentTarget = 0; 
    public float speed = 1.0f;
    public float rotationSpeed = 5.0f; // Velocidad para que el giro sea suave

    private bool isMoving = false;

    public void MoveToNextMarker()
    {
        if (!isMoving)
        {
            StartCoroutine(MoveModel());
        }
    }

    private IEnumerator MoveModel()
    {
        isMoving = true;

        int nextIndex = (currentTarget + 1) % ImageTargets.Length;
        ObserverBehaviour target = ImageTargets[nextIndex];

        if (target == null || !IsTargetTracked(target))
        {
            Debug.LogWarning("Marcador no detectado.");
            isMoving = false;
            yield break;
        }

        // --- 1. ROTACIÓN HACIA EL OBJETIVO ---
        Vector3 targetDirection = target.transform.position - model.transform.position;
        // Ignoramos la diferencia de altura (Y) para que no se incline hacia adelante o atrás
        targetDirection.y = 0; 

        if (targetDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            
            // Girar suavemente hasta que estemos mirando casi de frente al marcador
            while (Quaternion.Angle(model.transform.rotation, targetRotation) > 0.1f)
            {
                model.transform.rotation = Quaternion.Slerp(model.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
                yield return null;
            }
        }

        // --- 2. MOVIMIENTO (Tu lógica original con una mejora) ---
        Vector3 startPosition = model.transform.position;
        float journey = 0f;

        while (journey < 1f)
        {
            journey += Time.deltaTime * speed;
            Vector3 endPosition = target.transform.position;
            
            // Mantenemos la rotación mirando al objetivo por si mueves el marcador mientras camina
            Vector3 dynamicDir = endPosition - model.transform.position;
            dynamicDir.y = 0;
            if(dynamicDir != Vector3.zero)
            {
                model.transform.rotation = Quaternion.LookRotation(dynamicDir);
            }

            model.transform.position = Vector3.Lerp(startPosition, endPosition, journey);
            yield return null;
        }

        model.transform.position = target.transform.position;
        currentTarget = nextIndex;
        isMoving = false;
    }

    private bool IsTargetTracked(ObserverBehaviour target)
    {
        var status = target.TargetStatus.Status;
        return status == Status.TRACKED || status == Status.EXTENDED_TRACKED;
    }
}