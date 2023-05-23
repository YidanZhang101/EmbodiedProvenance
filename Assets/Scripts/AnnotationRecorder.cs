using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AnnotationRecorder : MonoBehaviour
{
    public string SpeechContent = "";
    public AudioSource AudioSource;
    private int prevContentLenth = 0;

    public GameObject annotationPrefab;
    private AnnotationManager annotationMg;

    private int annotationCounter = 0;

    public void InitAnnotation(Vector3 position)
    {
        Transform parent = Block.objectContainerInNewScene.transform;
        annotationMg = Instantiate(annotationPrefab, parent).GetComponent<AnnotationManager>();
        annotationMg.transform.name = "SpeechBubble_" + FindObjectsOfType<AnnotationManager>().Length;
        Vector3 annotationPos = position;
        annotationPos.x += 0.1f;
        annotationMg.transform.position = annotationPos;

        annotationCounter += 1;
        annotationMg.SetTitle(annotationCounter.ToString());
        annotationMg.SetContent(SpeechContent);

        AudioSource.Play();
    }
    
    private void Update()
    {
        if (annotationMg != null && SpeechContent.Length != prevContentLenth)
            annotationMg.SetContent(SpeechContent);

        prevContentLenth = SpeechContent.Length;
    }

    public void LogAnnotation()
    {
        DataLoggingManager.Instance.AnnotationList.Add(annotationMg);

        SpeechContent = "";
        prevContentLenth = 0;
    }
}
