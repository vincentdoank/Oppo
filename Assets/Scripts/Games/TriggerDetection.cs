using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerDetection : MonoBehaviour
{
    public string triggerTag;
    public UnityEvent onTriggerEntered;
    public float invokeDelay = 0f;
    public UnityEvent onInvokeDelayTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == triggerTag)
        {
            Debug.LogWarning("trigger activated");
            onTriggerEntered?.Invoke();
            StartCoroutine(InvokeDelay());
        }
    }

    private IEnumerator InvokeDelay()
    {
        yield return new WaitForSeconds(invokeDelay);
        onInvokeDelayTriggered?.Invoke();
    }
}
