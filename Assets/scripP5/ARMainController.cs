using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class ARMainController : MonoBehaviour
{
    [Header("Referencias de Módulos")]
    public ARUIManager uiManager;
    public ARMovement movement;
    public ARInventory inventory;
    public ARVisualManager visualManager;
    public ARNarrativeManager narrativeManager;
    public ARSecondaryCharacter secondaryCharacter;

    [Header("Configuración del Modelo")]
    public GameObject model;
    public ObserverBehaviour[] ImageTargets;

    private Animator animator;
    private bool isMoving = false;
    private int currentTarget = 0;

    private int indiceMochila;
    private int indiceEspada;

    private float arrivalTimer = 0f;
    public float arrivalThreshold = 0.15f;
    public float arrivalTimeRequired = 0.5f;

    private int currentSwordIndex = 0;

    void Start()
    {
        if (model != null)
            animator = model.GetComponent<Animator>();

        // Posicionar en primer marker
        if (ImageTargets.Length > 0 && ImageTargets[0] != null)
        {
            model.transform.SetParent(ImageTargets[0].transform, true);
        }

        // Generar posiciones aleatorias
        GenerarIndicesAleatorios();

        // Pasar datos a narrativa
        narrativeManager.indiceMochila = indiceMochila;
        narrativeManager.indiceEspada = indiceEspada;

        // Inicializar NPC secundario
        secondaryCharacter.imageTargets = System.Array.ConvertAll(ImageTargets, x => x.transform);
        secondaryCharacter.Inicializar(indiceMochila, indiceEspada);

        narrativeManager.IniciarNarrativa();
    }

    void GenerarIndicesAleatorios()
    {
        List<int> indices = new List<int>();

        for (int i = 0; i < ImageTargets.Length; i++)
            indices.Add(i);

        for (int i = 0; i < indices.Count; i++)
        {
            int rnd = Random.Range(0, indices.Count);
            int temp = indices[i];
            indices[i] = indices[rnd];
            indices[rnd] = temp;
        }

        indiceMochila = indices[1];
        indiceEspada = indices[2];
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

        model.transform.SetParent(null);

        if (animator != null)
            animator.SetTrigger("startWalking");

        arrivalTimer = 0f;

        float lostTimer = 0f;
        float maxLostTime = 2f;

        while (true)
        {
            // Si se pierde tracking
            if (!IsTargetTracked(target))
            {
                uiManager.MostrarTextoDirecto("¡Marcador perdido!");
                lostTimer += Time.deltaTime;

                if (lostTimer >= maxLostTime)
                {
                    Debug.Log("Abortando movimiento por pérdida de tracking");
                    isMoving = false;
                    yield break;
                }

                yield return null;
                continue;
            }

            lostTimer = 0f;

            Vector3 targetPos = target.transform.position;
            movement.MoverYRotarHacia(model, targetPos);

            float distance = Vector3.Distance(model.transform.position, targetPos);

            if (distance < arrivalThreshold)
            {
                arrivalTimer += Time.deltaTime;

                if (arrivalTimer >= arrivalTimeRequired)
                {
                    yield return StartCoroutine(LlegadaAlObjetivo(nextIndex));
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

    private IEnumerator LlegadaAlObjetivo(int index)
    {
        currentTarget = index;
        ObserverBehaviour target = ImageTargets[index];

        model.transform.position = target.transform.position;
        Quaternion finalRotation = model.transform.rotation;
        model.transform.SetParent(target.transform, true);
        model.transform.rotation = finalRotation;

        uiManager.MostrarTextoDirecto("¡Llegué al marcador!");

        if (animator != null)
            animator.SetTrigger("stopAction");

        if (visualManager != null)
            visualManager.CambiarColorPorIndice(currentTarget);

        // Narrativa
        narrativeManager.EvaluarTarget(currentTarget);

        // Mochila
        if (narrativeManager.estadoActual == ARNarrativeManager.EstadoNarrativa.BuscandoEspada 
            && !inventory.mochilaEnHueso.activeSelf)
        {
            yield return StartCoroutine(
                inventory.EquipAccessory(ARInventory.AccessoryType.Back, animator)
            );
        }

        // Espada
        if (narrativeManager.estadoActual == ARNarrativeManager.EstadoNarrativa.BuscandoNPC 
            && !inventory.espadaEnHueso.activeSelf)
        {
            yield return StartCoroutine(
                inventory.EquipAccessory(ARInventory.AccessoryType.Hand, animator)
            );
        }

        secondaryCharacter.EvaluarEstado();

        if (narrativeManager.PuedeContinuar())
        {
            yield return new WaitForSeconds(0.5f);
            MoveToNextMarker();
        }
    }

    private bool IsTargetTracked(ObserverBehaviour target)
    {
        var status = target.TargetStatus.Status;
        return status == Status.TRACKED || status == Status.EXTENDED_TRACKED;
    }

    public void ResetAR()
    {
        StopAllCoroutines();
        isMoving = false;

        currentTarget = 0;
        arrivalTimer = 0f;

        if (animator != null)
        {
            animator.Rebind();
            animator.Update(0f);
        }

        if (ImageTargets.Length > 0 && ImageTargets[0] != null)
        {
            model.transform.SetParent(ImageTargets[0].transform, true);
            model.transform.position = ImageTargets[0].transform.position;
        }

        model.transform.rotation = Quaternion.identity;

        if (inventory != null)
        {
            inventory.ClearAccessories();
        }

        if (visualManager != null)
        {
            visualManager.CambiarColorPorIndice(0);
        }

        narrativeManager.ResetNarrativa();
    }

    public void CambiarEspada()
    {
        if (inventory == null || inventory.espadasDisponibles.Length == 0)
            return;

        currentSwordIndex++;

        if (currentSwordIndex >= inventory.espadasDisponibles.Length)
            currentSwordIndex = 0;

        inventory.ChangeSwordByIndex(currentSwordIndex);
    }
}