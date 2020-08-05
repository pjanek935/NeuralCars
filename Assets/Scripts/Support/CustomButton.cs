using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.UI
{
	public class CustomButton : Button {

		Graphic [] graphics;

		protected override void Awake ()
		{
			base.Awake ();

			if (graphics == null)
			{
				graphics = GetComponentsInChildren <Graphic> ();
			}
		}

		protected override void DoStateTransition (SelectionState state, bool instant)
		{
			base.DoStateTransition (state, instant);

			Color color = this.colors.normalColor;

			switch (state)
			{
				case SelectionState.Disabled:
					color = this.colors.disabledColor;

					break;

				case SelectionState.Highlighted:
					color = this.colors.highlightedColor;

					break;

				case SelectionState.Normal:
					color = this.colors.normalColor;

					break;

				case SelectionState.Pressed:
					color = this.colors.pressedColor;

					break;
			}

			if (graphics != null)
			{
				for (int i = 0; i < graphics.Length; i++)
				{
                    if (graphics [i] != null)
                    {
                        graphics [i].CrossFadeColor (color, (instant ? 0f : this.colors.fadeDuration), true, true);
                    }
				}
			}
		}
	}
}

