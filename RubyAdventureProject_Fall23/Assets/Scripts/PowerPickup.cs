using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerPickup : MonoBehaviour //Hadassh R.- this is the script I added 
{
    public AudioClip collectedPowerUp; //Hadassah R. added for second sound, plays when the PowerPickup is collected.

    void OnTriggerEnter2D(Collider2D other)
    {
        RubyController controller = other.GetComponent<RubyController>();

        if (controller != null)
        {
           
                controller.ChangeSpeed(2);
                Destroy(gameObject);

                controller.PlaySound(collectedPowerUp);


        }

    }
}
