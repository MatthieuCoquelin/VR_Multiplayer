using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkPlayer : NetworkBehaviour
{
    [SerializeField] private Transform m_root = null;
    [SerializeField] private Transform m_head = null;
    [SerializeField] private Transform m_leftHand = null;
    [SerializeField] private Transform m_rightHand = null;

    [SerializeField] private List<Renderer> m_renderers = null;


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if(IsOwner)
        {
            for(int i = 0; i < m_renderers.Count; i++)
            {
                m_renderers[i].enabled = false;
            }
            
        }
    }



    // Update is called once per frame
    void Update()
    {
        if(IsOwner)
        {
            m_root.position = VRRigReference.Singleton.root.position;
            m_root.rotation = VRRigReference.Singleton.root.rotation;

            m_head.position = VRRigReference.Singleton.head.position;
            m_head.rotation = VRRigReference.Singleton.head.rotation;

            m_leftHand.position = VRRigReference.Singleton.leftHand.position;
            m_leftHand.rotation = VRRigReference.Singleton.leftHand.rotation;

            m_rightHand.position = VRRigReference.Singleton.rightHand.position;
            m_rightHand.rotation = VRRigReference.Singleton.rightHand.rotation;
        }
    }
}
