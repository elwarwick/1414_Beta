using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class BotonEscena : MonoBehaviour, IPointerClickHandler
{
    public string nombreEscena = "Cinematica_1414";

    public void OnPointerClick(PointerEventData eventData)
    {
        SceneManager.LoadScene(nombreEscena);
    }
}