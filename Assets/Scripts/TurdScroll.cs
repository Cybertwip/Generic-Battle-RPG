using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurdScroll : MonoBehaviour
{

    Material material;
    Vector2 offset;

    public float xVelocity = 0.1f;
    public float yVelocity = -0.1f;

    private void Awake()
    {
        material = GetComponent<Renderer>().material;
    }
    // Start is called before the first frame update
    void Start()
    {
        offset = new Vector2(xVelocity, yVelocity);
    }

    // Update is called once per frame
    void Update()
    {
        material.mainTextureOffset += offset * Time.deltaTime;
    }
}
