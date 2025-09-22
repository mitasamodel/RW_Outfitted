using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outfitted.RW_JustUtils;
using System.Security.Policy;

namespace Outfitted
{
#if DEBUG
	internal class DebugDeepScorePriorities
	{
		private bool _started = false;
		private bool _overlap = false;
		private readonly StringBuilder _showString = new StringBuilder();
		public string Name { get; set; } = "DebugDeepScore";
		public bool Enabled { get; set; } = false;

		internal DebugDeepScorePriorities()	{ }

		public void AddToLog(string str)
		{
			if (Enabled && !_overlap && _started)
			{
				_showString.Append(str);
			}
		}

		public void ShowLog()
		{
			if (Enabled)
			{
				if (_started)
					_showString.Append($"[{Name}] Finished.\n");
				Logger.Log(_showString.ToString());
				Clear();
			}
		}

		public void Start(string defName)
		{
			if (Enabled)
			{
				var selDefName = MyDebug.SelectedApparel?.def?.defName;
				if (string.IsNullOrEmpty(selDefName) || string.IsNullOrEmpty(defName)) return;

				if (_started)
					_overlap = true;

				if (!_started && defName == selDefName)
				{
					_showString.Append($"[{Name}] Started.\n");
					_started = true;
				}
			}
		}

		public void Clear()
		{
			_started = false;
			_overlap = false;
			_showString.Clear();
		}
	}
#endif
}
