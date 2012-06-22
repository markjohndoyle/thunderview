using UnityEngine;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using LitJson;

using org.hbird.application.thunderbirdviewer;

public class ThunderbirdTmUdpReceiver : MonoBehaviour {
	public String host = "localhost";
	public int port = 15478;

	private UdpClient udpClient;
	private IPEndPoint remoteIpEndPoint;
	private Thread receiveThread;
	private bool running;
	
	private float pitch;
	private float roll;
	private float heading;

	// Use this for initialization
	void Start () {
		
		// Set target rotation angles to the position the object is created at to prevent unwanted initial rotation.
		roll = transform.position.x;
		pitch = transform.position.y;
		heading = transform.position.z;
		
    	try{
			Debug.Log("Setting up UDP");
			udpClient = new UdpClient(port);

			//IPEndPoint object will allow us to read datagrams sent from any source.
         	remoteIpEndPoint = new IPEndPoint(IPAddress.Any, port);
			
			// Create the thread object, passing in the receivedTelemetry method
      		// via a ThreadStart delegate. This does not start the thread.
      		receiveThread = new Thread(new ThreadStart(this.receiveTelemetry));
			running = true;
			receiveThread.Start();

		}  
       	catch (Exception e ) {
			Debug.LogError(e.ToString());
			udpClient.Close();
        }
	}
	
	void receiveTelemetry() {
		while(running) {
			try{
				Byte[] receiveBytes = udpClient.Receive(ref remoteIpEndPoint);
					
				if(receiveBytes.Length > 0) {
					string returnData = Encoding.ASCII.GetString(receiveBytes, 0, receiveBytes.Length);
					
					if(returnData.Length < 1) {
						Debug.LogWarning("empty string");
					}
					
					HummingbirdParameter telemetry = JsonMapper.ToObject<HummingbirdParameter>(returnData);
					
					processTelemetryParameter(telemetry);
				}
				else {
					Debug.LogWarning("Empty UDP payload");
				}
			}
			catch (Exception e ) {
				Debug.LogError(e.ToString());
        	}
		}
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
	
	// Update is called once per frame
//	void Update () {
	void FixedUpdate() {
		float tgtRoll = transform.position.x;
		float tgtPitch = transform.position.y;
		float tgtHeading = transform.position.z;
		
		tgtRoll = roll - tgtRoll;
		tgtPitch = pitch - tgtPitch;
		tgtHeading = heading - tgtHeading;
		
//		Debug.Log("(" + tgtRoll + ", " + tgtPitch + ", " + tgtHeading + ")");
		
//		Vector3 targetAtitude = new Vector3(roll, pitch, heading);
		
		roll = transform.position.x;
		pitch = transform.position.y;
		heading = transform.position.z;
		
		transform.Rotate(tgtRoll, tgtPitch, tgtHeading);
	}
	
	void OnDestroy() {
		Debug.Log("Stopping thread and closing UDP socket");
		running = false;
		udpClient.Close();
	}
}
