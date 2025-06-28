using UnityEngine;

public class ReturnToTrack : MonoBehaviour
{
    bool up = false, returning = false, down = false;
    Vector3 nextPos;
    Quaternion nextRot;
    Vector3 currentPos;
    Transform finalTransform;
    Vector3 velocity = Vector3.zero;
    Transform holdingPoint;
    OnTrack onTrack;
    Transform cartPoint;
    Transform cart;

    private void Start()
    {
        transform.parent = null;
        cartPoint = GetComponentInChildren<Transform>();
        holdingPoint = GameObject.Find("ReturnObjectHoldingPoint").GetComponent<Transform>();
        transform.position = holdingPoint.position;
    }

    public void ReturnPlayerToTrack(Transform returnPoint, OnTrack carts)
    {
        finalTransform = returnPoint;
        onTrack = carts;
        cart = carts.gameObject.transform;
        up = true;
        nextPos = new Vector3(this.transform.position.x, this.transform.position.y + 5, this.transform.position.z);
        nextRot = cart.transform.rotation;
    }

    private void Update()
    {
        if (up)
        {
            float distance = Vector3.Distance(currentPos, nextPos);
            if (distance <= .3f)
            {
                up = false;
                returning = true;
                nextPos = new Vector3(finalTransform.position.x, finalTransform.position.y + 4, finalTransform.position.z);
                nextRot = finalTransform.rotation;
            }
            else
            {
                Move();
            }
        }
        else if (returning)
        {
            float distance = Vector3.Distance(currentPos, nextPos);
            if (distance <= .3f)
            {
                returning = false;
                down = true;
                nextPos = finalTransform.position;
                nextRot = this.transform.rotation;
            }
            else
            {
                Move();
            }
        }
        else if (down)
        {
            float distance = Vector3.Distance(currentPos, nextPos);
            if (distance <= .3f)
            {
                down = false;
                onTrack.Returned();
                transform.position = holdingPoint.position;
            }
            else
            {
                Move();
            }
        }
    }

    void Move()
    {
        transform.position = Vector3.SmoothDamp(transform.position, nextPos, ref velocity, .5f);
        transform.rotation = Quaternion.Slerp(transform.rotation, nextRot, .1f);
        currentPos = transform.position;
        cart.position = cartPoint.position;
        cart.rotation = cartPoint.rotation;
    }
}
