using UnityEngine;
using System.Collections;
using OpenCvSharp;


public class test : MonoBehaviour {

	public GameObject go;

	private VideoCapture cap;
	private Texture2D tex;
	Mat frame;

	// Use this for initialization
	void Start () {
		Mat src = new Mat(Application.dataPath + "/lena.png", ImreadModes.GrayScale);
		Mat dst = new Mat();
		frame = new Mat ();
		Cv2.Canny(src, dst, 50, 200);
		tex = new Texture2D (dst.Width, dst.Height);
		tex.LoadImage (dst.ToBytes (".png", new int[]{0}));
		go.GetComponent<Renderer> ().material.mainTexture = tex;

		cap = new VideoCapture (1);
	}

	// Update is called once per frame
	void Update () {
		cap.Read (frame);
		if (!frame.Empty()){
			tex.LoadImage (frame.ToBytes (".png", new int[]{0}));
		}

	}
}