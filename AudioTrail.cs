using UnityEngine;
using System.Collections.Generic;

public class AudioTrail : MonoBehaviour
{
    public AudioSource audioSource;
    public GameObject visualBarPrefab;
    public int bandCount = 256;
    public float spacing = 0.012f;
    public float scaleMultiplier = 10f;
    public float barSpacing = 1f;
    public float maxSegmentLength = 0.25f;
    public float barYawOffset = 90f;

    public Material spectrumMaterial;
    public float maxVal = 3f;
    public float curVal = 2f;

    private float[] spectrumData;
    private List<GameObject> bars = new List<GameObject>();
    private List<Vector3> positionHistory;
    private List<Quaternion> rotationHistory;
    private Vector3 startPosition;
    private Queue<float> heightHistory;
    private Quaternion barRotOffset;

    private List<MaterialPropertyBlock> propertyBlocks;
    private int colorIndexID;
    private int maxID;

    void Start()
    {
        spectrumData = new float[bandCount];
        startPosition = transform.position;
        heightHistory = new Queue<float>();
        positionHistory = new List<Vector3>();
        rotationHistory = new List<Quaternion>();
        propertyBlocks = new List<MaterialPropertyBlock>();

        colorIndexID = Shader.PropertyToID("_ColorIndex");
        maxID = Shader.PropertyToID("_Max");

        for (int i = 0; i < bandCount; i++)
        {
            heightHistory.Enqueue(0f);
            positionHistory.Add(transform.position);
            rotationHistory.Add(transform.rotation);
        }

        for (int i = 0; i < bandCount; i++)
        {
            GameObject bar = Instantiate(visualBarPrefab, transform);
            bar.transform.localPosition = new Vector3(-i * spacing, 0, 0);
            bars.Add(bar);

            var renderer = bar.GetComponent<Renderer>();
            var block = new MaterialPropertyBlock();
            block.SetFloat(colorIndexID, curVal);
            block.SetFloat(maxID, maxVal);
            renderer.SetPropertyBlock(block);
            propertyBlocks.Add(block);
        }
        barRotOffset = Quaternion.Euler(0f, barYawOffset, 0f);
    }

    void Update()
    {
        audioSource.GetSpectrumData(spectrumData, 0, FFTWindow.BlackmanHarris);

        float currentHeight = spectrumData[0] * scaleMultiplier;
        heightHistory.Dequeue();
        heightHistory.Enqueue(currentHeight);

        Vector3 latestPos   = transform.position;
        Quaternion latestRot = transform.rotation;

        if (positionHistory.Count == 0)
        {
            positionHistory.Insert(0, latestPos);
            rotationHistory.Insert(0, latestRot);
        }
        else
        {
            Vector3 prevPos = positionHistory[0];
            Quaternion prevRot = rotationHistory[0];

            float dist = Vector3.Distance(prevPos, latestPos);

            if (dist > 0f)
            {
                int steps = Mathf.FloorToInt(dist / maxSegmentLength);
                float inv = 1f / (steps + 1);

                for (int s = 1; s <= steps; s++)
                {
                    float t = inv * s;
                    positionHistory.Insert(0, Vector3.Lerp(prevPos, latestPos, t));
                    rotationHistory.Insert(0, Quaternion.Slerp(prevRot, latestRot, t));
                }
            }

            positionHistory.Insert(0, latestPos);
            rotationHistory.Insert(0, latestRot);
        }

        if (positionHistory.Count > bandCount)
        {
            positionHistory.RemoveAt(positionHistory.Count - 1);
            rotationHistory.RemoveAt(rotationHistory.Count - 1);
        }

        float[] heightArray = heightHistory.ToArray();
        float dynamicSpacing = spacing * barSpacing;

        for (int i = 0; i < bars.Count; i++)
        {
            if (i < positionHistory.Count)
            {
                bars[i].transform.position = positionHistory[i] +
                    (bars[i].transform.right * (-i * dynamicSpacing));
                bars[i].transform.rotation = rotationHistory[i] * barRotOffset;

                float height = heightArray[bandCount - 1 - i];
                Vector3 localScale = bars[i].transform.localScale;
                localScale.y = Mathf.Lerp(localScale.y, height, Time.deltaTime * 10f);
                bars[i].transform.localScale = localScale;

                Vector3 localPos = bars[i].transform.localPosition;
                localPos.y = localScale.y / 2f;
                bars[i].transform.localPosition = localPos;
            }
        }
    }

    public void UpdateTrailPosition(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
    }

    public void SetTrailColors(int colorIndex)
    {
        curVal = (float)colorIndex;

        for (int i = 0; i < bars.Count; i++)
        {
            var renderer = bars[i].GetComponent<Renderer>();
            var block = propertyBlocks[i];

            block.SetFloat(colorIndexID, curVal);
            block.SetFloat(maxID, maxVal);
            renderer.SetPropertyBlock(block);
        }
    }
}
