using UnityEngine;
using System.IO;

public class ModelPreviewCapture : MonoBehaviour
{
    public Camera previewCamera;
    public RenderTexture previewRT;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F9))
        {
            Capture();
        }
    }

    public void Capture()
    {
        RenderTexture prev = RenderTexture.active;
        RenderTexture.active = previewRT;

        previewCamera.Render();

        Texture2D tex = new Texture2D(
            previewRT.width,
            previewRT.height,
            TextureFormat.RGBA32,
            false
        );

        tex.ReadPixels(new Rect(0, 0, previewRT.width, previewRT.height), 0, 0);
        tex.Apply();

        RenderTexture.active = prev;

        File.WriteAllBytes(
            Application.dataPath + "/PreviewImage.png",
            tex.EncodeToPNG()
        );

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif

        Debug.Log("PreviewImage.png »ý¼ºµÊ");
    }
}
