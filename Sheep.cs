using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static InteractableObject;

public class Sheep : MonoBehaviour
{
    /*
     * For adding an action that use hovering, Edit endHover()
     * For adding a new action add that action to SheepAction and create a method. Do stuff in Interactable Object too
     * 
     */


    public float speed;
    public Transform[] movePoints; //Stores all the vertices of movement path

    [Header("Animations")]
    public AnimationClip anim_idle;
    public AnimationClip anim_moving;
    public AnimationClip anim_eating;
    public AnimationClip anim_drinking;
    public AnimationClip anim_shaving;
    public AnimationClip anim_shavingWithBucket;

    private int _currentTargetIndex; //Currenntly moving towards
    Rigidbody2D rb;
    Animator animator;
    public enum SheepAction
    {
        Moving,
        Idle,
        Eatting,
        Drinking,
        Shaving,
        ShavingWithBucket,
        nulll
    }
    public SheepAction currentAction;
    [HideInInspector]public SheepAction targetAction;
    [HideInInspector] public InteractableObject hoveredObject;

    bool allowedToAnimate = true;

    // Start is called before the first frame update
    void Start()
    {
        if (movePoints.Length != 0) _currentTargetIndex = 0; //Assigning the initial target

        currentAction = SheepAction.Moving;
        rb = GetComponent<Rigidbody2D>();

        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveToTarget();
        ChangeState();


        if (Input.GetKeyDown(KeyCode.Space))
            targetAction = SheepAction.Moving;
    }
    //Method when State is changed to change animation accodingly
    private void ChangeState()
    {
        if(currentAction != targetAction && allowedToAnimate)
        {
            currentAction = targetAction;
            switch(currentAction)
            {
                case SheepAction.Idle:
                    StartCoroutine(delayChangeAnim(anim_idle, anim_idle, 0f, SheepAction.Moving));
                    break;
                case SheepAction.Moving:
                    StartCoroutine(delayChangeAnim(anim_moving, anim_moving, 0f));
                    break;
                case SheepAction.Eatting:
                    StartCoroutine(delayChangeAnim(anim_eating, anim_moving, 0f, SheepAction.Moving));
                    break;
                case SheepAction.Drinking:
                    StartCoroutine(delayChangeAnim(anim_drinking, anim_moving, 3f, SheepAction.Moving));
                    break;
                case SheepAction.Shaving:
                    StartCoroutine(delayChangeAnim(anim_shaving, anim_moving, 0f, SheepAction.Moving));
                    break;
                case SheepAction.ShavingWithBucket:
                    StartCoroutine(delayChangeAnim(anim_shavingWithBucket, anim_moving, 0f, SheepAction.Moving));
                    break;
                default:
                    break;
            }
        }
    }

    //Called in Fixed update, Moves to Target location and switched if already there
    private void MoveToTarget()
    {
        if (currentAction != SheepAction.Moving || movePoints.Length == 0)
            return;

        float detectRange = 0.1f;

        if(Mathf.Abs((transform.position - movePoints[_currentTargetIndex].position).magnitude) <= detectRange)
        {
            //switch target to next in line and to initial if last
            _currentTargetIndex = (_currentTargetIndex == movePoints.Length - 1) ? 0 : _currentTargetIndex + 1;

            //Debug.Log(Mathf.Sign(transform.position.x - movePoints[_currentTargetIndex].position.x));

            float rotationY = 0;
            rotationY = (Mathf.Sign(Mathf.Sign(transform.position.x - movePoints[_currentTargetIndex].position.x)) == 1) ? 0 : 180;

            transform.eulerAngles = new Vector3(transform.eulerAngles.x, rotationY, transform.eulerAngles.z);
        }

        transform.position = Vector3.MoveTowards(transform.position, movePoints[_currentTargetIndex].position, speed * Time.deltaTime);

    }

    //This function is called from the Interactable object Script
    public void InteractWithObject(InteractableObject _object)
    {
        ObjectType _objectType = _object.typeOfObject;
        hoveredObject = _object;//For keeping track of hovering

        switch (_objectType)
        {
            case InteractableObject.ObjectType.Food:
                targetAction = SheepAction.Eatting;
                break;
            case InteractableObject.ObjectType.Water:
                targetAction = SheepAction.Drinking;
                break;
            case InteractableObject.ObjectType.Razor:
                targetAction = SheepAction.Shaving;
                break;
            case InteractableObject.ObjectType.WoolBox:
                CollectWool();
                break;
            default:
                break;
        }

        foreach(IOSpawner spawners in FindObjectsOfType<IOSpawner>())
        {
            spawners.Reset();
        }

        Destroy(_object.gameObject);

        

        // Attempt to write better code

        //StartCoroutine(delayChangeAnim(animToPlay, animAfterCompletion, time, A function to be called));
    }




    //This method will be called when food is dropped near our sheep 
    public void EatFood()
    {
        

    }

    //This method will be called when water is dropped near our sheep
    public void DrinkWater()
    {
        targetAction = SheepAction.Drinking;

    }

    //This method will be called when razor is dropped near our sheep
    public void GetRazored()
    {
        targetAction = SheepAction.Shaving;
    }


    bool allowToSwitchAnim = true;
    //This method will be called when woolBox is dropped near our sheep
    public void CollectWool()
    {
        if (currentAction != SheepAction.Shaving)
            return;

        allowToSwitchAnim = false;
        Debug.Log("CollectWool");

        targetAction = SheepAction.ShavingWithBucket;

    }

    //This Method will add a delay before playing a certian animtion 
    IEnumerator delayChangeAnim(AnimationClip playClip = null, AnimationClip switchTo = null, float time = 0.1f, SheepAction changeToAction = SheepAction.nulll)
    {
        animator.Play(playClip.name);

        StopCoroutine(delayChangeAnim());

        yield return new WaitForSeconds(playClip.length + time);

        /*if(playClip.name == anim_shaving.name)
        {
            woolAmount = 0;
            switchWoolAmount(1);

        }*/

        if(allowToSwitchAnim)
        {
            animator.Play(switchTo.name);

            if (changeToAction != SheepAction.nulll)
                targetAction = changeToAction;
        }
        else
        {
            allowToSwitchAnim = true;
        }



    }
}
