using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class LinternaMouse : MonoBehaviour
{
    public Transform personaje;
    public Vector2 desfase = Vector2.zero;
    public float retroceso = 0.5f;     // cuánto más atrás del personaje sale la luz
    public float velocidadGiro = 15f;

    void LateUpdate()
    {
        if (personaje == null) return;

        Vector2 centro = (Vector2)personaje.position + desfase;
        Vector2 dir = PosicionMouse() - centro;
        if (dir.sqrMagnitude < 0.0001f) return;
        dir.Normalize();

        // Se coloca detrás del personaje, en la dirección contraria al mouse
        Vector2 pos = centro - dir * retroceso;
        transform.position = new Vector3(pos.x, pos.y, transform.position.z);

        float angulo = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        Quaternion objetivo = Quaternion.Euler(0, 0, angulo);
        transform.rotation = velocidadGiro <= 0
            ? objetivo
            : Quaternion.Slerp(transform.rotation, objetivo, Mathf.Clamp01(velocidadGiro * Time.deltaTime));
    }

    Vector2 PosicionMouse()
    {
#if ENABLE_INPUT_SYSTEM
        Vector2 pantalla = Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
#else
        Vector2 pantalla = Input.mousePosition;
#endif
        Camera cam = Camera.main;
        if (cam == null) return transform.position;
        return cam.ScreenToWorldPoint(new Vector3(pantalla.x, pantalla.y, -cam.transform.position.z));
    }
}