using UnityEngine;
using System.Collections;

public class ChoosePlayerAnimationController : MonoBehaviour {
	
	public float shakeFrequency;
	public float spinningFrequency;

	public Sprite[] spinningSprites;

	private bool changeSprite;
	
	private int spriteNumber;
	
	private float x;
	private float y;
	private float time;
	private float spinningTimer;
	
	private GameManager manager;
	
	private enum State {
		Driving, Spinning, Facing
	}

	private State state;
	
	void Start () {

		// set start variables
		time = 0;
		spinningTimer = 0;
		changeSprite = false;
		state = State.Driving;

		// set position
		x = transform.position.x;
		y = transform.position.y;
		
		manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
	}
	
	void Update () {

		// spinning animation
		if( state == State.Spinning ) {
			
			// increase spriteNumber
			if( changeSprite ) {
				spriteNumber++;
				changeSprite = false;
			}
			
			// clip vs max
			if( spriteNumber >= spinningSprites.Length )
				spriteNumber = 0;

		} else if( state == State.Facing ) {

			if( spriteNumber == 4 )
				return;

			if( spriteNumber >= 5 && spriteNumber - 4 <= 10 && changeSprite ) {

				// decrease spriteNumber
				if( changeSprite ) {
					spriteNumber--;
					changeSprite = false;
				}
			} else {

				// increase spriteNumber
				if( changeSprite ) {
					spriteNumber++;

					if( spriteNumber >= 21 )
						spriteNumber = 0;

					changeSprite = false;
				}
			}

		} else
			spriteNumber = 0;

		GetComponentInChildren<SpriteRenderer>().sprite = spinningSprites[ spriteNumber ];

		// vibrations and translation
		if( Mathf.Sin( time * shakeFrequency ) > 0 && state == State.Driving )
			transform.position = new Vector2( x, y + 2f / 224f);
		else
			transform.position = new Vector2( x, y );
		
		time += Time.deltaTime;
		spinningTimer += Time.deltaTime;
		
		// reset timer
		if( spinningTimer >= spinningFrequency ) {
			changeSprite = true;
			spinningTimer = 0;
		}
	}

	public void SetStateToSpinning() {

		state = State.Spinning;
	}

	public void SetStateToDriving() {
		
		state = State.Driving;
	}

	public void SetStateToFacing() {
		
		state = State.Facing;
	}
}

