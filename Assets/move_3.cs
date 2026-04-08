using System.Collections;
using UnityEngine;
using TMPro;
using Vuforia;

public class move_3 : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI displayTexto;
    public string[] mensajes; 

    [Header("Configuración")]
    public GameObject model;
    public ObserverBehaviour[] ImageTargets;
    public float speed = 1.0f;
    public float rotationSpeed = 7.0f;

    [Header("Accesorios")]
    public GameObject prefabMochila;
    public GameObject prefabEspada;
    public Transform pointPivote;

    private Animator animator;
    private bool isMoving = false;
    private int currentTarget = 0;

    private float arrivalTimer = 0f;
    public float arrivalThreshold = 0.15f; // más tolerante
    public float arrivalTimeRequired = 0.5f; // tiempo estable

    void Start()
    {
        if (model != null)
            animator = model.GetComponent<Animator>();
            
        ActualizarMensaje(0); 
    }

    public void MoveToNextMarker()
    {
        if (!isMoving && ImageTargets.Length > 0)
        {
            StartCoroutine(MoveSequence());
        }
    }

    private IEnumerator MoveSequence()
    {
        isMoving = true;

        int nextIndex = (currentTarget + 1) % ImageTargets.Length;
        ObserverBehaviour target = ImageTargets[nextIndex];

        if (target == null)
        {
            isMoving = false;
            yield break;
        }

        if (animator != null) animator.SetTrigger("startWalking");

        // LOOP DE SEGUIMIENTO CONTINUO
        while (true)
        {
            if (!IsTargetTracked(target))
            {
                if (displayTexto != null) displayTexto.text = "¡Marcador perdido!";
                yield return null;
                continue;
            }

            Vector3 targetPos = target.transform.position;

            MoverYRotarHacia(targetPos);

            float distance = Vector3.Distance(model.transform.position, targetPos);

            // Si ya llegó, pero el target se puede mover
        if (distance < arrivalThreshold)
        {
            arrivalTimer += Time.deltaTime;

            if (arrivalTimer >= arrivalTimeRequired)
            {
                //LLEGÓ DE VERDAD
                currentTarget = nextIndex;

                if (displayTexto != null)
                    displayTexto.text = "¡Llegué al marcador!";

                if (animator != null)
                    animator.SetTrigger("stopAction");

                // Ejecutar lógica por marcador
                if (currentTarget == 1)
                    yield return StartCoroutine(EquipAccessory(prefabMochila));
                else if (currentTarget == 2)
                    yield return StartCoroutine(EquipAccessory(prefabEspada));

                isMoving = false;
                yield break;
            }
        }
        else
        {
            arrivalTimer = 0f;
        }

            yield return null;
        }
    }


    private void ActualizarMensaje(int indice)
    {
        if (displayTexto != null && mensajes != null && indice < mensajes.Length)
        {
            displayTexto.text = mensajes[indice];
        }
    }

    private void MoverYRotarHacia(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - model.transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            model.transform.rotation = Quaternion.Slerp(model.transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
        }
        model.transform.position = Vector3.MoveTowards(model.transform.position, targetPos, speed * Time.deltaTime);
    }

    private IEnumerator EquipAccessory(GameObject prefab)
    {
        if (animator != null) animator.SetTrigger("startLifting");
        yield return new WaitForSeconds(1.0f); 
        if (prefab != null)
        {
            GameObject accessory = Instantiate(prefab, pointPivote.position, pointPivote.rotation);
            accessory.transform.SetParent(pointPivote);
        }
        yield return new WaitForSeconds(1.0f); 
        if (animator != null) animator.SetTrigger("stopAction");
    }

    private bool IsTargetTracked(ObserverBehaviour target)
    {
        var status = target.TargetStatus.Status;
        return status == Status.TRACKED || status == Status.EXTENDED_TRACKED;
    }
}