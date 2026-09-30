using UnityEngine;

public class Enemigo : MonoBehaviour
{
    public int vida = 3;
    public float empuje = 3f; // qué tanto lo tira para atrás el golpe

    SpriteRenderer sr;
    Rigidbody2D rb;
    Color colorOriginal;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        if (sr != null) colorOriginal = sr.color;
    }

    public void RecibirGolpe(int danio, Vector2 direccion)
    {
        vida -= danio;

        if (rb != null) rb.AddForce(direccion * empuje, ForceMode2D.Impulse);

        if (sr != null)
        {
            sr.color = Color.red; // parpadea en rojo
            CancelInvoke(nameof(VolverColor));
            Invoke(nameof(VolverColor), 0.1f);
        }

        if (vida <= 0) Destroy(gameObject);
    }

    void VolverColor()
    {
        if (sr != null) sr.color = colorOriginal;
    }
}