using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeLight : MonoBehaviour
{
    private SpriteRenderer sr;
    public Material mat;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            mat.SetFloat("_DarknessStrength", 28);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            mat.SetFloat("_DarknessStrength", 200);
        }
    }
}
