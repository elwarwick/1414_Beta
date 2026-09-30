using UnityEngine;
using UnityEngine.SceneManagement;

public class SiguienteEscena : MonoBehaviour
{
    public string Entrada;

    public void Cargar()
    {
        SceneManager.LoadScene(Entrada);
    }
}