/*
=========================================================================================================
  Module      : マクロキオスクのDN受信ファイルデータ(MacroKioskDNFileData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/

using System.IO;

namespace UpdateSMSState
{
	/// <summary>
	/// マクロキオスクのDN受信ファイルデータ
	/// </summary>
	public class MacroKioskDNFileData
	{
		#region +Load ロード
		/// <summary>
		/// ロード
		/// </summary>
		/// <param name="filePath">DNファイルのパス</param>
		/// <returns>ファイルデータインスタンス</returns>
		public static MacroKioskDNFileData Load(string filePath)
		{
			// ファイルがなければNULL

			if (File.Exists(filePath) == false)
			{
				return null;
			}

			// 正しいフォーマットでない場合はNULLを返す

			var line = "";

			using (var sr = new StreamReader(filePath))
			{
				line = sr.ReadToEnd();
			}

			if (string.IsNullOrEmpty(line))
			{
				return null;
			}

			var data = line.Split(',');
			if (data.Length != 3)
			{
				return null;
			}

			foreach (var d in data)
			{
				if (d.Split('：').Length != 2)
				{
					return null;
				}
			}

			// インスタンス生成
			var rtn = new MacroKioskDNFileData();

			foreach (var d in data)
			{
				var key = d.Split('：')[0].ToLower();
				var val = d.Split('：')[1].Replace("\r", "").Replace("\n", "");

				if (key == "msgid")
				{
					rtn.MsgId = val;
				}
				else if (key == "msisdn")
				{
					rtn.Msisdn = val;
				}
				else if (key == "status")
				{
					rtn.Status = val;
				}
			}
			return rtn;
		}
		#endregion

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MacroKioskDNFileData()
		{
			this.MsgId = "";
			this.Msisdn = "";
			this.Status = "";
		}

		/// <summary>メッセージID</summary>
		public string MsgId { get; set; }
		/// <summary>SMS送信先電話番号</summary>
		public string Msisdn { get; set; }
		/// <summary>ステータス</summary>
		public string Status { get; set; }
	}
}
