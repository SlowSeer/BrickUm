using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public abstract class UIAbstractContainerWrap : UIAbstractContainer2
{


	public UIAbstractContainerWrap() : this( UILayoutType.Vertical )
	{}


	/// <summary>
	/// We need the layout type set from the getgo so we can lay things out properly
	/// </summary>
	public UIAbstractContainerWrap( UILayoutType layoutType )
	{
		//_layoutType = layoutType;
        this.layoutType = layoutType;
	}



    protected override void layoutChildren(){
		if( _suspendUpdates )
			return;

		// Get HD factor
		var hdFactor = 1f / UI.scaleFactor;

		// rules for vertical and horizontal layouts
        if (layoutType == UIAbstractContainerWrap.UILayoutType.Horizontal || layoutType == UIAbstractContainerWrap.UILayoutType.Vertical)
		{
			// start with the insets, then add each object + spacing then end with insets
			_contentWidth = _edgeInsets.left;
			_contentHeight = _edgeInsets.top;

			// create UIAnchorInfo to control positioning
			var anchorInfo = UIAnchorInfo.DefaultAnchorInfo();
			anchorInfo.ParentUIObject = this;

            if (layoutType == UIAbstractContainerWrap.UILayoutType.Horizontal)
			{
				// Set anchor information
				switch( verticalAlignMode )
				{
					case UIContainerVerticalAlignMode.Top:
						anchorInfo.UIyAnchor = UIyAnchor.Top;
						anchorInfo.ParentUIyAnchor = UIyAnchor.Top;
						anchorInfo.OffsetY = _edgeInsets.top * hdFactor;
						break;
					case UIContainerVerticalAlignMode.Middle:
						anchorInfo.UIyAnchor = UIyAnchor.Center;
						anchorInfo.ParentUIyAnchor = UIyAnchor.Center;
						break;
					case UIContainerVerticalAlignMode.Bottom:
						anchorInfo.UIyAnchor = UIyAnchor.Bottom;
						anchorInfo.ParentUIyAnchor = UIyAnchor.Bottom;
						anchorInfo.OffsetY = _edgeInsets.bottom * hdFactor;
						break;
				}

				var i = 0;
				var lastIndex = _children.Count;
                float tempWidth = 0;
                List<float> rows = new List<float>(); rows.Add(0); // first row.
				foreach( var item in _children ) {
                    if (item as UIAbstractContainer2 != null) {
                        // -WIP----------------------------------------------------------------------
                        // we add spacing for all but the first and last
                        if (i != 0 && i != lastIndex)
                            _contentWidth += _spacing;

                        // Set anchor offset

                        if (position.x + tempWidth + ((UIAbstractContainer2)item).width > Screen.width) {
                            rows.Add(0);
                            tempWidth = 0;
                            anchorInfo.OffsetY = (_contentHeight) * hdFactor;
                        }

                        anchorInfo.OffsetX = (tempWidth + _scrollPosition) * hdFactor;
                        tempWidth += ((UIAbstractContainer2)item).width;

                        if (rows[rows.Count - 1] < ((UIAbstractContainer2)item).height)
                            rows[rows.Count - 1] = ((UIAbstractContainer2)item).height + _spacing;

                        if (tempWidth > _contentWidth)
                            _contentWidth = tempWidth;

                        // dont overwrite the sprites origin anchor!
                        anchorInfo.OriginUIxAnchor = ((UIAbstractContainer2)item).anchorInfo.OriginUIxAnchor;
                        anchorInfo.OriginUIyAnchor = ((UIAbstractContainer2)item).anchorInfo.OriginUIyAnchor;

                        ((UIAbstractContainer2)item).anchorInfo = anchorInfo;

                        _contentHeight = 0;
                        foreach (float row in rows) _contentHeight += row;
                        // --------------------------------------------------------------------------
                    } else if (item as UISprite != null) {
                        // -WIP----------------------------------------------------------------------
                        // we add spacing for all but the first and last
                        if (i != 0 && i != lastIndex)
                            _contentWidth += _spacing;

                        // Set anchor offset

                        if (position.x + tempWidth + ((UISprite)item).width > Screen.width) {
                            rows.Add(0);
                            tempWidth = 0;
                            anchorInfo.OffsetY = (_contentHeight) * hdFactor;
                        }

                        anchorInfo.OffsetX = (tempWidth + _scrollPosition) * hdFactor;
                        tempWidth += ((UISprite)item).width;

                        if (rows[rows.Count - 1] < ((UISprite)item).height)
                            rows[rows.Count - 1] = ((UISprite)item).height;

                        if (tempWidth > _contentWidth)
                            _contentWidth = tempWidth;

                        // dont overwrite the sprites origin anchor!
                        anchorInfo.OriginUIxAnchor = ((UISprite)item).anchorInfo.OriginUIxAnchor;
                        anchorInfo.OriginUIyAnchor = ((UISprite)item).anchorInfo.OriginUIyAnchor;

                        ((UISprite)item).anchorInfo = anchorInfo;

                        _contentHeight = 0;
                        foreach (float row in rows) _contentHeight += row;
                        // --------------------------------------------------------------------------

                    } else if (item as UITextInstance != null) {
                        // we add spacing for all but the first and last
                        if (i != 0 && i != lastIndex)
                            _contentHeight += _spacing;

                        // Set anchor offset
                        //anchorInfo.OffsetY = (_contentHeight + _scrollPosition) * hdFactor;
                        //anchorInfo.OffsetX = item.anchorInfo.OffsetX;

                        // Set anchor offset

                        if (position.x + tempWidth + ((UITextInstance)item).width > Screen.width) {
                            rows.Add(0);
                            tempWidth = 0;
                            anchorInfo.OffsetY = (_contentHeight) * hdFactor;
                            Debug.Log("newtextrow");
                        }

                        anchorInfo.OffsetX = (tempWidth + _scrollPosition) * hdFactor;
                        tempWidth += ((UITextInstance)item).width;

                        if (rows[rows.Count - 1] < ((UITextInstance)item).height)
                            rows[rows.Count - 1] = ((UITextInstance)item).height;

                        if (tempWidth > _contentWidth)
                            _contentWidth = tempWidth;

                        // dont overwrite the sprites origin anchor!
                        anchorInfo.OriginUIxAnchor = ((UITextInstance)item).anchorInfo.OriginUIxAnchor;
                        anchorInfo.OriginUIyAnchor = ((UITextInstance)item).anchorInfo.OriginUIyAnchor;
                        ((UITextInstance)item).anchorInfo = anchorInfo;

                        // all items get their height added
                        _contentHeight += ((UITextInstance)item).height;

                        // width will just be the width of the widest item
                        if (_contentWidth < ((UITextInstance)item).width)
                            _contentWidth = ((UITextInstance)item).width;
                    }
					i++;
				}
			}
			else // vertical alignment
			{
				// Set anchor information
				switch( alignMode )
				{
					case UIContainerAlignMode.Left:
						anchorInfo.UIxAnchor = UIxAnchor.Left;
						anchorInfo.ParentUIxAnchor = UIxAnchor.Left;
						anchorInfo.OffsetX = _edgeInsets.left * hdFactor;
						break;
					case UIContainerAlignMode.Center:
						anchorInfo.UIxAnchor = UIxAnchor.Center;
						anchorInfo.ParentUIxAnchor = UIxAnchor.Center;
						break;
					case UIContainerAlignMode.Right:
						anchorInfo.UIxAnchor = UIxAnchor.Right;
						anchorInfo.ParentUIxAnchor = UIxAnchor.Right;
						anchorInfo.OffsetX = _edgeInsets.right * hdFactor;
						break;
				}

				var i = 0;
				var lastIndex = _children.Count;
				foreach( var item in _children )
				{
                    if (item as UIAbstractContainer2 != null){
                        // we add spacing for all but the first and last
                        if (i != 0 && i != lastIndex)
                            _contentHeight += _spacing;

                        // Set anchor offset
                        anchorInfo.OffsetY = (_contentHeight + _scrollPosition) * hdFactor;
                        //anchorInfo.OffsetX = item.anchorInfo.OffsetX;

                        // dont overwrite the sprites origin anchor!
                        anchorInfo.OriginUIxAnchor = ((UIAbstractContainer2)item).anchorInfo.OriginUIxAnchor;
                        anchorInfo.OriginUIyAnchor = ((UIAbstractContainer2)item).anchorInfo.OriginUIyAnchor;
                        ((UIAbstractContainer2)item).anchorInfo = anchorInfo;

                        // all items get their height added
                        _contentHeight += ((UIAbstractContainer2)item).height;

                        // width will just be the width of the widest item
                        if (_contentWidth < ((UIAbstractContainer2)item).width)
                            _contentWidth = ((UIAbstractContainer2)item).width;
                    } else if (item as UISprite != null) {
                        // we add spacing for all but the first and last
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
                            _contentWidth = ((UISprite)item).width;
                    } else if (item as UITextInstance != null) {
                        // we add spacing for all but the first and last
                        if (i != 0 && i != lastIndex)
                            _contentHeight += _spacing;

                        // Set anchor offset
                        anchorInfo.OffsetY = (_contentHeight + _scrollPosition) * hdFactor;
                        //anchorInfo.OffsetX = item.anchorInfo.OffsetX;

                        // dont overwrite the sprites origin anchor!
                        anchorInfo.OriginUIxAnchor = ((UITextInstance)item).anchorInfo.OriginUIxAnchor;
                        anchorInfo.OriginUIyAnchor = ((UITextInstance)item).anchorInfo.OriginUIyAnchor;
                        ((UITextInstance)item).anchorInfo = anchorInfo;

                        // all items get their height added
                        _contentHeight += ((UITextInstance)item).height;

                        // width will just be the width of the widest item
                        if (_contentWidth < ((UITextInstance)item).width)
                            _contentWidth = ((UITextInstance)item).width;
                    }

					i++;
				}
			}

			// add the right and bottom edge inset to finish things off
			_contentWidth += _edgeInsets.right;
			_contentHeight += _edgeInsets.bottom;
		} else if (layoutType == UIAbstractContainerWrap.UILayoutType.AbsoluteLayout){
			foreach( var item in _children ){
                if (item as UIAbstractContainer2 != null) {

                    UIAbstractContainer2 temp = (UIAbstractContainer2)item;
                    //temp.localPosition = new Vector3(temp.position.x, temp.position.y, temp.position.z);

                    // find the width that contains the item with the largest offset/width
                    if (_contentWidth < temp.localPosition.x + temp.width)
                        _contentWidth = temp.localPosition.x + temp.width;

                    // find the height that contains the item with the largest offset/height
                    if (_contentHeight < -temp.localPosition.y + temp.height)
                        _contentHeight = -temp.localPosition.y + temp.height;

                } else if (item as UISprite != null) {

                    UISprite temp = (UISprite)item;
                    //temp.localPosition = new Vector3(temp.position.x, temp.position.y, temp.position.z);

                    // find the width that contains the item with the largest offset/width
                    if (_contentWidth < temp.localPosition.x + temp.width)
                        _contentWidth = temp.localPosition.x + temp.width;

                    // find the height that contains the item with the largest offset/height
                    if (_contentHeight < -temp.localPosition.y + temp.height)
                        _contentHeight = -temp.localPosition.y + temp.height;
                } else if (item as UITextInstance != null) {

                    UITextInstance temp = (UITextInstance)item;
                    //temp.localPosition = new Vector3(temp.position.x, temp.position.y, temp.position.z);

                    // find the width that contains the item with the largest offset/width
                    if (_contentWidth < temp.localPosition.x + temp.width)
                        _contentWidth = temp.localPosition.x + temp.width;

                    // find the height that contains the item with the largest offset/height
                    if (_contentHeight < -temp.localPosition.y + temp.height)
                        _contentHeight = -temp.localPosition.y + temp.height;
                }
			}
		}

		// Refresh child position to proper positions
		foreach( var item in _children )
			item.refreshPosition();

        matchSizeToContentSize();
	}
}
