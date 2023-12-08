using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class PowerUpHealth : BasePowerUp
{
    public int healthAmount = 50;

    protected override bool ApplyToPlayer(Player thePickerUpper)
    {
        if (thePickerUpper.playerHP != null)
        {
            thePickerUpper.playerHP.Value += healthAmount;
            return true;
        }

        return false;
    }
}