/**
 * Carries out transformations on the gameObject given the telemetry values.
 * 
 * @author Mark Doyle
 */

using UnityEngine;
using System.Collections.Generic;

using org.hbird.application.thunderbirdviewer.tm;

public class TelemetryTransformer : MonoBehaviour, TelemetryReceiverListener {

	private Queue<HummingbirdParameter> tmIn = new Queue<HummingbirdParameter>();
	
	private float targetPitch = 0.0f;
	private float targetRoll = 0.0f;
	private float targetHeading = 0.0f;

	// Use this for initialization
	void Start () {
		// Set target rotation angles to the position the object is created at to prevent unwanted initial rotation.
		targetRoll = transform.rotation.x;
		targetHeading = transform.rotation.y;
		targetPitch = transform.rotation.z;
		
		// Register with the TM receiver for tm updates.
		TelemetryProvider tmProvider = GameObject.Find("TelemetryReceiver").GetComponent<ThunderbirdTmUdpReceiver>();
		tmProvider.registerForTelemeytryUpdates(this);
	}
	
	/** 
	 * Unity axis mapping
	 * X = Roll
	 * Y = Yaw (Heading)
	 * Z = Pitch
	 */
	void Update () {
		if(tmIn.Count > 0) {
			HummingbirdParameter tm = tmIn.Dequeue();
			processTelemetryParameter(tm);
		}
		
		Debug.Log("Target roll = " + targetRoll);
		
		Vector3 newRot = new Vector3(targetRoll, targetHeading, targetPitch);
		Quaternion newRotQuart = Quaternion.Euler(newRot);

		transform.rotation = Quaternion.Lerp(transform.rotation, newRotQuart,  Time.deltaTime);

		Debug.Log("Target roll reset to " + targetRoll);
	}
		
	public void telemetryReceived(HummingbirdParameter tm) {
		tmIn.Enqueue(tm);
	}
	
	private void processTelemetryParameter(HummingbirdParameter p) {
		switch(p.name) {
		case "IMU_Pitch":
			this.targetPitch = p.value;
			break;
		case "IMU_Roll":
			this.targetRoll = p.value;
			break;
		case "IMU_Heading":
			this.targetHeading = p.value;
			break;
		default:
			Debug.LogWarning("Unexpected telemetry parameter; ignoring");
			break;
		}
	}
}
