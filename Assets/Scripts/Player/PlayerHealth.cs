using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    public customFloatEvent updateUIHealth;
    public float playerHealth = 100f;

    void Update() 
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        updateHealth(-25f);
        Debug.Log("hit");
        Debug.Log(playerHealth);

    }
    public void updateHealth(float damage)
    {

       playerHealth += damage;

       updateUIHealth.Invoke(playerHealth);


        if (playerHealth <= 0)
        {
            FindObjectOfType<GameManager>().EndGame();
        }
    }

}
