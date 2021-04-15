using UnityEngine;
using System.Collections;

public class MenuAnimationController : MonoBehaviour {

	public bool willSpin;
	public bool willInvincible;

	public float drivingTranslationSpeed;
	public float spinningTranslationSpeed;
	public float invincibleTranslationSpeed;
	public float spinningFrequency;
	public float invincibleFrequency;
	public float shakeFrequency;
	public float spinningStartTime;
	public float spinningEndTime;
	public float invincibleStartTime;
	public float invincibleEndTime;
	public float appearTime;
	public float dissappearTime;

	public Sprite[] spinningSprites;
	public Sprite[] invincibleSprites;

	private bool stateChanged;
	private bool changeSprite;

	private int spriteNumber;

	private float x;
	private float y;
	private float time;
	private float speed;
	private float offset;
	private float spinningTimer;

	private SpriteRenderer sr;

	private GameManager manager;

	enum State{ Driving, Spinning, Invincible };

	State state;

	void Start () {

		// set start variables
		time = 0;
		spinningTimer = 0;
		stateChanged = true;
		changeSprite = false;
	
		sr = gameObject.GetComponent< SpriteRenderer >();
		sr.enabled = false;

		// set position
		x = transform.position.x;
		y = transform.position.y;

		manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
	}

	void Update () {

		if( manager.menuActive ) {
			time = 0;
			sr.enabled = false;
		} else
			sr.enabled = true;

		if( time >= dissappearTime || time <= appearTime )
			sr.enabled = false;

		setState();

		// set speed value if state has changed
		if( stateChanged ) {
			switch( state ) {
			case State.Driving:
				speed = drivingTranslationSpeed;
				break;
			case State.Spinning:
				offset = speed * time;
				speed = spinningTranslationSpeed;
				offset -= speed  * time;
				break;
			case State.Invincible:
				offset += speed * time;
				speed = invincibleTranslationSpeed;
				offset -= speed  * time;
				spriteNumber = 0;
				break;
			default:
				speed = (float)drivingTranslationSpeed;
				break;
			}
			stateChanged = false;
		}

		// spinning animation
		if( state == State.Spinning ) {
			GetComponentInChildren<SpriteRenderer>().sprite = spinningSprites[ spriteNumber ];

			// increase spriteNumber
			if( changeSprite ) {
				spriteNumber++;
				changeSprite = false;
			}

			// clip vs max
			if( spriteNumber >= spinningSprites.Length )
				spriteNumber = 0;
		}

		// invincible animation
		if( state == State.Invincible ) {
			GetComponentInChildren<SpriteRenderer>().sprite = invincibleSprites[ spriteNumber ];
			
			// increase spriteNumber
			if( Time.frameCount % invincibleFrequency == 0 )
				spriteNumber++;
			
			// clip vs max
			if( spriteNumber >= invincibleSprites.Length )
				spriteNumber = 0;
		}

		// vibrations and translation
		if( Mathf.Sin( time * shakeFrequency ) > 0 && state == State.Driving )
			transform.position = new Vector2( x + speed * time + offset, y + Screen.height * .000015f);
		else
			transform.position = new Vector2( x + speed * time + offset, y );

		time += Time.deltaTime;
		spinningTimer += Time.deltaTime;

		// reset timer
		if( spinningTimer >= spinningFrequency ) {
			changeSprite = true;
			spinningTimer = 0;
		}
	}

	State setState() {

		// set state
		if( willSpin && time >= spinningStartTime && time <= spinningEndTime && state != State.Spinning ) {
			state = State.Spinning;
			stateChanged = true;
		}
		if( willInvincible && time >= invincibleStartTime && time <= invincibleEndTime && state != State.Invincible ) {
			state = State.Invincible;
			stateChanged = true;
		}

		return state;
	}
}
