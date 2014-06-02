using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class UIAbsoluteLayoutWrap : UIAbstractContainerWrap
{
    public UIAbsoluteLayoutWrap(int spacing)
        : base(UILayoutType.AbsoluteLayout)
	{
		_spacing = spacing;
	}
     

}
