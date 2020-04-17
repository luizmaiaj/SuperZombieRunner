using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public RecycleGameObject prefab;

    private List<RecycleGameObject> poolInstances = new List<RecycleGameObject>();

    private RecycleGameObject CreateInstance(Vector3 pos)
    {
        var clone = GameObject.Instantiate(prefab);
        clone.transform.position = pos;
        clone.transform.parent = transform;

        poolInstances.Add(clone);

        return clone;
    }

    public RecycleGameObject NextObject(Vector3 pos)
    {
        RecycleGameObject instance = null;

        foreach(var go in poolInstances)
        {
            if(go.gameObject.activeSelf != true) // if instance is not being used
            {
                instance = go; // get unused instance
                instance.transform.position = pos; // set to position from call
            }
        }

        if(instance == null) instance = CreateInstance(pos); // in case no instance is available

        instance.Restart();

        return instance;
    }
}
