using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class OnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // UnityEvents to be invoked when the UI element is hovered or the hover state is exited.
    public UnityEvent onHoverEnterEvent;
    public UnityEvent onHoverExitEvent;

    // This method is called when the mouse pointer enters the UI element
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Invoke the onHoverEnterEvent when the UI element is hovered
        onHoverEnterEvent.Invoke();
    }

    // This method is called when the mouse pointer exits the UI element
    public void OnPointerExit(PointerEventData eventData)
    {
        // Invoke the onHoverExitEvent when the UI element is no longer hovered
        onHoverExitEvent.Invoke();
    }
}
