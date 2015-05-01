using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DraggableCircle : MonoBehaviour {

	Vector2 pos;
	Vector2 lerpFrom;
	bool dragging = false;
	Vector2 startPos;
	GameManager myManager;

	public bool slidding;
	public float startTime, fadeTime = 0.5f;

	public string currentCategory;
	public GameObject categoryGameObject;


	public Sprite m_neutralSprite;
	public Sprite hoveringSprite;
	Image m_image;
	public GameObject m_fill;


	Canvas myCanvas;
	// Use this for initialization
	void Start () {
		myCanvas = GameObject.Find("Canvas").GetComponent<Canvas>(); //TODO drag and drop or something
		myManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		startPos = transform.position;
		m_image = gameObject.GetComponent<Image>();
		m_neutralSprite = m_image.sprite;
	}
	
	// Update is called once per frame
	void Update () {
		SnapToCenter();
	}

	public void MoveToMouse(){
		RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform, Input.mousePosition, myCanvas.worldCamera, out pos);
		transform.position = myCanvas.transform.TransformPoint(pos);

	}

	//THIS BOOLEAN LOGIC WORKS DOOOOOG
	//doctors hate this
	public void toggleDragging(){
		dragging = !dragging;

		if(!dragging){
			startTime = Time.time;
			slidding = true;
			lerpFrom = transform.position;
		}

		if(slidding){
			//startTime = 1f;
			fadeTime = 0.5f;
		}
	}

	void SnapToCenter(){
		if(!dragging){
			if(slidding){
				float timePassed = (Time.time - startTime);
				float fracJourney = timePassed / fadeTime;
				transform.position = Vector2.Lerp(lerpFrom, startPos, fracJourney);

				if((Vector2)transform.position == startPos){
					slidding = false;
					//startTime = 1f;
					fadeTime = 0.5f;
					print ("done sliding!");
				}
			}
			//transform.position = startPos; //worse than lerp. switch to lerp

			//trying a wyatt lerp to center



		}
	}

	void OnTriggerEnter2D(Collider2D other){
		//print ("COLLISION");
		if(other.gameObject.GetComponentInChildren<Text>().text != currentCategory){
			myManager.incorrectAnswer = true;
			print ("Manager: " + myManager.incorrectAnswer);
		}
		currentCategory = other.gameObject.GetComponentInChildren<Text>().text;
		SpriteSwap();
		m_fill.GetComponent<Image>().color = other.GetComponent<Image>().color;
		categoryGameObject = other.gameObject;

		//print (currentCategory);
	}
	void OnTriggerExit2d(Collider2D other){
		currentCategory = "";
		SpriteSwap();
	}

	//switch to white outline circle that is the color of the category behind it once we tlak a term. switch back to blakc out line white center when no term. 
	void SpriteSwap(){
		if(m_image.sprite != hoveringSprite && currentCategory != ""){
			m_image.sprite = hoveringSprite;
			gameObject.GetComponentInChildren<Text>().color = Color.white;
		}
		else if(currentCategory == ""){
			m_image.sprite = m_neutralSprite;
			gameObject.GetComponentInChildren<Text>().color = Color.black;
		}
	}
}
