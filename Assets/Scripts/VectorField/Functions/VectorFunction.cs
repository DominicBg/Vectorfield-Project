using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class VectorFunction : MonoBehaviour
{
    public UnityEvent OnValueChanged = new UnityEvent();
    void OnValidate()
    {
        OnValueChanged.Invoke();
    }

    public virtual void Initialize() { }
    public abstract Vector3 GetVector(Vector3 point);
}

