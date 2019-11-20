using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPatrol : MonoBehaviour
{    
    [SerializeField]
    float minX;
    [SerializeField]
    float maxX;
    [SerializeField]
    float minY;
    [SerializeField]
    float maxY;

    [SerializeField]
    float Speed;
        
    Vector2 TargetPosition;

    void Update()
    {
        if (!this.transform.root.gameObject.activeSelf)
            return;

        if((Vector2)transform.position != TargetPosition)
        {
            transform.localPosition = Vector2.MoveTowards(transform.localPosition, TargetPosition, Speed * Time.deltaTime);
        }
        else
        {
            TargetPosition = GetRandomPosition();
        }
    }

    Vector2 GetRandomPosition()
    {
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);
        return new Vector2(randomX, randomY);
    }
}
