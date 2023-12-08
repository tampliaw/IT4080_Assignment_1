using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class PlayerUI : NetworkBehaviour
{
    public Slider healthUI;

    // public GameObject scoreCard;

    private void OnEnable()
    {
        GetComponent<Player>().playerHP.OnValueChanged += HealthChanged;


    }

    private void OnDisable()
    {
        GetComponent<Player>().playerHP.OnValueChanged -= HealthChanged;

    }

    private void HealthChanged(int previousValue, int newValue)
    {
        if (newValue / 100f > 1)
        {
            healthUI.value = 1;
}
        else
        {
            healthUI.value = newValue / 100f;
        }
    }
}
