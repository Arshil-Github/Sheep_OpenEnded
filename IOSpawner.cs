using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class IOSpawner : MonoBehaviour, IDragHandler
{
    public GameObject pf_Object;
    bool isObjectInstantiated = false;
    Transform instantiatedBlock;

    bool shouldSpawn = true;

    private void Update()
    {
        //When lifting the finger
        if (Input.GetMouseButtonUp(0) && !shouldSpawn)
            shouldSpawn = true;
    }
    public void InstantiateGameObject()
    {
        if (isObjectInstantiated || !shouldSpawn) return;

        isObjectInstantiated = true;
        shouldSpawn = false;

        Debug.Log(shouldSpawn);

        instantiatedBlock = Instantiate(pf_Object).transform;

        instantiatedBlock.GetComponent<InteractableObject>().DragOn(this);

        instantiatedBlock.position = GetMousePosition();

    }
    public void OnDrag(PointerEventData eventData)
    {
        InstantiateGameObject();
    }
    public void Reset()
    {
        isObjectInstantiated = false; instantiatedBlock = null;

    }
    private Vector3 GetMousePosition()
    {
        Vector3 newInput = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        newInput.z = 0;

        return newInput;
    }
}
