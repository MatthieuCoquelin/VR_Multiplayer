using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRRigReference : MonoBehaviour
{
    public static VRRigReference Singleton;

    public Transform root = null;
    public Transform head = null;
    public Transform leftHand = null;
    public Transform rightHand = null;

    private void Awake()
    {
        Singleton = this;
    }
}
