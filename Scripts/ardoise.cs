using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;

using PDollarGestureRecognizer;
using UnityEngine.SceneManagement;

public class ardoise : MonoBehaviour
{
	AudioSource audio;
	public string folder, folderPath;
	public Transform gestureOnScreenPrefab;
	public Transform[]CraieOBject;
	private List<Gesture> trainingSet = new List<Gesture>();
	private List<Point> points = new List<Point>();
	private int strokeId = -1;
	private Vector3 virtualKeyPosition = Vector2.zero;
	private Rect drawArea;
	public GUIStyle cusmStyle;
	private RuntimePlatform platform;
	private int vertexCount = 0;
	private List<LineRenderer> gestureLinesRenderer = new List<LineRenderer>();
	private LineRenderer currentGestureLineRenderer;

	//GUI
	private string message;
	private bool recognized;
	private string newGestureName = "";
	public float devicewidth;
	public static string TheAssigment;
	public float deviceHeight, score;
	public Text infotext;
	public string  theWord;
	public string r;
	public static int ModelIndex, chalk, timeClean, niveau;

	public bool Dev;

	void Start()
	{
		

		if (folder == "")
        {
            folder = "a-d";
        }
     
        platform = Application.platform;
		folderPath = "/PDollar/Resources/GestureSet/" + folder;
		devicewidth = Screen.width;
		deviceHeight = Screen.height;
		TheAssigment = "a";
		platform = Application.platform;
		drawArea = new Rect(devicewidth / 12f, devicewidth / 12f, devicewidth / 1.2f, Screen.height / 1.5f);

		TextAsset[] gesturesXml = Resources.LoadAll<TextAsset>("GestureSet/" + folder+"/");
		//TextAsset[] gesturesXml = Resources.LoadAll<TextAsset>("GestureSet/10-stylus-MEDIUM/");
		foreach (TextAsset gestureXml in gesturesXml)
			trainingSet.Add(GestureIO.ReadGestureFromXML(gestureXml.text));

		//Load user custom gestures
		string[] filePaths = Directory.GetFiles(Application.persistentDataPath, "*.xml");
		foreach (string filePath in filePaths)
			trainingSet.Add(GestureIO.ReadGestureFromFile(filePath));
	}


	public void SetColorCraie(int i)
    {
		gestureOnScreenPrefab = CraieOBject[PaintMenu.thePaintIndex];
		chalk = i;

	}


	void Update()
	{
		
		folder = PlayerPrefs.GetString("folder");
		try {
			gestureOnScreenPrefab = CraieOBject[PaintMenu.thePaintIndex];
		} catch (Exception e)
        {

        }
		

			if (platform == RuntimePlatform.Android || platform == RuntimePlatform.IPhonePlayer)
			{
				if (Input.touchCount > 0)
				{
					virtualKeyPosition = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
				}
			}
			else
			{
				if (Input.GetMouseButton(0))
				{
					virtualKeyPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
				}
			}

			if (drawArea.Contains(virtualKeyPosition))
			{

				if (Input.GetMouseButtonDown(0))
				{

					if (recognized)
					{
						recognized = false;
						strokeId = -1;

						points.Clear();

						foreach (LineRenderer lineRenderer in gestureLinesRenderer)
						{

							lineRenderer.SetVertexCount(0);
							Destroy(lineRenderer.gameObject);
						}

						gestureLinesRenderer.Clear();
					}

					++strokeId;

					Transform tmpGesture = Instantiate(gestureOnScreenPrefab, transform.position, transform.rotation) as Transform;
					currentGestureLineRenderer = tmpGesture.GetComponent<LineRenderer>();
					gestureLinesRenderer.Add(currentGestureLineRenderer);

					vertexCount = 0;
				}

				if (Input.GetMouseButton(0))
				{
					points.Add(new Point(virtualKeyPosition.x, -virtualKeyPosition.y, strokeId));

					currentGestureLineRenderer.SetVertexCount(++vertexCount);
					currentGestureLineRenderer.SetPosition(vertexCount - 1, Camera.main.ScreenToWorldPoint(new Vector3(virtualKeyPosition.x, virtualKeyPosition.y, 10)));
				}
			}
	
	}

	public void GiveAssigment(string a)
    {
		TheAssigment = a;

	}


	public void SubmitSlate() {

		recognized = true;
		theWord = "";
		Gesture candidate = new Gesture(points.ToArray());
		Result gestureResult = PointCloudRecognizer.Classify(candidate, trainingSet.ToArray());

		message = gestureResult.GestureClass + " at " + gestureResult.Score * 100+ "%";
		infotext.text = "" + message;

		//if (gestureResult.GestureClass == TheAssigment &&  gestureResult.Score >= 0.88f)
		//{
		//	infotext.text = "" + message;

		//}
		//      else
		//      {

		//	infotext.text = "Try again";
		//}

	}

	


	public void cleanArdoise()
    {
		recognized = false;
		strokeId = -1;
		timeClean = 150;
		points.Clear();

		foreach (LineRenderer lineRenderer in gestureLinesRenderer)
		{

			lineRenderer.SetVertexCount(0);
			Destroy(lineRenderer.gameObject);
		}

		gestureLinesRenderer.Clear();
	}

	void OnGUI()
	{

        if (Dev)
        {
			GUI.Box(drawArea, "", cusmStyle);
			GUI.Label(new Rect(10, Screen.height - 40, 500, 50), message);

			if (GUI.Button(new Rect(Screen.width - 100, 10, 100, 30), "Recognize"))
			{

				recognized = true;

				Gesture candidate = new Gesture(points.ToArray());
				Result gestureResult = PointCloudRecognizer.Classify(candidate, trainingSet.ToArray());

				message = gestureResult.GestureClass + " " + gestureResult.Score;
			}

			GUI.Label(new Rect(Screen.width - 200, 150, 70, 30), "Add as: ");
			newGestureName = GUI.TextField(new Rect(Screen.width - 150, 150, 100, 30), newGestureName);

			if (GUI.Button(new Rect(Screen.width - 50, 150, 50, 30), "Add") && points.Count > 0 && newGestureName != "")
			{

				string fileName = String.Format("{0}/{1}-{2}.xml", Application.dataPath + folderPath, newGestureName, DateTime.Now.ToFileTime());

#if !UNITY_WEBPLAYER
				GestureIO.WriteGesture(points.ToArray(), newGestureName, fileName);
#endif

				trainingSet.Add(new Gesture(points.ToArray(), newGestureName));

				newGestureName = "";
			}
		}
		
	}
}
