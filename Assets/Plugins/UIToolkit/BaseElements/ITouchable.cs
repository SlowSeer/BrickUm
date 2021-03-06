using UnityEngine;
using System.Collections;


public interface ITouchable
{
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER || UNITY_WEBGL
	bool hoveredOver { get; set; }
#endif
	bool highlighted { get; set; }
	bool hidden { get; set; }
	bool allowTouchBeganWhenMovedOver { get; set; } // if true, we allow a touch moved over the button to fire onTouchBegan
	Rect touchFrame { get; }
	Vector3 position { get; set; }
	
	
	bool hitTest( Vector2 point );
	
	
	void onTouchBegan( myTouch touch, Vector2 touchPos );

	
	void onTouchMoved( myTouch touch, Vector2 touchPos );
	

	void onTouchEnded( myTouch touch, Vector2 touchPos, bool touchWasInsideTouchFrame );

}
