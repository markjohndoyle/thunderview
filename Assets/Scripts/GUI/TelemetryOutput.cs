using UnityEngine;
using System.Collections.Generic;

using org.hbird.application.thunderbirdviewer.tm;
	
public class TelemetryOutput : MonoBehaviour, TelemetryReceiverListener {
	
	private Queue<HummingbirdParameter> tmIn = new Queue<HummingbirdParameter>();
	
	private float pitch;
	private float roll;
	private float heading;

	// Use this for initialization
	void Start () {
		TelemetryProvider tmProvider = GameObject.Find("TelemetryReceiver").GetComponent<ThunderbirdTmUdpReceiver>();
		tmProvider.registerForTelemeytryUpdates(this);
		guiText.text = "Pitch = No telemetry";
	}
	
	// Update is called once per frame
	void Update () {
		if(tmIn.Count > 0) {
			HummingbirdParameter tm = tmIn.Dequeue();
			processTelemetryParameter(tm);
		}
//		Debug.Log(pitch);
		guiText.text = "Pitch = " + this.pitch;
	}
		
	public void telemetryReceived(HummingbirdParameter tm) {
		tmIn.Enqueue(tm);
	}
	
	private void processTelemetryParameter(HummingbirdParameter p) {
		switch(p.name) {
		case "IMU_Pitch":
			this.pitch = p.value;
			break;
		case "IMU_Roll":
			this.roll = p.value;
			break;
		case "IMU_Heading":
			this.heading = p.value;
			break;
		default:
			Debug.LogWarning("Unexpected telemetry parameter; ignoring");
			break;
		}
	}
}
