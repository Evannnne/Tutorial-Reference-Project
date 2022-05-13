using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    private NavMeshAgent m_agent;
    private void Awake() => m_agent = GetComponentInChildren<NavMeshAgent>();

    // How much health the enemy has
    public float health = 100;

    // How much damage the enemy does
    public float damage = 20;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_agent.SetDestination(PlayerController.Instance.transform.position);
    }

    void Hit(float damage)
    {
        health -= damage;
        if (health <= 0) Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            collision.gameObject.BroadcastMessage("Hit", damage, SendMessageOptions.DontRequireReceiver);
            Destroy(gameObject);
        }
    }
}
