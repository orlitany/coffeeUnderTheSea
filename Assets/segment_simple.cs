using UnityEngine;
using System.Collections;
using OpenCvSharp;

public class segment_simple : MonoBehaviour {

	public GameObject go;
	private VideoCapture cap;
	private Texture2D tex;
	Mat frame,smoothed_img;
	Mat frame_hsv;
	Mat bkrnd_win;
	int[] bkrnd_win_size;	
	Mat dst,thresh,mask;
	MatOfPoint[] contours;


	// Use this for initialization
	private void Start () {						
		
		frame = new Mat ();
		cap = new VideoCapture (1);
		tex = new Texture2D (cap.FrameWidth, cap.FrameHeight);			 
		cap.Read (frame);

		dst = new Mat ();
		thresh = new Mat ();

		tex.LoadImage (frame.ToBytes (".png", new int[]{0}));
		go.GetComponent<Renderer> ().material.mainTexture = tex;
	}

	// Update is called once per frame
	void Update () {
		
		cap.Read (frame);


		if (!frame.Empty()){

			//assume this part of the frame contains only background
			smoothed_img = frame.Blur(new Size(5,5));

			frame_hsv = frame.CvtColor (ColorConversionCodes.BGR2HSV);
			Scalar lb = new Scalar (0, 0, 50);
			Scalar ub = new Scalar (180, 70, 180);

			Mat disc = Cv2.GetStructuringElement (MorphShapes.Ellipse, new Size (7, 7));

			Cv2.MorphologyEx (thresh, thresh, MorphTypes.Close, disc,null,3);


			contours = Cv2.FindContoursAsMat (thresh , RetrievalModes.List, ContourApproximationModes.ApproxSimple);


			mask = new Mat (thresh.Size (), thresh.Type (), Scalar.All (0));


			Cv2.Merge(new Mat[]{mask,mask,mask},mask);
			Cv2.BitwiseAnd (mask, frame, mask);

			//Cv2.Merge(new Mat[]{frame_backproj,frame_backproj,frame_backproj},frame_backproj);

			tex.LoadImage (smoothed_img.ToBytes (".png", new int[]{ 0 }));

		}

	}

	void OnDestroy(){
		
		cap.Release ();

	}
}