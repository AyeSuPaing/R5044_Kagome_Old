/*
=========================================================================================================
  Module      : スケジューラサービスとの通信(ScheduleService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Diagnostics;

///**************************************************************************************
/// <summary>
/// スケジューラサービスへのアクセスを提供します
/// </summary>
///**************************************************************************************
public static class ScheduleService
{
	/// <summary>
	/// スケジューラサービスの更新イベントを発行する
	/// </summary>
	public static void Kick()
	{
		ProcessStartInfo psInfo = new ProcessStartInfo();
		psInfo.FileName = Constants.PHYSICALDIRPATH_KICKUPDATE_SERVICE_EXE; // 実行するファイル
		psInfo.CreateNoWindow = true; // コンソール・ウィンドウを開かない
		psInfo.UseShellExecute = false; // シェル機能を使用しない

		// スケジューラサービスの更新イベントを発行する
		Process.Start(psInfo);
	}
}
