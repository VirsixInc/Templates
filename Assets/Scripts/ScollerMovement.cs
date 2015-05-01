using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScollerMovement : MonoBehaviour {

	//This class handles the touch/drag scroll movement for the GUI menu of assignments

	Scrollbar scrollbar;
	public GameObject myCanvas;
	public Vector2 pos;
	float initY, currentY;
	bool isDragging = false;

	void Start () {
		myCanvas = GameObject.Find ("Canvas");
		scrollbar = GameObject.Find ("Scrollbar").GetComponent<Scrollbar>();
	}


	public void OnPointerDown(){
		isDragging = true;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform, Input.mousePosition, myCanvas.GetComponent<Canvas>().worldCamera, out pos);
		initY = pos.y - transform.localPosition.y;

	}

	public void OnPointerUp() {
		isDragging = false;
	}

	void Update() {
	
#if UNITY_EDITOR
		if (Input.GetMouseButtonDown(0)){
			OnPointerDown();
		}
		if (Input.GetMouseButtonUp(0)){
			OnPointerUp();
		}
#else
		if (Input.GetTouch (0).phase == TouchPhase.Began)
			OnPointerDown();
		if (Input.GetTouch (0).phase == TouchPhase.Ended)
			OnPointerUp();
#endif
	


		if (isDragging) {
			RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform, Input.mousePosition, myCanvas.GetComponent<Canvas>().worldCamera, out pos);
			currentY = pos.y;
			transform.localPosition = new Vector3(0f, -(initY-currentY), 0f);
		}

		if (transform.localPosition.y < 0)
			transform.localPosition = new Vector3 (0, 0, 0);

		if (transform.localPosition.y > AssignmentManager.s_instance.upperBound)
			transform.localPosition = new Vector3 (0, AssignmentManager.s_instance.upperBound, 0);

		scrollbar.value = transform.localPosition.y / AssignmentManager.s_instance.upperBound;
	
	}
}
