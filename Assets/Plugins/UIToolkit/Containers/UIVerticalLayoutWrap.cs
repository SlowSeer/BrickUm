using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class UIVerticalLayoutWrap : UIAbstractContainerWrap
{
    public UIVerticalLayoutWrap(int spacing) : base(UILayoutType.Vertical){
		_spacing = spacing;
	}


}
