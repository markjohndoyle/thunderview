#pragma strict

var power : float = 1500;
var moveSpeed : float = 5;

function Update () {
	var hInput : float = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
	var vInput : float = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;
	
	transform.Translate(hInput, vInput, 0);
}