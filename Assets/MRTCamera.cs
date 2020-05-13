using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MRTCamera : MonoBehaviour
{

    public GameObject Template;
    public Material StandardMaterial;
    public Shader MrtShader;

    // Start is called before the first frame update
    private Camera _camera;
    private RenderTexture[] _renderTextureArray;
    private RenderBuffer[] _colorBuffers;
    private RenderTexture _depthBuffer;


    private Texture2D _texture2D;

    void Start()
    {
        _camera = GetComponent<Camera>();
        _camera.enabled = false;
        _texture2D = new Texture2D(640, 480);

        _renderTextureArray = new RenderTexture[3]
        {
            new RenderTexture(640,480, 0, RenderTextureFormat.Default),
            new RenderTexture(640,480, 0, RenderTextureFormat.Default),
            new RenderTexture(640,480, 0, RenderTextureFormat.Default),
        };

        _renderTextureArray[0].Create();
        _renderTextureArray[1].Create();
        _renderTextureArray[2].Create();

        _colorBuffers = new RenderBuffer[]{ _renderTextureArray[0].colorBuffer, _renderTextureArray[1].colorBuffer, _renderTextureArray[2].colorBuffer };

        _depthBuffer = new RenderTexture(640, 480, 24, RenderTextureFormat.Depth);
        _depthBuffer.Create();


        var o1 = Instantiate(Template, new Vector3(0, 2, 0), Quaternion.identity);
        o1.GetComponent<Renderer>().material = new Material(StandardMaterial) { mainTexture = _renderTextureArray[0] };

        var o2 = Instantiate(Template, new Vector3(0, 3, 0), Quaternion.identity);
        o2.GetComponent<Renderer>().material = new Material(StandardMaterial) { mainTexture = _renderTextureArray[1] };

        var o3 = Instantiate(Template, new Vector3(0, 4, 0), Quaternion.identity);
        o3.GetComponent<Renderer>().material = new Material(StandardMaterial) { mainTexture = _renderTextureArray[2] };
    }

    public void SaveImage(RenderTexture renderTexture, string path)
    {
        var currentActive = RenderTexture.active;
        RenderTexture.active = renderTexture;

        _texture2D.ReadPixels(new Rect(0, 0, _texture2D.width, _texture2D.height), 0, 0);
        _texture2D.Apply();

        byte[] bytes = _texture2D.EncodeToPNG();
        File.WriteAllBytes(path, bytes);

        RenderTexture.active = currentActive;
    }

    // Update is called once per frame
    void Update()
    {
        _camera.SetTargetBuffers(this._colorBuffers, this._depthBuffer.depthBuffer);
        _camera.RenderWithShader(MrtShader, "");

        //SaveImage(_renderTextureArray[0], Path.Combine(Application.streamingAssetsPath, "img0.png"));
        //SaveImage(_renderTextureArray[1], Path.Combine(Application.streamingAssetsPath, "img1.png"));
        //SaveImage(_renderTextureArray[2], Path.Combine(Application.streamingAssetsPath, "img2.png"));
    }
    void OnDestroy()
    {
        _renderTextureArray[0].Release();
        _renderTextureArray[1].Release();
        _renderTextureArray[2].Release();

        _depthBuffer.Release();
    }
}
