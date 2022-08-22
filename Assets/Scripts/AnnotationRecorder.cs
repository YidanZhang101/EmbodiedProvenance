using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AnnotationRecorder : MonoBehaviour
{
    public string SpeechContent = "";
    private int prevContentLenth = 0;

    public GameObject annotationPrefab;
    private AnnotationManager annotationMg;

    public string FilePath;
    private static StreamWriter annotationStreamWriter;
    private string scene = "";
    private string sceneNo = "";

    private int annotationCounter = 0;
    private List<AnnotationManager> annotationList = new List<AnnotationManager>();

    private void Start()
    {
        if (!FilePath.EndsWith("/"))
            FilePath += "/";

        scene = "06_Christmas";
        sceneNo = "1";

        string path = string.Format("{0}{1}_Scene{2}_Annotations.txt", FilePath, scene, sceneNo);
        annotationStreamWriter = new StreamWriter(path, false);
        annotationStreamWriter.WriteLine("Timestamp\tObjectName\tContent\tPosition\tRotation");
    }

    public void InitAnnotation(Vector3 position)
    {
        annotationMg = Instantiate(annotationPrefab).GetComponent<AnnotationManager>();
        Vector3 annotationPos = position;
        annotationPos.x += 0.1f;
        annotationMg.transform.position = annotationPos;

        annotationCounter += 1;
        annotationMg.SetTitle(annotationCounter.ToString());
        annotationMg.SetContent(SpeechContent);
    }

    private void Update()
    {
        if (annotationMg != null && SpeechContent.Length != prevContentLenth)
            annotationMg.SetContent(SpeechContent);

        prevContentLenth = SpeechContent.Length;

        if (annotationList.Count > 0)
        {
            foreach (AnnotationManager annotation in annotationList)
            {
                annotationStreamWriter.WriteLine(string.Join("\t",
                    DataLoggingManager.Instance.timestamp,
                    annotation.GetTitle(),
                    annotation.GetContent(),
                    annotation.transform.position,
                    annotation.transform.rotation
                ));
                annotationStreamWriter.Flush();
            }
        }
    }

    public void LogAnnotation()
    {
        annotationStreamWriter.WriteLine(string.Join("\t",
            DataLoggingManager.Instance.timestamp,
            annotationCounter,
            SpeechContent,
            annotationMg.transform.position,
            annotationMg.transform.rotation
        ));
        annotationStreamWriter.Flush();

        annotationList.Add(annotationMg);

        SpeechContent = "";
        prevContentLenth = 0;
    }

    private void OnApplicationQuit()
    {
        annotationStreamWriter.Close();
    }
}
