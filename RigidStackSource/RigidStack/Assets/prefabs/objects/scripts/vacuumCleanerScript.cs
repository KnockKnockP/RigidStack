using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vacuumCleanerScript : objectInformation {
    private bool _isServer, isFindingCoroutineActive, isVacuumActive = true;
    [Header("Vacuum cleaner script."), SerializeField] private Color originalContainerColor = Color.white;
    private Vector3 endPointPosition, newEndPointPosition;
    [SerializeField] private Vector2 size = Vector2.zero;
    [SerializeField] private Transform _transform = null, endPointTransform = null;
    [SerializeField] private LayerMask objectLayerMask = default;
    [SerializeField] private Sprite disabledSprite = null;
    [SerializeField] private SpriteRenderer containerSpriteRenderer = null;
    private Collider2D[] objectsToDrag = null;
    private readonly List<Rigidbody2D> objectsToDragRigidBody2D = new List<Rigidbody2D>();
    private IEnumerator coroutine;

    private void Start() {
        _isServer = isServer;
        if (_isServer == true) {
            endPointPosition = endPointTransform.localPosition;
        }
        dimDelegate = Dim;
        unDimDelegate = UnDim;
        return;
    }

    private void Update() {
        if (Time.timeScale == 0f) {
            spriteRenderer.sprite = disabledSprite;
        }
        if (_isServer == true) {
            newEndPointPosition = (_transform.position + endPointPosition);
            if ((isFindingCoroutineActive == false) && (_collider2D.isTrigger == false)) {
                isFindingCoroutineActive = true;
                coroutine = FindObjectsToVacuum();
                StartCoroutine(coroutine);
            }
            if (isVacuumActive == true) {
                Vacuum();
            }
        }
        return;
    }

    private IEnumerator FindObjectsToVacuum() {
        while (true) {
            objectsToDrag = Physics2D.OverlapBoxAll(newEndPointPosition, size, _transform.rotation.eulerAngles.z, objectLayerMask);
            objectsToDragRigidBody2D.Clear();
            foreach (Collider2D collider2D in objectsToDrag) {
                objectsToDragRigidBody2D.Add(collider2D.attachedRigidbody);
            }
            yield return new WaitForSeconds(1f);
        }
    }

    //http://answers.unity.com/answers/462601/view.html
    private void Vacuum() {
        for (int i = 0; i < objectsToDrag?.Length; i++) {
            try {
                if (objectsToDrag[i] != null) {
                    if (objectsToDrag[i].transform != _transform) {
                        Vector2 objectPosition = objectsToDrag[i].transform.position, difference = (newEndPointPosition - (Vector3)(objectPosition));
                        Vector2 pullDirection = difference.normalized;
                        const float pullForce = 10f;
                        if (objectsToDragRigidBody2D[i] != null) {
                            objectsToDragRigidBody2D[i].velocity += (pullDirection * (pullForce * Time.deltaTime));
                        }
                    } else {
                        objectsToDrag[i] = null;
                    }
                }
            } catch (IndexOutOfRangeException indexOutOfRangeException) {
                Debug.LogWarning($"{indexOutOfRangeException} on vacuum cleaner script.");
                break;
            }
        }
        return;
    }

    public override void Dim() {
        base.Dim();
        containerSpriteRenderer.color = dimmedColor;
        StopCoroutine(coroutine);
        isVacuumActive = false;
        return;
    }

    public override void UnDim() {
        base.UnDim();
        containerSpriteRenderer.color = originalContainerColor;
        return;
    }
}