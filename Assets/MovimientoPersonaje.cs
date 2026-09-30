using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(Rigidbody2D))]
public class PersonajeMouse : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 5f;
    public float distanciaParada = 0.2f;
    public bool moverSoloConClickDerecho = false;

    [Header("Dirección")]
    public bool spriteMiraDerecha = true; // tu dibujo original mira a la derecha

    [Header("Golpe")]
    public float alcance = 1f;
    public float radioGolpe = 0.6f;
    public int danio = 1;
    public float tiempoEntreGolpes = 0.4f;
    public float duracionGolpe = 0.3f;   // cuánto dura la animación de golpe
    public bool quietoAlPegar = true;    // se frena mientras pega
    public AtaqueCuchillo cuchillo;
    Rigidbody2D rb;
    Animator anim;
    Vector2 destino;
    Vector2 direccion = Vector2.right;
    bool moverse;
    bool golpeando;
    float finGolpe;
    float proximoGolpe;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    void Update()
    {
                if (LibroControles.abierto) { moverse = false; return; }
        Vector2 mouse = PosicionMouse();
        Vector2 haciaMouse = mouse - rb.position;
        if (haciaMouse.sqrMagnitude > 0.0001f) direccion = haciaMouse.normalized;

        if (golpeando && Time.time >= finGolpe) golpeando = false;

        // Mientras pega, no se da vuelta
        if (!golpeando) Mirar(direccion);

        destino = mouse;
        moverse = !moverSoloConClickDerecho || ClickDerechoMantenido();
        if (golpeando && quietoAlPegar) moverse = false;

        if (ClickIzquierdo() && Time.time >= proximoGolpe && !LibroControles.BloqueaClick(mouse))
                {
            proximoGolpe = Time.time + tiempoEntreGolpes;
            Golpear();
        }
    }

    void FixedUpdate()
    {
        Vector2 d = destino - rb.position;
        Vector2 v = (moverse && d.magnitude > distanciaParada) ? d.normalized * velocidad : Vector2.zero;
#if UNITY_6000_0_OR_NEWER
        rb.linearVelocity = v;
#else
        rb.velocity = v;
#endif
    }

    void Golpear()
    {
        Mirar(direccion); // se da vuelta hacia donde pega
        golpeando = true;
        finGolpe = Time.time + duracionGolpe;

        if (anim != null && TieneParametro("Golpe")) anim.SetTrigger("Golpe");
                if (cuchillo != null) cuchillo.Atacar(direccion);

        Vector2 centro = rb.position + direccion * alcance;
        foreach (var h in Physics2D.OverlapCircleAll(centro, radioGolpe))
        {
            if (h.gameObject == gameObject) continue;
            var e = h.GetComponentInParent<Enemigo>();
            if (e != null) e.RecibirGolpe(danio, direccion);
        }
    }

    // Da vuelta al personaje entero (incluye objetos hijos, como un arma)
    void Mirar(Vector2 dir)
    {
        if (Mathf.Abs(dir.x) < 0.01f) return;
        bool haciaIzquierda = dir.x < 0;
        float signo = (haciaIzquierda == spriteMiraDerecha) ? -1f : 1f;
        Vector3 s = transform.localScale;
        s.x = Mathf.Abs(s.x) * signo;
        transform.localScale = s;
    }

    bool TieneParametro(string nombre)
    {
        foreach (var p in anim.parameters) if (p.name == nombre) return true;
        return false;
    }

    Vector2 PosicionMouse()
    {
#if ENABLE_INPUT_SYSTEM
        Vector2 pantalla = Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
#else
        Vector2 pantalla = Input.mousePosition;
#endif
        Camera cam = Camera.main;
        if (cam == null) return rb.position;
        return cam.ScreenToWorldPoint(new Vector3(pantalla.x, pantalla.y, -cam.transform.position.z));
    }

    bool ClickIzquierdo()
    {
#if ENABLE_INPUT_SYSTEM
        return Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
#else
        return Input.GetMouseButtonDown(0);
#endif
    }

    bool ClickDerechoMantenido()
    {
#if ENABLE_INPUT_SYSTEM
        return Mouse.current != null && Mouse.current.rightButton.isPressed;
#else
        return Input.GetMouseButton(1);
#endif
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 dir = Application.isPlaying ? (Vector3)direccion : Vector3.right;
        Gizmos.DrawWireSphere(transform.position + dir * alcance, radioGolpe);
    }
}