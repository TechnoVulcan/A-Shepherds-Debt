using UnityEngine;

public class StatueBarrierController : MonoBehaviour
{
    [SerializeField] private GameObject leftBarrier;
    [SerializeField] private GameObject rightBarrier;

    private bool barriersRemoved = false;

    private void Update()
    {
        if (GameProgress.Instance.talkedToStatue && !barriersRemoved)
        {
            RemoveBarriers();
        }
    }

    public void RemoveBarriers()
    {
        leftBarrier.SetActive(false);
        rightBarrier.SetActive(false);

        barriersRemoved = true;
    }
}