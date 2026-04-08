using System.Collections;
using UnityEngine;
using Vuforia;

public class Move_2 : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject model;
    public Animator animator;
    public ObserverBehaviour target1; // Marcador de inicio
    public ObserverBehaviour target2; // Marcador de destino

    [Header("Configuración")]
    public float speed = 1.0f;
    public float rotationSpeed = 7.0f;
    public string walkParameter = "isWalking"; // Nombre del booleano en tu Animator

    private bool isMoving = false;

    public void StartTransition()
    {
        if (!isMoving) StartCoroutine(NavigationRoutine());
    }

    private IEnumerator NavigationRoutine()
    {
        isMoving = true;

        // 1. Activar animación de caminata
        if (animator != null) animator.SetBool(walkParameter, true);

        // 2. Rotación hacia el segundo marcador
        Vector3 targetDir = target2.transform.position - model.transform.position;
        targetDir.y = 0;
        
        if (targetDir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(targetDir);
            while (Quaternion.Angle(model.transform.rotation, targetRot) > 0.1f)
            {
                model.transform.rotation = Quaternion.Slerp(model.transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
                yield return null;
            }
        }

        // 3. Traslación
        Vector3 startPos = model.transform.position;
        float journey = 0f;

        while (journey < 1f)
        {
            journey += Time.deltaTime * speed;
            // Lerp hacia la posición actual de destino (por si se mueve el marcador)
            model.transform.position = Vector3.Lerp(startPos, target2.transform.position, journey);
            
            // Mantener rotación actualizada
            Vector3 dynamicDir = target2.transform.position - model.transform.position;
            dynamicDir.y = 0;
            if(dynamicDir != Vector3.zero) model.transform.rotation = Quaternion.LookRotation(dynamicDir);
            
            yield return null;
        }

        // 4. Cambio de Jerarquía (Parenting)
        // Al llegar, el modelo se vuelve hijo del segundo marcador
        model.transform.SetParent(target2.transform, true); 
        
        // 5. Finalizar animación y estado
        if (animator != null) animator.SetBool(walkParameter, false);
        
        isMoving = false;
        Debug.Log("Transferencia de marcador completada.");
    }
}