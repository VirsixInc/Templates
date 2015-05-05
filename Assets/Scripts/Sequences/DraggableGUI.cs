using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DraggableGUI : MonoBehaviour {

	public string stringValue;
	public Button image;
	public Sprite sprite; //to be loaded from Resources/SubFolder/
	public bool isDragging = false, isSnapped = false, isMismatched = false;
	public GameObject currentTarget;
	public Vector3 currentDirection;
	public GameObject myCanvas;
	public Vector2 pos;
	Event e;
	float speed = 1;

	void Start () {
		image = GetComponentInChildren<Button> ();
		RandomizeDirection ();
		myCanvas = GameObject.Find ("GameCanvas");


	}

	void RandomizeDirection(){
		float randX = Random.Range (-100, 100);
		float randY = Random.Range (-100, 100);
		currentDirection = new Vector3 (randX, randY, 0).normalized;
	}

	void Update () {
		if (isDragging)
			MoveToMouse();
		if (!isSnapped)
			FloatAround ();
	}

	void ResetToCenter(){
		isDragging = false;
		transform.localPosition = new Vector3 (0f, 0f, 0f);

	}

	public void OnPointerDown () {
		isDragging = true;
		if (isSnapped) {
			currentTarget.GetComponent<TargetGUI> ().isOccupied = false;
			isSnapped = false;
		}
	}

	public void MoveToMouse(){
		RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform, Input.mousePosition, myCanvas.GetComponent<Canvas>().worldCamera, out pos);
		transform.position = myCanvas.transform.TransformPoint(pos);

	}

//	void OnGUI() {
//		e = Event.current;
//		if (isDragging) {
//			gameObject.transform.position = new Vector3(e.mousePosition.x, Screen.height-e.mousePosition.y, 0);
//		}
//	}

	public void OnPointerUp () {
		isDragging = false;
		RandomizeDirection ();

	}

	void SnapToTarget () {
		if (currentTarget != null) {
			if (SoundManager.s_instance!=null) SoundManager.s_instance.PlaySound (SoundManager.s_instance.m_snap);
			gameObject.transform.position = currentTarget.transform.position;
			isSnapped = true;
			currentTarget.GetComponent<TargetGUI>().isOccupied = true;
			currentTarget.GetComponent<TargetGUI>().occupier = gameObject;
			isDragging = false;
		}
	}

	public void AutoFillToTarget(GameObject target){
		currentTarget = target;
		gameObject.transform.position = target.transform.position;
		isSnapped = true;
		target.GetComponent<TargetGUI>().isOccupied = true;
		target.GetComponent<TargetGUI>().occupier = gameObject;
		isDragging = false;
	
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.gameObject.tag == "Target" && other.gameObject.GetComponent<TargetGUI>().isOccupied == false) {
			currentTarget= other.gameObject;
			isDragging = false;
			SnapToTarget ();
		}
		else if (other.gameObject.tag == "Resetter") {
			ResetToCenter();
		}

		else if (other.gameObject.tag == "BoundaryH") {
			ChangeDirectionH();
		}
		else if (other.gameObject.tag == "BoundaryV") {
			ChangeDirectionV();
		}
		else if (other.gameObject.tag == "Resetter") {
			transform.position = new Vector3(Screen.width/2, Screen.height/2, 0);
		}
	}



	public void SetValues (string s, GameType g) {

		if (g == GameType.Text) {
			stringValue = s;
			GetComponentInChildren<Text>().text = s;
		}
	}

	void FloatAround() {
		//moves the object in a direction
		gameObject.transform.Translate (currentDirection.x * speed, currentDirection.y * speed, 0);
		//print (currentDirection);
	}

	void ChangeDirectionH() {
		currentDirection = new Vector3 (currentDirection.x, -currentDirection.y, 0);
	}

	void ChangeDirectionV(){
		currentDirection = new Vector3 (-currentDirection.x, currentDirection.y, 0);
	}

}
