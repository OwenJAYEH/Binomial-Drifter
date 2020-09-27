using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{

    public GameManager gmScript;

    // Start is called before the first frame update

    private void OnTriggerExit()
    {
        gmScript.CheckpointIncrease();
    }
}
