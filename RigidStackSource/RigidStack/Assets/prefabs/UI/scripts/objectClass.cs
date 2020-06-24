﻿using UnityEngine;
using UnityEngine.UI;

public class objectClass : MonoBehaviour {
    private short _objectCount = 0;
    [HideInInspector] public short objectCount {
        get {
            return _objectCount;
        }
        set {
            _objectCount = value;
            counterText.text = _objectCount.ToString();
            if (_objectCount == 0) {
                Debug.Log("You can not use this object.");
            }
        }
    }
    [SerializeField] private Text counterText = null;
}