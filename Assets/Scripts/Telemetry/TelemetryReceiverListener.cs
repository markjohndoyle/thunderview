using UnityEngine;
using System.Collections;

namespace org.hbird.application.thunderbirdviewer.tm {
	
public interface TelemetryReceiverListener {
	void telemetryReceived(HummingbirdParameter tm);
}
	
}
