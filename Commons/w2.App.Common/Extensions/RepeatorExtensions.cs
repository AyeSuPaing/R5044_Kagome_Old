/*
=========================================================================================================
  Module      : リピーターアイテム拡張メソッドクラス(RepeatorExtensions.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Web.UI.WebControls;
using w2.Domain;

namespace w2.App.Common.Extensions
{
	/// <summary> リピータアイテム拡張メソッド </summary>
	public static class RepeatorExtensions
	{
		/// <summary>
		/// 指定モデルに変換
		/// </summary>
		/// <typeparam name="T">変換するモデル型</typeparam>
		/// <param name="self">self</param>
		/// <returns>
		/// 変換したモデル
		/// 変換できない場合はエラーがでる
		/// </returns>
		public static T ToModel<T>(this RepeaterItem self)
			where T : class,IModel
		{
			var model = self.DataItem as T;
			if (model == null) throw new Exception("DataItemは指定の型に変換できませんでした。");
			return model;
		}
	}
}
