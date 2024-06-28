using UnityEngine;
using UnityEngine.Events;

public class BrainPartTouchHandler : MonoBehaviour
{
    public AnatomyName anatomyName;
    public UnityEvent OnTouched;

    void OnMouseDown()
    {
        OnTouched.Invoke();
    }
}
