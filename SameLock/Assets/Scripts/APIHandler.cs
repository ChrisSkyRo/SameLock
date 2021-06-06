using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APIHandler : MonoBehaviour
{
    public io.newgrounds.core ngio_core;

    public void UnlockMedal(int medal_id)
    {
        io.newgrounds.components.Medal.unlock medal_unlock = new io.newgrounds.components.Medal.unlock();
        medal_unlock.id = medal_id;
        medal_unlock.callWith(ngio_core);
    }
}
