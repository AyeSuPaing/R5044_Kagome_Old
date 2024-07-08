/*
=========================================================================================================
  Module      : 自動翻訳API 長期間利用されていないワードを削除(DeleteMailSendLogCommand.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Global.Config;
using w2.Common.Logger;
using w2.Domain.AutoTranslationWord;

namespace w2.Commerce.Batch.DeleteData.Commands
{
	/// <summary>
	/// 自動翻訳API 長期間利用されていないワードを削除
	/// </summary>
	public class DeleteAutoTranslationOldWord
	{
		/// <summary>
		/// 実行
		/// </summary>
		public void Exec()
		{
			if (GlobalConfigUtil.GlobalTranslationEnabled() == false) return;

			try
			{
				var deleteCount = new AutoTranslationWordService()
					.OldWordDelete(DateTime.Now.AddDays(-1 * GlobalConfigs.GetInstance().GlobalSettings.Translation.TranslationDeletingIntervalDay));
				Console.WriteLine(string.Format("全{0}件を削除しました。", deleteCount.ToString()));
				FileLogger.WriteInfo(string.Format("自動翻訳ワードの利用されていない古いデータを削除:全{0}件を削除しました。", deleteCount.ToString()));
			}
			catch (Exception ex)
			{
				Console.WriteLine("削除に失敗しました。", ex);
				FileLogger.WriteError("自動翻訳ワードの利用されていない古いデータを削除:削除に失敗しました。", ex);
			}
		}
	}
}
