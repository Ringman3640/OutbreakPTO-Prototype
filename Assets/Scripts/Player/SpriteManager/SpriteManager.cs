using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class SpriteManager : MonoBehaviour
{
    // Defines the bodypart to play the animation
    // "full"
    // "top"
    // "bottom"
    // "topbottom"
    public string BodyPart
    {
        get; set;
    }

    // Action name of the animation
    public string Action
    {
        get; set;
    }

    // Direction name of the animation
    public string Direction
    {
        get; set;
    }
    
    // Indicate if the sprite should be X flipped
    public bool FlipX
    {
        get; set;
    }

    // X direction range for front and back direction detection
    public float frontBackRange = 0.4f;

    // Component references
    private SubspriteManager fullSM;
    private SubspriteManager topSM;
    private SubspriteManager bottomSM;

    // Start is called before the first frame update
    void Start()
    {
        fullSM = transform.Find("Full").GetComponent<SubspriteManager>();
        Assert.IsNotNull(fullSM);

        topSM = transform.Find("Top").GetComponent<SubspriteManager>();
        Assert.IsNotNull(topSM);

        bottomSM = transform.Find("Bottom").GetComponent<SubspriteManager>();
        Assert.IsNotNull(bottomSM);

        BodyPart = "";
        Action = "";
        Direction = "";
        FlipX = false;
    }

    // Update the sprite animation given the current animation descriptors
    public void UpdateSprite()
    {
        string animationName = Action + "_" + Direction;

        switch(BodyPart)
        {
            case "full":
                fullSM.PlayAnimation("player_full_" + animationName, FlipX);
                topSM.Disable();
                bottomSM.Disable();
                return;

            case "top":
                topSM.PlayAnimation("player_top_" + animationName, FlipX);
                break;

            case "bottom":
                bottomSM.PlayAnimation("player_bottom_" + animationName, FlipX);
                break;

            case "topbottom":
                topSM.PlayAnimation("player_top_" + animationName, FlipX);
                bottomSM.PlayAnimation("player_bottom_" + animationName, FlipX);
                break;

            default:
                Debug.LogError("SpriteManager: Trying to play animation on invalid BodyPart: " + BodyPart);
                return;
        }

        fullSM.Disable();
    }

    // Play a specific animation
    public void PlayAnimation(string animationName)
    {
        if (animationName.Contains("full"))
        {
            fullSM.PlayAnimation(animationName, FlipX);
            topSM.Disable();
            bottomSM.Disable();
            return;
        }
        if (animationName.Contains("top"))
        {
            topSM.PlayAnimation(animationName, FlipX);
            fullSM.Disable();
            return;
        }
        if (animationName.Contains("bottom"))
        {
            bottomSM.PlayAnimation(animationName, FlipX);
            fullSM.Disable();
            return;
        }
    }

    // Calculate the Direction animation descriptor given a direction vector
    public void CalculateDirection(Vector3 directionVector)
    {
        if (Mathf.Abs(directionVector.x) <= frontBackRange)
        {
            if (directionVector.y > 0)
            {
                Direction = "back";
            }
            else
            {
                Direction = "front";
            }
        }
        else
        {
            if (directionVector.y > 0)
            {
                Direction = "sideback";
            }
            else
            {
                Direction = "sidefront";
            }
        }

        FlipX = (directionVector.x < -frontBackRange / 2);
    }
}
