using UnityEngine;
using System.Collections;

namespace org.hbird.application.thunderbirdviewer {

public class ThunderbirdTelemetry : MonoBehaviour {

	private string headingName;
	private string pitchName;
	private string rollName;
}
	
public class HummingbirdParameter {
		
	public HummingbirdParameter() {
	}
		
	public HummingbirdParameter(string qualifiedName, string name, string shortDescription, string longDescription) {
		this.name = name;
		this.qualifiedName = qualifiedName;
		this.shortDescription = shortDescription;
		this.longDescription = longDescription;
	}
		
	private string parameterQualifiedName;
	public string qualifiedName {
		get { return parameterQualifiedName;}
		set {parameterQualifiedName = value;}
	}
		
	private string parameterName;
	public string name {
		get {return parameterName;}
		set {parameterName = value;}
	}
		
	private string parameterShortDsc;
	public string shortDescription {
		get {return parameterShortDsc;}
		set {parameterShortDsc = value;}
	}
		
	private string parameterLongDsc;
	public string longDescription {
		get {return parameterLongDsc;}
		set {parameterLongDsc = value;}
	}
		
	private long parameterReceivedTimestamp;
	public long receivedTime {
		get {return parameterReceivedTimestamp;}
		set {parameterReceivedTimestamp = value;}
	}
		
	private int parameterValue;
	public int value {
		get {return parameterValue;}
		set { this.parameterValue = value;}
	}

}
	
}
