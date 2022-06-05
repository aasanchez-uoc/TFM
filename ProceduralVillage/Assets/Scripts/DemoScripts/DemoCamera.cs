using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class DemoCamera : MonoBehaviour
{
    public Vector3 target;
    public float RotateSpeed = 10.0f;

    PostProcessVolume m_Volume;
    DepthOfField m_DepthOfField;

    void Start()
    {
        transform.LookAt(target);
    }

    void Update()
    {
        transform.LookAt(target);
        transform.RotateAround(target, new Vector3(0.0f, 1.0f, 0.0f), Time.deltaTime * RotateSpeed);

    }

    public void SetTarget(float x, float y)
    {
        target = new Vector3(x, 0, y);
        float d = Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2));
        UpdateEffects(d);
        transform.position = new Vector3(x- d * 0.7f, d / 4, y - d * 0.75f);
    }

    public void UpdateEffects(float distance)
    {
        m_Volume = GetComponent<PostProcessVolume>();
        m_DepthOfField = m_Volume.profile.GetSetting<DepthOfField>();

        m_DepthOfField.focusDistance.value = distance * 0.4f;
        m_DepthOfField.focalLength.value = distance * 0.33f + 20.5f;

    }

}
