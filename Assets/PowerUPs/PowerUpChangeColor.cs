using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class PowerUpChangeColor : BasePowerUp
{
    protected override bool ApplyToPlayer(Player thePickerUpper)
    {
        if (thePickerUpper.playerColor.Value == Color.white)
        {
            return false;
        }
        else
        {
            thePickerUpper.playerColor.Value = Color.white;
            return true;
        }
    }
}
