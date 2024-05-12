using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Renderer))]

public class Cube : MonoBehaviour
{
    public event Action CollisionDetected;

    private Renderer _renderer;

    public bool CanRelease { get; private set; }
    public bool IsTouch { get; private set; }

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }

    public void AssignCanReleaseFalse()
    {
        CanRelease = false;
    }

    public void AssignIsTouchFalse()
    {
        IsTouch = false;
    }

    private void ChangeColor()
    {
        _renderer.material.color = Color.magenta;
    }

    private IEnumerator Countdown()
    {
        int minDelay = 2;
        int maxDelay = 5;
        int delay = UnityEngine.Random.Range(minDelay, maxDelay);
        yield return new WaitForSeconds(delay);
        CanRelease = true;
        CollisionDetected?.Invoke();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Plane>())
        {
            if (IsTouch == false)
            {
                ChangeColor();
                StartCoroutine(Countdown());
                IsTouch = true;
            }
        }
    }
}