using UnityEngine;
using System.Collections;
using OpenCvSharp;


public class segmentPerson : MonoBehaviour {

	public GameObject go;
	private VideoCapture cap;
	private Texture2D tex;
	Mat frame;
	Mat frame_hsv;
	Mat gray;
	Mat bkrnd_win;
	int[] bkrnd_win_size;
	OpenCvSharp.Rect bkrnd_rect;// = new OpenCvSharp.Rect (45, 45, 40, 40);
	Mat hist = new Mat();
	Mat frame_backproj;
	Mat dst,thresh,contours; 

	//int height;
	//OpenCvSharp.Rect[] detRect;
	//CascadeClassifier myDetector;
	bool isVid = true;

	// Use this for initialization
	private void Start () {						
		if (isVid) {
			frame = new Mat ();
			//gray = new Mat();
			cap = new VideoCapture (1);
			tex = new Texture2D (cap.FrameWidth, cap.FrameHeight);			 
			cap.Read (frame);
		} else {
			frame = new Mat(Application.dataPath + "/profile_photo.png", ImreadModes.Color);
			tex = new Texture2D (frame.Width, frame.Height);
		}
		bkrnd_win_size = new int[]{100,50};
		frame_backproj = new Mat ();
		dst = new Mat ();
		thresh = new Mat ();
		contours = new Mat ();
		tex.LoadImage (frame.ToBytes (".png", new int[]{0}));
		go.GetComponent<Renderer> ().material.mainTexture = tex;
		//myDetector = new CascadeClassifier ("C:/Users/admin/opencv/build/share/OpenCV/haarcascades/haarcascade_frontalface_default.xml");
		bkrnd_rect = new OpenCvSharp.Rect(1,1,bkrnd_win_size,bkrnd_win_size);
	}

	// Update is called once per frame
	void Update () {
		if (isVid) {
			cap.Read (frame);
		}

		if (!frame.Empty()){
			
			//assume this part of the frame contains only background
			bkrnd_win = frame.Clone(bkrnd_rect);

			bkrnd_win = bkrnd_win.CvtColor(ColorConversionCodes.BGR2HSV);
			frame_hsv = frame.CvtColor (ColorConversionCodes.BGR2HSV);

			Rangef[] ranges = { new Rangef (0, 180) }; 

			//calc the *h* (hsv) histogram of the background
			Cv2.CalcHist (new Mat[]{ bkrnd_win }, new int[]{ 0 }, null, hist, 1, new int[]{ 180 }, ranges);
			hist = hist.Normalize (0, 255, NormTypes.MinMax);

			Cv2.CalcBackProject (new Mat[]{ frame_hsv }, new int[]{ 0 }, hist, frame_backproj, ranges);

			Mat kernel = Cv2.GetStructuringElement (MorphShapes.Ellipse, new Size (7, 7));
			Cv2.Filter2D (frame_backproj, dst, dst.Type (), kernel);

			Cv2.Threshold (dst, thresh, 30.0, 255.0, 0);
			thresh = 255 - thresh;

			Cv2.MorphologyEx (tresh, tresh, MorphTypes.Open, kernel,null,3);
			Cv2.MorphologyEx (tresh, tresh, MorphTypes.ERODE, kernel,null,1);

			Cv2.FindContours (thresh, contours, null, RetrievalModes.List, ContourApproximationModes.ApproxSimple, new Point? (null));

			//contours

			//Mat mask = new Mat (thresh.Size (), thresh.Type ());


			//Cv2.Merge(new Mat[]{mask,mask,mask},mask);
			//Cv2.BitwiseAnd (mask, frame, mask);

			//Cv2.Merge(new Mat[]{frame_backproj,frame_backproj,frame_backproj},frame_backproj);

			tex.LoadImage (contours.ToBytes (".png", new int[]{ 0 }));
			
		}

	}

	void OnDestroy(){
		if (isVid) {
			cap.Release ();
		}
	}
}
