using UnityEngine;

namespace Outfitted
{
	public interface ITabView
	{
		void Draw(Rect inRect);
		bool Enabled();
		string GetLabel();
	}
}
