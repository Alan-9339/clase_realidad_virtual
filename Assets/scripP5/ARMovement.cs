using UnityEngine;

public class ARMovement : MonoBehaviour
{
    public float speed = 1.0f;
    public float rotationSpeed = 7.0f;

    public void MoverYRotarHacia(GameObject model, Vector3 targetPos)
    {
        Vector3 direction = (targetPos - model.transform.position).normalized;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            model.transform.rotation = Quaternion.Slerp(
                model.transform.rotation,
                targetRot,
                Time.deltaTime * rotationSpeed
            );
        }

        model.transform.position = Vector3.MoveTowards(
            model.transform.position,
            targetPos,
            speed * Time.deltaTime
        );
    }
}