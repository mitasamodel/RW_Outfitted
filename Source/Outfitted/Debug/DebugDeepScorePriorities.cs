using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outfitted.RW_JustUtils;

namespace Outfitted
{
#if DEBUG
	internal static class DebugDeepScorePriorities
	{
		private static bool _started = false;
		private static bool _overlap = false;
		private static readonly StringBuilder _showString = new StringBuilder();

		public static void AddToLog(string str)
		{
			if (MyDebug.DeepScorePriorities && !_overlap && _started)
			{
				_showString.Append(str);
			}
		}

		public static void ShowLog()
		{
			if (MyDebug.DeepScorePriorities)
			{
				if (_started)
					_showString.Append($"[DebugDeepScorePriorities] Finished.\n");
				Logger.Log(_showString.ToString());
				Clear();
			}
		}

		public static void Start(string defName)
		{
			if (MyDebug.DeepScorePriorities)
			{
				var selDefName = MyDebug.SelectedApparel?.def?.defName;
				if (string.IsNullOrEmpty(selDefName) || string.IsNullOrEmpty(defName)) return;

				if (_started)
					_overlap = true;

				if (!_started && defName == selDefName)
				{
					_showString.Append($"[DebugDeepScorePriorities] Started.\n");
					_started = true;
				}
			}
		}

		public static void Clear()
		{
			_started = false;
			_overlap = false;
			_showString.Clear();
		}
	}
#endif
}
