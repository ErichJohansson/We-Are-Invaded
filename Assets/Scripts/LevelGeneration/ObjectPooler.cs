using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public List<ObjectToPool> objectsToPool;
    private List<GameObject> pooledObjects;

    private void Start()
    {
        pooledObjects = new List<GameObject>();
        for (int i = 0; i < objectsToPool.Count; i++)
        {
            for (int j = 0; j < objectsToPool[i].amountToPool; j++)
            {
                GameObject obj = Instantiate(objectsToPool[i].objectToPool);
                obj.SetActive(false);
                pooledObjects.Add(obj);
            }
        }
    }

    public GameObject GetPooledObject(string tag)
    {
        foreach(GameObject obj in pooledObjects)
        {
            if (obj == null)
                continue;
            if (!obj.activeInHierarchy && obj.tag == tag)
                return obj;
        }

        for (int i = 0; i < objectsToPool.Count; i++)
        {
            if(objectsToPool[i].objectToPool != null && objectsToPool[i].objectToPool.tag == tag && objectsToPool[i].shouldGrow)
            {
                GameObject obj = Instantiate(objectsToPool[i].objectToPool);
                obj.SetActive(false);
                pooledObjects.Add(obj);
                return obj;
            }
        }

        return null;
    }

    public void Restart()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (pooledObjects[i] != null)
                pooledObjects[i].SetActive(false);
        }
    }
}
