using System;
using System.Collections.Generic;

namespace LinqCached
{

public static class LinqCache
{
	private static List<Action> framePassedActions = new List<Action>();
	private static List<Action> setAllDirtyActions = new List<Action>();
	private static List<Action> clearActions = new List<Action>();

	public static bool AutomaticMode { get; set; }

	public static void FramePassed()
	{
		int count = framePassedActions.Count;

		for( int i = 0; i < count; ++i )
		{
			framePassedActions[i]();
		}
	}

	public static void SetAllDirty()
	{
		int count = setAllDirtyActions.Count;

		for( int i = 0; i < count; ++i )
		{
			setAllDirtyActions[i]();
		}
	}

	public static void Clear()
	{
		int count = clearActions.Count;

		for( int i = 0; i < count; ++i )
		{
			clearActions[i]();
		}
	}

	internal static void AddFramePassedAction(Action framePassedAction)
	{
		framePassedActions.Add(framePassedAction);
	}

	internal static void AddSetAllDirtyAction(Action setAllDirtyAction)
	{
		setAllDirtyActions.Add(setAllDirtyAction);
	}

	internal static void AddClearAction(Action clearAction)
	{
		clearActions.Add(clearAction);
	}
}

}
