using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public enum ObjectType
    { 
        Food,
        Water,
        Razor,
        WoolBox
    }
    public ObjectType typeOfObject;


    private bool isDragged = false;
    [HideInInspector]public IOSpawner mySpawner; //Spawner which spawned this 


    private Sheep interactingSheep;

    private void Update()
    {
        if (isDragged)
            transform.position = GetMousePosition();

        if (isDragged && Input.GetMouseButtonUp(0)) //Dropped Condition
            Drop();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Dragging the object to a sheep
        if (collision.CompareTag("Sheep"))
        {
            //Start Hover here
            interactingSheep = collision.GetComponent<Sheep>();
        }
    }
    //Called when instantiated
    public void DragOn(IOSpawner spawnedFrom)
    {
        mySpawner = spawnedFrom;
        isDragged = true;
    }

    //Called when the item is dropped
    public void Drop()
    {
        interactingSheep.InteractWithObject(this); //Interact with the sheep

        mySpawner.Reset();


        Destroy(gameObject); //Destroy 
    }
    
    private Vector3 GetMousePosition()
    {
        Vector3 newInput = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        newInput.z = 0;

        return newInput;
    }
}
