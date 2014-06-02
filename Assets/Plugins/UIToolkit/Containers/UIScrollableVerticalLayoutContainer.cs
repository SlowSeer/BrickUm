using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class UIScrollableVerticalLayoutContainer : UIAbstractTouchableContainer {

    // List of layout children
    //List<UIAbstractContainer> _layouts = new List<UIAbstractContainer>();

    public float myPadding = 0;

    public UIScrollableVerticalLayoutContainer(int spacing) : base(UILayoutType.Vertical, spacing) {}
    
	protected override void clipChild( UISprite child )
	{
		var topContained = child.position.y < -touchFrame.yMin && child.position.y > -touchFrame.yMax;
		var bottomContained = child.position.y - child.height < -touchFrame.yMin && child.position.y - child.height > -touchFrame.yMax;

		// first, handle if we are fully visible
		if( topContained && bottomContained )
		{
			// unclip if we are clipped
			if( child.clipped )
				child.clipped = false;
			child.hidden = false;
		}
		else if( topContained || bottomContained )
		{
			// wrap the changes in a call to beginUpdates to avoid changing verts more than once
			child.beginUpdates();

			child.hidden = false;

			// are we clipping the top or bottom?
			if( topContained ) // clipping the bottom
 			{
				var clippedHeight = child.position.y + touchFrame.yMax;

				child.uvFrameClipped = child.uvFrame.rectClippedToBounds( child.width / child.scale.x, clippedHeight / child.scale.y, UIClippingPlane.Bottom, child.manager.textureSize );
				child.setClippedSize( child.width / child.scale.x, clippedHeight / child.scale.y, UIClippingPlane.Bottom );
			}
			else // clipping the top, so we need to adjust the position.y as well
 			{
				var clippedHeight = child.height - child.position.y - touchFrame.yMin;

				child.uvFrameClipped = child.uvFrame.rectClippedToBounds( child.width / child.scale.x, clippedHeight / child.scale.y, UIClippingPlane.Top, child.manager.textureSize );
				child.setClippedSize( child.width / child.scale.x, clippedHeight / child.scale.y, UIClippingPlane.Top );
			}

			// commit the changes
			child.endUpdates();
		}
		else
		{
			// fully outside our bounds
			child.hidden = true;
		}

		// Recurse
		recurseAndClipChildren( child );
	}
    
    /// <summary>
    /// Responsible for laying out the child Layouts who will in turn 
    /// layout their child sprites
    /// </summary>
    protected override void layoutChildren() {
        //Debug.Log("In UILayoutContainer's layoutChildren");
        if (_suspendUpdates)
            return;

        //var hdFactor = 1f / UI.scaleFactor;
        var anchorInfo = UIAnchorInfo.DefaultAnchorInfo();
        anchorInfo.ParentUIObject = this;
        var i = 0;
        var lastIndex = _children.Count;
        _contentHeight = 0;
        foreach (var item in _children) {
            if (item as UIAbstractContainer2 != null) {
                // we add spacing for all but the first and last
                if (i != 0 && i != lastIndex) {
                    _contentHeight += _spacing;
                } else {
                    _contentHeight += myPadding;
                }

                item.localPosition = new Vector3(_edgeInsets.left, -(_contentHeight + _scrollPosition), item.localPosition.z);

                // all items get their height added
                _contentHeight += ((UIAbstractContainer2)item).contentHeight;

                // width will just be the width of the widest item
                if (_contentWidth < ((UIAbstractContainer2)item).contentWidth)
                    _contentWidth = ((UIAbstractContainer2)item).contentWidth;

            } else if (item as UISprite != null) {

                
                // we add spacing for all but the first and last
                if (i != 0 && i != lastIndex) {
                    _contentHeight += _spacing;
                } else {
                    _contentHeight += myPadding;
                }

                item.localPosition = new Vector3(_edgeInsets.left, -(_contentHeight + _scrollPosition), item.localPosition.z);

                // all items get their height added
                _contentHeight += ((UISprite)item).height;

                // width will just be the width of the widest item
                if (_contentWidth < ((UISprite)item).width)
                    _contentWidth = ((UISprite)item).width;
                
                /*
                if (i != 0 && i != lastIndex)
                    _contentHeight += _spacing;

                // Set anchor offset
                anchorInfo.OffsetY = (_contentHeight + _scrollPosition) * hdFactor;
                //anchorInfo.OffsetX = item.anchorInfo.OffsetX;

                // dont overwrite the sprites origin anchor!
                anchorInfo.OriginUIxAnchor = ((UISprite)item).anchorInfo.OriginUIxAnchor;
                anchorInfo.OriginUIyAnchor = ((UISprite)item).anchorInfo.OriginUIyAnchor;
                ((UISprite)item).anchorInfo = anchorInfo;

                // all items get their height added
                _contentHeight += ((UISprite)item).height;

                // width will just be the width of the widest item
                if (_contentWidth < ((UISprite)item).width)
                    _contentWidth = ((UISprite)item).width;*/
            } else if (item as UITextInstance != null) {
                // we add spacing for all but the first and last
                if (i != 0 && i != lastIndex) {
                    _contentHeight += _spacing;
                } else {
                    _contentHeight += myPadding;
                }

                item.localPosition = new Vector3(_edgeInsets.left, -(_contentHeight + _scrollPosition), item.localPosition.z);

                // all items get their height added
                _contentHeight += ((UITextInstance)item).height;

                // width will just be the width of the widest item
                if (_contentWidth < ((UITextInstance)item).width)
                    _contentWidth = ((UITextInstance)item).width;
            }

            i++;
        }

        // add the right and bottom edge inset to finish things off
        _contentWidth += _edgeInsets.right;
        _contentHeight += _edgeInsets.bottom;

        //matchSizeToContentSize();
        clipToBounds();
    }


	public override void onTouchMoved( myTouch touch, Vector2 touchPos )
	{
		// increment deltaTouch so we can pass on the touch if necessary
		_deltaTouch += touch.deltaPosition.y;
		_lastTouch = touch;

        /*if (Vector2.Magnitude(touchStartPos, touchPos) < 10) {
        }*/

		// once we move too far unhighlight and stop tracking the touchable
		if( _activeTouchable != null && Mathf.Abs( _deltaTouch ) > TOUCH_MAX_DELTA_FOR_ACTIVATION )
		{
			_activeTouchable.onTouchEnded( touch, touchPos, true );
			_activeTouchable = null;
		}


		var newTop = _scrollPosition - touch.deltaPosition.y;
		
		// are we dragging above/below the scrollables boundaries?
		_isDraggingPastExtents = ( newTop > 0 || newTop < _minEdgeInset.y );
		
		// if we are dragging past our extents dragging is no longer 1:1. we apply an exponential falloff
		if( _isDraggingPastExtents )
		{
			// how far from the top/bottom are we?
			var distanceFromSource = 0f;
			
			if( newTop > 0 ) // stretching down
				distanceFromSource = newTop;
			else
				distanceFromSource = Mathf.Abs( _contentHeight + newTop - height );
			
			// we need to know the percentage we are from the source
			var percentFromSource = distanceFromSource / height;
			
			// apply exponential falloff so that the further we are from source the less 1 pixel drag actually goes
			newTop = _scrollPosition - ( touch.deltaPosition.y * Mathf.Pow( 0.04f, percentFromSource ) );
		}
		
		
		_scrollPosition = newTop;
		layoutChildren();

		// pop any extra velocities and push the current velocity onto the stack
		if( _velocities.Count == TOTAL_VELOCITY_SAMPLE_COUNT )
			_velocities.Dequeue();
		_velocities.Enqueue( touch.deltaPosition.y / Time.deltaTime );
	}


    // clip hidden children of layouts
    protected override void clipToBounds() {
        /* // Commented out for testing.
        foreach (var layout in _layouts) {
            foreach (var child in layout.Children) {
                clipChild(child);
            }
        }
        */
    }

    protected override ITouchable getButtonForScreenPosition(Vector2 touchPosition) {
        // we loop backwards so that any clipped elements at the top dont try to override the hitTest
        // due to their frame overlapping the touchable below
        /*for (var l = _children.Count - 1; l >= 0; l--) {

            for (var i = _children[l].Children.Count - 1; i >= 0; i--) {
                var touchable = _children[l].Children[i];
                if (touchable != null) {
                    ITouchable touched = testTouchable(touchable, touchPosition); // Recursive
                    if (touched != null)
                        return touched;
                }
            }
        }*/

        return recurseAllContainers(Children, touchPosition);
    }

    ITouchable tempTouchable;
    ITouchable recurseAllContainers(List<UIObject> childs, Vector2 touchPosition) {
        //Debug.Log("NEW CONTAINER, size: " + childs.Count + " (" + Time.time + ")");
        for (var i = childs.Count - 1; i >= 0; i--) {
            if (childs[i] as UIAbstractContainer2 != null) {
                tempTouchable = recurseAllContainers(((UIAbstractContainer2)childs[i]).Children, touchPosition);
                if (tempTouchable != null) return tempTouchable;
            }else{
                if (childs[i] as UISprite != null) {
                //Debug.Log("child not container" + " (" + Time.time + ")");
                    var touchable = (UISprite)childs[i];
                    if (touchable != null) {
                        //Debug.Log("child touchable: " + (childs[i] as ITouchable).touchFrame.ToString() + " " + childs[i].client.name + " (" + Time.time + ")");
                        if((childs[i] as ITouchable).touchFrame.height > 0){
                            ITouchable touched = testTouchable(touchable, touchPosition); // Recursive
                            if (touched != null) {
                                //Debug.Log("Found Touchable @ " + touchPosition.ToString());
                                return touched;
                            }
                        }
                    } else {
                        //Debug.Log("child not touchable" + " (" + Time.time + ")");
                    }
                }
            }
        }
        //Debug.Log("nothing touched, size: " + childs.Count + " (" + Time.time + ")");
        return null;
    }
}
