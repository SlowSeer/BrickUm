using UnityEngine;
using System;
using System.Collections;
using System.Reflection;

public class myTouch {
    public int m_FingerId = 0;
    public int m_TapCount = 0;
    public Vector2 m_Position = Vector2.zero;
    public Vector2 m_PositionDelta = Vector2.zero;
    public float m_TimeDelta = 0;
    public TouchPhase m_Phase = TouchPhase.Ended;

    public int fingerId { get; set; }
    public int tapCount { get; set; }
    public Vector2 position { get; set; }
    public Vector2 deltaPosition { get; set; }
    public float deltaTime { get; set; }
    public TouchPhase phase { get; set; }

    public myTouch() {
        m_FingerId = 0;
        m_TapCount = 0;
        m_Position = Vector2.zero;
        m_PositionDelta = Vector2.zero;
        m_TimeDelta = 0;
        m_Phase = TouchPhase.Ended;

        //set(t);
    }

    public void set(Touch t) {
        fingerId = t.fingerId;
        tapCount = t.tapCount;
        position = t.position;
        deltaPosition = t.deltaPosition;
        deltaTime = t.deltaTime;
        phase = t.phase;

    }
}

#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER || UNITY_WEBGL
public enum UIMouseState
{
	UpThisFrame,
	DownThisFrame,
	HeldDown
};



/// <summary>
/// this class now exists only to allow standalones/web players to create Touch objects
/// </summary>
public struct UITouchMaker
{
    public static myTouch createTouch(int finderId, int tapCount, Vector2 position, Vector2 deltaPos, float timeDelta, TouchPhase phase)
	{
        var self = new myTouch();
		//ValueType valueSelf = self;
        var type = typeof(myTouch);
        
		//type.GetField( "m_FingerId", BindingFlags.Instance | BindingFlags.NonPublic ).SetValue( valueSelf, finderId );
        self.fingerId = finderId;
		//type.GetField( "m_TapCount", BindingFlags.Instance | BindingFlags.NonPublic ).SetValue( valueSelf, tapCount );
        self.tapCount = tapCount;
		//type.GetField( "m_Position", BindingFlags.Instance | BindingFlags.NonPublic ).SetValue( valueSelf, position );
        self.position = position;
		//type.GetField( "m_PositionDelta", BindingFlags.Instance | BindingFlags.NonPublic ).SetValue( valueSelf, deltaPos );
        self.deltaPosition = deltaPos;
		//type.GetField( "m_timeDelta", BindingFlags.Instance | BindingFlags.NonPublic ).SetValue( valueSelf, timeDelta );
        self.deltaTime = timeDelta;
		//type.GetField( "m_Phase", BindingFlags.Instance | BindingFlags.NonPublic ).SetValue( valueSelf, phase );
        self.phase = phase;

        //return (myTouch)valueSelf;
        return self;
	}


    public static myTouch createTouchFromInput(UIMouseState mouseState, ref Vector2? lastMousePosition)
	{
        //try{
            var self = new myTouch();
		    //ValueType valueSelf = self;
            var type = typeof(myTouch);
		
		    var currentMousePosition = new Vector2( Input.mousePosition.x, Input.mousePosition.y );
		
		    // if we have a lastMousePosition use it to get a delta
            if (lastMousePosition.HasValue)
                self.deltaPosition = (Vector2)(currentMousePosition - lastMousePosition);
                //type.GetField("m_PositionDelta", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(valueSelf, currentMousePosition - lastMousePosition);
		
		    if( mouseState == UIMouseState.DownThisFrame ) // equivalent to touchBegan
		    {
                self.phase = TouchPhase.Began;
                //type.GetField("m_Phase", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(valueSelf, TouchPhase.Began);
			    lastMousePosition = Input.mousePosition;
		    }
		    else if( mouseState == UIMouseState.UpThisFrame ) // equivalent to touchEnded
		    {
                self.phase = TouchPhase.Ended;
                //type.GetField("m_Phase", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(valueSelf, TouchPhase.Ended);
			    lastMousePosition = Vector2.zero;
		    }
		    else // UIMouseState.HeldDown - equivalent to touchMoved/Stationary
		    {
                self.phase = TouchPhase.Moved;
                //type.GetField("m_Phase", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(valueSelf, TouchPhase.Moved);
			    lastMousePosition = Input.mousePosition;
		    }

            self.position = currentMousePosition;
            //type.GetField("m_Position", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(valueSelf, currentMousePosition);


            //return (myTouch)valueSelf;
            return self;
        /*}catch(Exception e){
            Statics.Instance.myString = e.ToString();
        }
        return new Touch();*/
	}
}
#endif