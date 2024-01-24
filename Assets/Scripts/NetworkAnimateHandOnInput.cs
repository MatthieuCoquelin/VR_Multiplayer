using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class NetworkAnimateHandOnInput : NetworkBehaviour
{
    [SerializeField] private InputActionReference m_pinchAnimationAction = null;
    [SerializeField] private InputActionReference m_gripAnimationAction = null;
    [SerializeField] private Animator m_handAnimator = null;

    // Update is called once per frame
    void Update()
    {
       if(IsOwner)
       {
            float triggerValue = m_pinchAnimationAction.action.ReadValue<float>();
            m_handAnimator.SetFloat("Trigger", triggerValue);

            float gripValue = m_gripAnimationAction.action.ReadValue<float>();
            m_handAnimator.SetFloat("Grip", gripValue);
       }
    }
}
