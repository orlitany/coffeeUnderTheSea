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
	int bkrnd_win_size;
	OpenCvSharp.Rect bkrnd_rect;// = new OpenCvSharp.Rect (45, 45, 40, 40);
	Mat hist = new Mat();
	Mat frame_backproj;
	Mat mask; 

	//int height;
	//OpenCvSharp.Rect[] detRect;
	//CascadeClassifier myDetector;
	bool isVid = true;

	double thresh;

	// Use this for initialization
	private void Start () {						
		if (isVid) {
			frame = new Mat ();
			//gray = new Mat();
			cap = new VideoCapture (1);
			tex = new Texture2D (cap.FrameWidth, cap.FrameHeight);
			bkrnd_win_size = 20; //cap.FrameWidth / 5;
			cap.Read (frame);
		} else {
			frame = new Mat(Application.dataPath + "/profile_photo.png", ImreadModes.Color);
			tex = new Texture2D (frame.Width, frame.Height);
			bkrnd_win_size = 20;//frame.Width / 5;
		}
		frame_backproj = new Mat ();
		mask = new Mat ();
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
			
			bkrnd_win = frame.Clone(bkrnd_rect);

			//calc the hsv histogram inside that window
			Rangef[] ranges = { new Rangef (0, 180), new Rangef (0, 256) }; 
			bkrnd_win = bkrnd_win.CvtColor(ColorConversionCodes.BGR2HSV);


			frame_hsv = frame.CvtColor (ColorConversionCodes.BGR2HSV);

			Cv2.CalcHist (new Mat[]{ bkrnd_win }, new int[]{ 0, 1 }, null, hist, 2, new int[]{ 180, 256 }, ranges);
			hist = hist.Normalize (0, 255, NormTypes.MinMax);

			Point min_loc, max_loc;
			Cv2.MinMaxLoc (hist, out min_loc, out max_loc);
			Debug.Log (max_loc.X.ToString ());


			//double[] lowerb = {0,0,0};
			//double[] upperb = {180,255,255};

			//Mat M = new Mat(1, 3, frame_hsv.Type(), Scalar (0, 0, 0));

			Mat lowerb = new Mat (new Size (1, 3), frame_hsv.Type (), Scalar.All(100));
			Mat upperb = new Mat(new Size(1,3), frame_hsv.Type(),Scalar.All(255));

			//Debug.Log(frame_hsv.Type().ToString());
			Cv2.InRange (frame_hsv, lowerb, upperb, frame_backproj);
			//Cv2.CalcBackProject (new Mat[]{ frame_hsv }, new int[]{ 0, 1 }, hist, frame_backproj, ranges);



			Mat kernel = Cv2.GetStructuringElement (MorphShapes.Ellipse, new Size (5, 5));
			Cv2.Filter2D (frame_backproj, mask, mask.Type (), kernel);

			//thresh = Cv2.Threshold (mask, mask, 0.0, 255.0, ThresholdTypes.Otsu);

			//mask = 255 - mask;


			kernel = Cv2.GetStructuringElement (MorphShapes.Rect, new Size (3, 3));
			Cv2.MorphologyEx (mask, mask, MorphTypes.ERODE, kernel,null,2);

			kernel = Cv2.GetStructuringElement (MorphShapes.Rect, new Size (15, 15));
			Cv2.MorphologyEx (mask, mask, MorphTypes.Close, kernel,null,5);


			Cv2.Merge(new Mat[]{mask,mask,mask},mask);
			Cv2.BitwiseAnd (mask, frame, mask);

			Cv2.Merge(new Mat[]{frame_backproj,frame_backproj,frame_backproj},frame_backproj);

			tex.LoadImage (frame_backproj.ToBytes (".png", new int[]{ 0 }));
			
		}

	}

	void OnDestroy(){
		if (isVid) {
			cap.Release ();
		}
	}
}
