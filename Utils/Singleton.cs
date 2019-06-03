﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance = null;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                T[] instances =
                    FindObjectsOfType<T>();

                if (instances.Length > 1)
                {
                    Debug.LogError(instances[0].transform.parent.name + "/" + instances[0].name + " There is more than one instance of " + typeof(T) + " Singleton. ");
                }
                if (instances != null && instances.Length > 0)
                {
                    _instance = instances[0];
                }
            }

            return _instance;
        }

        set
        {
            _instance = value;
        }
    }
}

// NOTE: avoid this
public static class SingletonExtension
{
    public static void ResetSingleton()
    {
        CharControllerManager.Instance = null;
        CharFeedbacks.Instance = null;
        GameManager.Instance = null;
        UIManager.Instance = null;
        DynamicsObjects.Instance = null;
    }
}
