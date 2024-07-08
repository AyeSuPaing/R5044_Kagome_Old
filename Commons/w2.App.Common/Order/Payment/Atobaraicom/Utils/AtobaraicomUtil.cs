/*
=========================================================================================================
  Module      : Atobaraicom Util (AtobaraicomUtil.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Text;
using System.Xml.Linq;

namespace w2.App.Common.Order
{
	/// <summary>
	/// Atobaraicom Util
	/// </summary>
	public class AtobaraicomUtil
	{
		/// <summary>
		/// メッセージを取得
		/// </summary>
		/// <param name="messages">メッセージ</param>
		/// <param name="elementName">要素名</param>
		/// <returns>メッセージを表示</returns>
		public static string GetMessages(XElement messages, string elementName)
		{
			var msgBuilder = new StringBuilder();
			foreach (var element in messages.Elements(elementName))
			{
				if (msgBuilder.Length > 0)
				{
					msgBuilder.Append(", ");
				}
				msgBuilder.Append(element.Attribute("cd").Value).Append(": ");
				msgBuilder.Append(element.Value);
			}
			return msgBuilder.ToString();
		}
	}
}
