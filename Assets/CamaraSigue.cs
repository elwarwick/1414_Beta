using UnityEngine;

public class CamaraSigue : MonoBehaviour
{
    public Transform objetivo;          // el personaje
    public float suavidad = 0.25f;      // más alto = más lenta y suave
    public Vector2 desfase = Vector2.zero; // por si la querés un poco más arriba o al costado

    Vector3 vel;

    void LateUpdate()
    {
        if (objetivo == null) return;
        Vector3 destino = new Vector3(
            objetivo.position.x + desfase.x,
            objetivo.position.y + desfase.y,
            transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, destino, ref vel, suavidad);
    }
}