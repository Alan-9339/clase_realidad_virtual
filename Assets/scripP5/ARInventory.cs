using UnityEngine;

public class ARInventory : MonoBehaviour
{
    public enum AccessoryType
    {
        Back,
        Hand
    }

    [Header("Referencias a Objetos en Escena")]
    public GameObject mochilaEnHueso;
    public GameObject espadaEnHueso;

    [Header("Espadas")]
    public GameObject[] espadasDisponibles;

    void Awake()
    {
        ClearAccessories();
    }


    public System.Collections.IEnumerator EquipAccessory(AccessoryType type, Animator animator = null)
     {
        if (animator != null) animator.SetTrigger("startLifting");
    
        //  Espera a que el Animator procese el cambio
        yield return null;
        yield return null;

        if (type == AccessoryType.Back && mochilaEnHueso != null)
        {
            mochilaEnHueso.SetActive(true);
        }
        else if (type == AccessoryType.Hand && espadaEnHueso != null)
        {
            espadaEnHueso.SetActive(true);

            if (espadasDisponibles.Length > 0)
                espadasDisponibles[0].SetActive(true);
        }

        yield return new WaitForSeconds(1.0f);

        if (animator != null) animator.SetTrigger("stopAction");
    }

    public void ChangeSwordByIndex(int index)
    {
        foreach (GameObject sword in espadasDisponibles)
        {
            if(sword != null) sword.SetActive(false);
        }

        if (index >= 0 && index < espadasDisponibles.Length)
        {
            espadasDisponibles[index].SetActive(true);
        }
    }

    public void ClearAccessories()
    {
        if (mochilaEnHueso != null) mochilaEnHueso.SetActive(false);
        if (espadaEnHueso != null) espadaEnHueso.SetActive(false);
        
        foreach (GameObject sword in espadasDisponibles)
        {
            if(sword != null) sword.SetActive(false);
        }
    }
}