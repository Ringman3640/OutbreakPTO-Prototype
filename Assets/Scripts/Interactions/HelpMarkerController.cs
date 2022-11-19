using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Assertions;

public class HelpMarkerController : Interactable
{
    [SerializeField]
    private SpriteRenderer signSprite;

    [SerializeField]
    private SpriteRenderer spriteLight;

    [SerializeField]
    private Light2D pointLight;

    [SerializeField]
    private Vector2 signDimension = Vector2.zero;

    [SerializeField]
    private float signSpeed = 0.1f;

    [SerializeField]
    private float colorTimeScale = 0.25f;

    private bool showCoroutineStarted;
    private bool hideCoroutineStarted;

    private Color currColor;
    private float currHue;

    // Start is called before the first frame update
    void Start()
    {
        Assert.IsNotNull(signSprite);
        Assert.IsNotNull(spriteLight);
        Assert.IsNotNull(pointLight);
        Assert.IsTrue(signDimension != Vector2.zero);

        showCoroutineStarted = false;
        hideCoroutineStarted = false;

        currColor = Color.red;
        currHue = 0f;

        signSprite.size = new(signDimension.x, 0f);
    }

    public override void Interact()
    {
        // stub
        Debug.Log("AMONGUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS SUS");
    }

    // Update is called once per frame
    void Update()
    {
        currHue = (currHue + Time.deltaTime * colorTimeScale) % 1f;
        currColor = Color.HSVToRGB(currHue, 0.25f, 1f);
        spriteLight.color = currColor;
        pointLight.color = currColor;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.HasTag("Player") && !showCoroutineStarted)
        {
            StartCoroutine(SignShowCoroutine());
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.HasTag("Player") && !hideCoroutineStarted)
        {
            StartCoroutine("SignHideCoroutine");
        }
    }

    private IEnumerator SignShowCoroutine()
    {
        while (hideCoroutineStarted)
        {
            yield return null;
        }
        showCoroutineStarted = true;

        float startTime = Time.time;
        Vector2 currSize = signSprite.size;
        while (Time.time - startTime < signSpeed)
        {
            currSize.y = Mathf.Lerp(0, signDimension.y, (Time.time - startTime) / signSpeed);
            signSprite.size = currSize;
            yield return null;
        }

        signSprite.size = signDimension;

        showCoroutineStarted = false;
        yield break;
    }

    private IEnumerator SignHideCoroutine()
    {
        while (showCoroutineStarted)
        {
            yield return null;
        }
        hideCoroutineStarted = true;

        float startTime = Time.time;
        Vector2 currSize = signSprite.size;
        while (Time.time - startTime < signSpeed)
        {
            currSize.y = Mathf.Lerp(signDimension.y, 0, (Time.time - startTime) / signSpeed);
            signSprite.size = currSize;
            yield return null;
        }

        currSize.y = 0f;
        signSprite.size = currSize;

        hideCoroutineStarted = false;
        yield break;
    }
}
