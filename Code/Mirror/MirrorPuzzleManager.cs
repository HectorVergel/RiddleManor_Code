using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MirrorPuzzleManager : MonoBehaviour
{
    MirrorObject[] mirrorObjects;

    public static MirrorPuzzleManager instance;
    public Transform planeReference;
    public UnityEvent eventOnComplete;
    public float distanceToDetect = 2f;
    private bool isCompleted;
    public Material ghostMat;
    public PusheableObject trophy1;
    public PusheableObject trophy2;
    public PusheableObject trophy3;
    public Transform trophy1Pos;
    public Transform trophy2Pos;
    public Transform trophy3Pos;
    public GameObject ghosts;
    public float timeToArrive;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    void Start()
    {
        mirrorObjects = FindObjectsOfType<MirrorObject>();
        SetDistance();
    }

    void Update()
    {
        CheckPuzzleComplete();
    }
    private void SetDistance()
    {
        foreach (MirrorObject item in mirrorObjects)
        {
            item.SetDistance(distanceToDetect);
        }
    }
    private void CheckPuzzleComplete()
    {
        if (mirrorObjects.Length != 0)
        {
            foreach (MirrorObject item in mirrorObjects)
            {
                if (!item.GetIsValid()) return;
            }
            CompletePuzzle();
        }
    }

    private void CompletePuzzle()
    {
        if (!isCompleted)
        {
            eventOnComplete?.Invoke();
            AudioManager.Play("lockedMirrorAudio").Volume(1f);
            isCompleted = true;
        }
    }
    public void HideGhosts()
    {
        StartCoroutine(SetPositions());
    }
    IEnumerator SetPositions()
    {
        PlayerController.instance.StopPushing();
        ghosts.SetActive(false);
        trophy1.canBePushed = false;
        trophy2.canBePushed = false;
        trophy3.canBePushed = false;
        MoveObject move1 = trophy1.gameObject.AddComponent<MoveObject>();
        MoveObject move2 = trophy2.gameObject.AddComponent<MoveObject>();
        MoveObject move3 = trophy3.gameObject.AddComponent<MoveObject>();
        move1.ChangeParams(trophy1Pos,timeToArrive);
        move2.ChangeParams(trophy2Pos,timeToArrive);
        move3.ChangeParams(trophy3Pos,timeToArrive);
        move1.Move();
        move2.Move();
        move3.Move();
        yield return new WaitForSeconds(timeToArrive);
        yield return new WaitForEndOfFrame();
        Destroy(move1);
        Destroy(move2);
        Destroy(move3);
    }
}
