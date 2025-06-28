using System.Collections;
using UnityEngine;

public class OnTrack : MonoBehaviour
{
    public bool onTrack;
    public bool isPlayer = false;
    bool leftTrack = false;
    bool finished = false;
    int returnID;
    public int checks = 0;
    public int laps = 0;
    public int id;
    Collider colliderA;
    TrackRespawns trackRespawns;
    Rigidbody rb;
    ConnectionManager connectionManager;
    ItemManager itemManager;
    [SerializeField] private CarDrive carDrive;
    [SerializeField] private GameObject returnObject;
    [SerializeField] private Transform returnGrabPoint;

    void Start()
    {
        id = Random.Range(0, 100);
        onTrack = true;
        trackRespawns = GameObject.Find("TrackSegments").GetComponent<TrackRespawns>();
        connectionManager = GameObject.Find("NetworkManager").GetComponent<ConnectionManager>();
        rb = GetComponent<Rigidbody>();
        colliderA = GetComponent<Collider>();
        itemManager = GetComponent<ItemManager>();
        switch(connectionManager.ClientID)
        {
            case 1:
                gameObject.tag = "Player1";
                break;
            case 2:
                gameObject.tag = "Player2";
                break;
            case 3:
                gameObject.tag = "Player3";
                break;
            case 4:
                gameObject.tag = "Player4";
                break;
            default:
                gameObject.tag = "Player1";
                break;
        }
        print(gameObject.tag);
        print(isPlayer);
        if (!isPlayer)
            colliderA.isTrigger = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (leftTrack) { onTrack = false; }
        if (!finished && laps >= 1)
        {
            finished = true;
            carDrive.onTrack = false;
            Finish();
        }
    }

    private void LateUpdate()
    {
        if (!onTrack && leftTrack) 
        {
            carDrive.onTrack = false;
            leftTrack = false; 
            StartCoroutine(ReturnToTrack());
        }
        if(Input.GetKeyDown(KeyCode.L)) 
        {
            carDrive.onTrack = false;
            leftTrack = false; 
            StartCoroutine(ReturnToTrack()); 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Checkpoint")
        {
            int checkNum = other.gameObject.GetComponent<Checkpoint>().checkpointID;
            if (checkNum == (checks + 1))
            {
                checks = checkNum;
            }
        }
        if (other.gameObject.tag == "Fin")
        {
            if(checks == 8)
            {
                laps++;
                checks = 0;
                print("Lap: " +  laps);
            }
        }
        if(other.gameObject.tag == "Shell")
        {
            Destroy(other.gameObject);
            StartCoroutine(GotHit());
        }
        if(other.gameObject.tag == this.gameObject.tag && isPlayer)
        {
            carDrive.onTrack = false;
            
            Vector3 direction = other.transform.position - transform.position;
            direction = Vector3.Normalize(direction);
            direction = transform.InverseTransformDirection(direction);
            direction.y = 0;
            if (direction.z > 0)
            {
/*                if (other.gameObject.GetComponent<OnTrack>().isPlayer == false)
                {
                    other.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                }*/
                rb.AddRelativeForce(direction * 4000000);
            }
            StartCoroutine(HitPlayer());

        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Track" || other.gameObject.tag == "Checkpoint")
        {
            if (!onTrack) { onTrack = true; }
            if (leftTrack) { leftTrack = false; }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Track" || other.gameObject.tag == "Checkpoint")
        {
            leftTrack = true;
            returnID = other.gameObject.GetComponent<BoxID>().ID;
        }
    }

    public void Returned()
    {
        this.transform.parent = null;
        rb.useGravity = true;
        rb.isKinematic = false;
        carDrive.onTrack = true;
    }

    public void Finish()
    {
        if(isPlayer)
            GameObject.Find("RankingObj").GetComponent<Rankings>().AddPlayerToRankingRpc(connectionManager.PlayerName, gameObject.tag);
        GetComponent<PostFinishMove>().ContinueAfterFinish();
    }

    IEnumerator ReturnToTrack()
    {
        yield return new WaitForSeconds(.5f);
        rb.useGravity = false;
        rb.isKinematic = true;
        returnObject.transform.position = returnGrabPoint.position;
        returnObject.transform.rotation = returnGrabPoint.rotation;
        this.transform.parent = returnGrabPoint.transform;
        returnObject.GetComponent<ReturnToTrack>().ReturnPlayerToTrack(trackRespawns.respawns[returnID], this);
    }

    IEnumerator GotHit()
    {
        carDrive.onTrack = false;
        if (isPlayer)
        {
            itemManager.coinCount.Value -= 2;
            if (itemManager.coinCount.Value < 0)
                itemManager.coinCount.Value = 0;
        }
        yield return new WaitForSeconds(3f);
        carDrive.onTrack = true;
    }

    IEnumerator HitPlayer()
    {
        yield return new WaitForSeconds(.1f);
        carDrive.onTrack = true;
        //otherRB.isKinematic = false;
    }
}
