using UnityEngine;
using System.Collections;

using com.google.zxing.qrcode;

public class CameraImageAccess : MonoBehaviour, ITrackerEventHandler {
	
	private bool isFrameFormatSet;
	
	private Image cameraFeed;
	private string tempText;
	private string qrText;

	public void OnInitialized(){
	}

	void Start () {
		QCARBehaviour qcarBehaviour = GetComponent<QCARBehaviour>();
		
		if (qcarBehaviour) {
			qcarBehaviour.RegisterTrackerEventHandler(this);
		}
		
		isFrameFormatSet = CameraDevice.Instance.SetFrameFormat(Image.PIXEL_FORMAT.GRAYSCALE, true);
		
		InvokeRepeating("Autofocus", 1f, 2f);
	}
	
	void Autofocus () {
		CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_TRIGGERAUTO);
	}

	void Update () {

	}
	
	void OnGUI () {
		GUI.Box(new Rect(0, Screen.height - 25, Screen.width, 25), qrText);
	}
	
	public void OnTrackablesUpdated () {
		try {
			if(!isFrameFormatSet) {
				isFrameFormatSet = CameraDevice.Instance.SetFrameFormat(Image.PIXEL_FORMAT.GRAYSCALE, true);
			}
			
			cameraFeed = CameraDevice.Instance.GetCameraImage(Image.PIXEL_FORMAT.GRAYSCALE);
			tempText = new QRCodeReader().decode(cameraFeed.Pixels, cameraFeed.BufferWidth, cameraFeed.BufferHeight).Text;

			tryEnableGame(tempText);
		}
		catch {
			// Fail detecting QR Code!
		}
		finally {
			if(!string.IsNullOrEmpty(tempText)) {
				qrText = tempText;
			}
		}
	}

	private void tryEnableGame(string text){
		if (text != null && text.Length > 0){
			if (text.StartsWith("ArcadeCity-")){
				Debug.Log("ACCEPTED CODE: "+text);
				string[] tokens = text.Split('-');
				if (tokens.Length > 1){
					string gameName = tokens[1];

					PlayerPrefs.SetInt(gameName, 1);

					Application.LoadLevel("GamesScene");
				}
			}
		}
	}
}
