using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public sealed class PreviewCamClear : MonoBehaviour
{
    private Camera cam;
    private CommandBuffer cmd;

    void OnEnable()
    {
        cam = GetComponent<Camera>();

        cmd = new CommandBuffer();
        cmd.name = "Clear Preview RenderTexture";

        cmd.SetRenderTarget(cam.targetTexture);
        cmd.ClearRenderTarget(true, true, Color.clear);

        cam.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, cmd);
    }

    void OnDisable()
    {
        if (cam != null && cmd != null)
        {
            cam.RemoveCommandBuffer(CameraEvent.BeforeForwardOpaque, cmd);
            cmd.Release();
        }
    }
}
