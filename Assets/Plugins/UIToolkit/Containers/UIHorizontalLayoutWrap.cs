using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class UIHorizontalLayoutWrap : UIAbstractContainerWrap
{
	public UIHorizontalLayoutWrap( int spacing ) : base( UILayoutType.Horizontal )
	{
		_spacing = spacing;
	}


}
