/*
=========================================================================================================
  Module      : 翻訳タグ
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System.IO;
using System.Web.UI;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Translation;

namespace w2.App.Common.Web.WebCustomControl
{
	/// <summary>
	/// 翻訳タグ
	/// </summary>
	[ToolboxData("<{0}:Translation  runat=server></{0}:Translation>")]
	public class Translation : Control
	{
		/// <summary>中国語（簡体） 言語ロケールID</summary>
		private const string LANGUAGE_LOCALE_ZH_CN = "zh-CN";
		/// <summary>中国語（繁体） 言語ロケールID</summary>
		private const string LANGUAGE_LOCALE_ZH_TW = "zh-TW";

		/// <summary>
		/// レンダリング
		/// </summary>
		/// <param name="writer">HTMLライタ</param>
		protected override void Render(HtmlTextWriter writer)
		{
			using (var sw = new StringWriter())
			using (var htw = new HtmlTextWriter(sw))
			{
				// メモリ内にレスポンス出力
				base.Render(htw);
				var captured = sw.ToString();

				var translateText = new TranslationManager()
					.TranslationApi(captured, GetLanguageCode(), Constants.FLG_LASTCHANGED_USER);

				// 文字列化したレスポンスをブラウザに出力
				if (this.HtmlEncode)
				{
					writer.WriteEncodedText(translateText);
				}
				else
				{
					writer.Write(translateText);
				}
			}
		}

		/// <summary>
		/// 表示する言語コードの取得
		/// </summary>
		/// <returns>言語コード</returns>
		private string GetLanguageCode()
		{
			if (string.IsNullOrEmpty(this.Lang) == false) return this.Lang;

			var result = RegionManager.GetInstance().Region.LanguageCode;
			// https://cloud.google.com/translate/docs/basic/discovering-supported-languages?hl=ja
			// google 翻訳APIのサポート言語より中国語（簡体）,中国語（繁体）はBCP-47形式であるため変換
			// 言語ロケールIDがzh-CN,zh-TW(BCP-47)の場合にこちらを利用する
			switch (RegionManager.GetInstance().Region.LanguageLocaleId)
			{
				case LANGUAGE_LOCALE_ZH_TW:
					result = LANGUAGE_LOCALE_ZH_TW;
					break;

				case LANGUAGE_LOCALE_ZH_CN:
					result = LANGUAGE_LOCALE_ZH_CN;
					break;

				default:
					break;
			}
			return result;
		}

		/// <summary>翻訳言語（外部設定）</summary>
		public string Lang { private get; set; }
		/// <summary>HTMLエンコード（外部設定）</summary>
		public bool HtmlEncode { private get; set; }
	}
}
