using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Outfitted.RW_JustUtils
{
	public static class Logger
	{
		private const string loggerFile = "Outfitted.log";
		private const string loggerModName = "Outfitted";
		private static bool _init = false;
		static readonly string logFile = @Environment.CurrentDirectory + @"\Mods\" + loggerFile;

		[ThreadStatic]
		private static int _tabLevel = 0;

		public static void Init()
		{
			if (!_init)
			{
				_init = true;
				File.WriteAllText(logFile, $"[{loggerModName}] Debug start\n");
			}
		}

		public static void LogNL(string msg = "")
		{
			if (!_init) Init();
			File.AppendAllText(logFile, GetTabs() + msg + "\n");
		}
		public static void AppendTab(this StringBuilder sb, string msg)
		{
			sb.Append(GetTabs() + msg);
		}

		// Log from the beginning no matter tabs.
		public static void LogNL(int tab, string msg)
		{
			if (!_init) Init();
			File.AppendAllText(logFile, msg + "\n");
		}

		public static void Log(string msg)
		{
			if (!_init) Init();
			File.AppendAllText(logFile, msg);
		}

		public static void Log_Warning(string str)
		{
			Verse.Log.Warning($"[{loggerModName}] " + str);
#if DEBUG
			LogNL(str);
#endif
		}

		public static void Log_Error(string str)
		{
			Verse.Log.Error($"[{loggerModName}] " + str);
#if DEBUG
			LogNL(str);
#endif
		}
		public static void Log_ErrorOnce(string str, int num)
		{
			Verse.Log.ErrorOnce($"[{loggerModName}] " + str, num);
#if DEBUG
			LogNL(str);
#endif
		}

		private static string GetTabs()
		{
			if (_tabLevel <= 0) return string.Empty;
			return new string('\t', _tabLevel);
		}

		public static void IncreaseTab() => _tabLevel++;

		public static void DecreaseTab() { if (_tabLevel > 0) _tabLevel--; }

		public static void ResetTab() => _tabLevel = 0;

		/// <summary>
		/// Increase the tab at the call.
		/// Automatically decrease the tab at local variable's termination (end of block/method).
		/// </summary>
		/// <returns></returns>
		public static IDisposable Scope()
		{
#if DEBUG
			IncreaseTab();
			return new IndentPopper();
#else
			return default;
#endif
		}

		/// <summary>
		/// Simple object, which implements IDisposable.
		/// </summary>
		private readonly struct IndentPopper : IDisposable
		{
			public void Dispose()
			{
				DecreaseTab();
			}
		}
	}
}
