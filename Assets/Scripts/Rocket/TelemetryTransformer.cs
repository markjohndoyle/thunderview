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
	
	private float targetPitch = 0F;
	private float targetRoll = 0F;
	private float targetHeading = 0F;
	
	private bool pitchChanged = false;
	private bool rollChanged = false;
	private bool headingChanged = false;
	
	Vector3 targetVector = new Vector3();
	
	// Use this for initialization
	void Start () {
		// Set target rotation angles to the position the object is created at to prevent unwanted initial rotation.
		targetRoll = transform.localRotation.eulerAngles.x;
		targetHeading = transform.localRotation.eulerAngles.y;
		targetPitch = transform.localRotation.eulerAngles.z;
		
		Debug.Log("Initialised target rotations (r,h,p) = " + targetRoll + ", " + targetHeading + ", " + targetPitch);
		
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
			
			Quaternion targetQuaternion = new Quaternion();
			
			
			float currentRoll =  transform.localRotation.eulerAngles.x;
			float currentHeading = transform.localRotation.eulerAngles.y;
			float currentPitch = transform.localRotation.eulerAngles.z;
			
			Debug.Log("------------------------------------");
			Debug.Log("Currents (R,H,P)  = " + currentRoll + ", " + currentHeading + ", " + currentPitch);
			Debug.Log("Targets (R,H,P)  = " + targetRoll + ", " + targetHeading + ", " + targetPitch);
			
			if(rollChanged) {
				Debug.Log("--------- Roll updated ---------");
				targetQuaternion = Quaternion.Euler(targetRoll - currentRoll, 0F, 0F);
				targetVector.Set(targetRoll - currentRoll, 0F, 0F);
//				targetQuaternion = Quaternion.Euler(targetRoll, currentHeading, currentPitch);
			}
			else if(headingChanged) {
				Debug.Log("--------- Heading updated ---------");
				targetQuaternion = Quaternion.Euler(0F, targetHeading - currentHeading, 0F);
				targetVector.Set(0F, targetHeading - currentHeading, 0F);
//				targetQuaternion = Quaternion.Euler(currentRoll, targetHeading, currentPitch);
			}
			else if(pitchChanged) {
				Debug.Log("--------- Pitch updated ---------");
				targetQuaternion = Quaternion.Euler(0F, 0F, targetPitch - currentPitch);
				targetVector.Set(0F, 0F, targetPitch - currentPitch);
//				targetQuaternion = Quaternion.Euler(currentRoll, currentHeading, targetPitch);
			}
			
			Debug.Log("Target Vector = " + targetVector);
			
			// Carry out rotation
//			transform.Rotate(targetQuaternion.eulerAngles);
			Debug.Log("Setting Rotation.");
			transform.localRotation = Quaternion.Euler(targetVector);
//			transform.Rotate(targetVector);
//			transform.localRotation = Quaternion.Slerp(transform.localRotation, targetQuaternion, Time.time);
			
			Debug.Log("------------------------------------");

		}
		
		resetChangedFlags();
	}
		
	public void telemetryReceived(HummingbirdParameter tm) {
		tmIn.Enqueue(tm);
	}
	
	private void processTelemetryParameter(HummingbirdParameter p) {
		switch(p.name) {
		case "IMU_Pitch":
			this.targetPitch = p.value;
			pitchChanged = true;
			break;
		case "IMU_Roll":
			this.targetRoll = p.value;
			rollChanged = true;
			break;
		case "IMU_Heading":
			this.targetHeading = p.value;
			headingChanged = true;
			break;
		default:
			Debug.LogWarning("Unexpected telemetry parameter; ignoring");
			break;
		}
	}
	
	private void resetChangedFlags() {
		pitchChanged = false;
		rollChanged = false;
		headingChanged = false;
	}
}
