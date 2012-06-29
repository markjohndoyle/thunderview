using UnityEngine;
using System.Collections;

namespace org.hbird.application.thunderbirdviewer.tm {
	
public interface TelemetryProvider {
		void registerForTelemeytryUpdates(TelemetryReceiverListener l);
		void unregisterForTelemeytryUpdates(TelemetryReceiverListener l);
}
	
}
