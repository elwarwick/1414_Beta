using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(Collider2D))]
public class LibroControles : MonoBehaviour
{
    public GameObject panelControles; // la imagen de los controles (en el Canvas)
    public bool pausarJuego = true;

    public static bool abierto;
    static int frameCambio = -1;
    static LibroControles instancia;
    Collider2D col;

    void Awake()
    {
        instancia = this;
        col = GetComponent<Collider2D>();
        abierto = false;
        if (panelControles != null) panelControles.SetActive(false);
    }

    void OnDestroy()
    {
        if (instancia == this) instancia = null;
        abierto = false;
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (Time.frameCount == frameCambio) return;

        if (abierto)
        {
            if (ClickIzquierdo() || EscapeApretado()) Cerrar();
        }
        else if (ClickIzquierdo() && col.OverlapPoint(PosicionMouse()))
        {
            Abrir();
        }
    }

    void Abrir()
    {
        abierto = true;
        frameCambio = Time.frameCount;
        if (panelControles != null) panelControles.SetActive(true);
        if (pausarJuego) Time.timeScale = 0f;
    }

    void Cerrar()
    {
        abierto = false;
        frameCambio = Time.frameCount;
        if (panelControles != null) panelControles.SetActive(false);
        Time.timeScale = 1f;
    }

    // Lo usa el personaje para no pegar cuando hacés clic en el libro
    public static bool BloqueaClick(Vector2 puntoMundo)
    {
        if (abierto || Time.frameCount == frameCambio) return true;
        return instancia != null && instancia.col.OverlapPoint(puntoMundo);
    }

    Vector2 PosicionMouse()
    {
#if ENABLE_INPUT_SYSTEM
        Vector2 pantalla = Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
#else
        Vector2 pantalla = Input.mousePosition;
#endif
        Camera cam = Camera.main;
        if (cam == null) return Vector2.zero;
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

    bool EscapeApretado()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.Escape);
#endif
    }
}