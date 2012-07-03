using UnityEngine;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;

using LitJson;

using org.hbird.application.thunderbirdviewer.tm;

public class ThunderbirdTmUdpReceiver : MonoBehaviour, TelemetryProvider {
	public String host = "localhost";
	public int port = 15478;

	private UdpClient udpClient;
	private IPEndPoint remoteIpEndPoint;
	private Thread receiveThread;
	private bool running;
	
	private IList<TelemetryReceiverListener> listeners = new List<TelemetryReceiverListener>();

	// Use this for initialization
	void Start () {
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
					updateListeners(telemetry);
				}
				else {
					Debug.LogWarning("Empty UDP payload");
				}
			}
			catch(SocketException sockE) {
				if(running) {
					Debug.LogError(sockE.ToString());
				}
				else {
					Debug.Log("Socket closed");
				}
			}
			catch (Exception e ) {
				Debug.LogError(e.ToString());
        	}
		}
	}
	
	private void updateListeners(HummingbirdParameter telemetry) {
		if(listeners.Count > 0) {
			foreach(TelemetryReceiverListener l in listeners) {
				l.telemetryReceived(telemetry);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	void OnDestroy() {
		Debug.Log("Stopping thread and closing UDP socket");
		running = false;
		udpClient.Close();
	}
		
	public void registerForTelemeytryUpdates(TelemetryReceiverListener l) {
		Debug.Log("Adding telemetry listener");
		this.listeners.Add(l);
	}
		
	public void unregisterForTelemeytryUpdates(TelemetryReceiverListener l) {
		this.listeners.Remove(l);
	}
}