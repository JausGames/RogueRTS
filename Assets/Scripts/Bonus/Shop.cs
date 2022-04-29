using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] List<Factory> factories;
    virtual public void SetUpShop()
    {
        foreach (Factory factory in factories)
        {
            factory.OnInteract(null);
        }
    }
}
