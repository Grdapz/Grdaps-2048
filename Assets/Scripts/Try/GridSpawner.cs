using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSpawner : MonoBehaviour
{
    [SerializeField] public GameObject Grid;
    [SerializeField] public GameObject Square;
    void Awake()
    {
        for (int i = 0; i < 16; i++)
        {
            Instantiate(Square,Grid.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
