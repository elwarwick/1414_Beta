using UnityEngine;

public class AtaqueCuchillo : MonoBehaviour
{
    public Transform personaje;
    public SpriteRenderer spriteCuchillo;
    public TrailRenderer estela;      // opcional: la "raya" que deja el cuchillo
    public float arco = 120f;         // cuántos grados recorre el barrido
    public float duracion = 0.2f;     // cuánto tarda el barrido

    float inicio;
    bool atacando;
    float anguloBase;
    float signo = 1f;

    void Awake()
    {
        if (spriteCuchillo != null) spriteCuchillo.enabled = false;
        if (estela != null) estela.emitting = false;
    }

    public void Atacar(Vector2 dir)
    {
        anguloBase = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        signo = dir.x < 0 ? -1f : 1f; // así siempre corta de arriba hacia abajo

        if (spriteCuchillo != null)
        {
            spriteCuchillo.enabled = true;
            spriteCuchillo.flipY = dir.x < 0; // que no quede dado vuelta a la izquierda
        }

        inicio = Time.time;
        atacando = true;
        Girar(0f);

        if (estela != null) { estela.Clear(); estela.emitting = true; }
    }

    void LateUpdate()
    {
        if (personaje != null)
            transform.position = new Vector3(personaje.position.x, personaje.position.y, transform.position.z);

        if (!atacando) return;

        float p = Mathf.Clamp01((Time.time - inicio) / duracion);
        Girar(p);

        if (p >= 1f)
        {
            atacando = false;
            if (spriteCuchillo != null) spriteCuchillo.enabled = false;
            if (estela != null) estela.emitting = false;
        }
    }

    void Girar(float p)
    {
        float suave = p * p * (3f - 2f * p); // arranca y termina suave
        float angulo = anguloBase + signo * Mathf.Lerp(arco / 2f, -arco / 2f, suave);
        transform.rotation = Quaternion.Euler(0, 0, angulo);
    }
}