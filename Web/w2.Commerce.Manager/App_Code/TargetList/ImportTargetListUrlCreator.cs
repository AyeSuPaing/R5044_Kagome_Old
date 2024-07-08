/*
=========================================================================================================
  Module      : ターゲットリスト登録URLクリエータ(ImportTargetListUrlCreator.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System.Text;
using System.Web;

/// <summary>
/// ターゲットリスト登録URLクリエータ
/// </summary>
public class ImportTargetListUrlCreator
{
	/// <summary>
	/// 作成
	/// </summary>
	/// <param name="windowKbn">ウィンドウ区分</param>
	/// <returns>URL</returns>
	public static string Create(string windowKbn = Constants.KBN_WINDOW_DEFAULT)
	{
		var urlBuilder = new StringBuilder();
		urlBuilder.Append(Constants.PATH_ROOT_EC).Append(Constants.PAGE_MANAGER_USER_TARGETLIST);
		urlBuilder.Append("?").Append(Constants.REQUEST_KEY_WINDOW_KBN).Append("=").Append(windowKbn);
		return urlBuilder.ToString();
	}
}