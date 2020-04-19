using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
    public static int numberOfPlayers;
    public static bool useDelay;

    public static void Reset()
    {
        numberOfPlayers = 2;
        useDelay = true;
    }
}
