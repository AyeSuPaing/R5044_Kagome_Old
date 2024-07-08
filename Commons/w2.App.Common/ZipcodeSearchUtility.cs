/*
=========================================================================================================
  Module      : 郵便番号検索クラスクラス(ZipcodeUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.Common.Sql;
using System.Collections;
using w2.Domain.Zipcode;

namespace w2.App.Common
{
	///*********************************************************************************************
	/// <summary>
	/// 郵便番号検索クラス
	/// </summary>
	///*********************************************************************************************
	public class ZipcodeSearchUtility
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ZipcodeSearchUtility(string strZipCode)
		{
			Match(strZipCode);
		}

		/// <summary>
		/// 住所検索
		/// </summary>
		/// <param name="zipcode">郵便番号</param>
		public void Match(string zipcode)
		{
			var addresses = new ZipcodeService().GetByZipcode(zipcode);

			if (addresses.Length == 1)
			{
				var address = addresses[0];
				this.PrefectureName = address.Prefecture;
				this.CityName = address.City;
				this.TownName = address.Town;
				this.PrefectureKana = address.PrefectureKana;
				this.CityKana = address.CityKana;
				this.TownKana = address.TownKana;
				this.Success = true;
			}
			else if (addresses.Length > 1)
			{
				var address = addresses[0];
				this.PrefectureName = address.Prefecture;
				this.CityName = address.City;
				this.TownName = "";	// 複数検索されたので町名は入力してもらう
				this.PrefectureKana = address.PrefectureKana;
				this.CityKana = address.CityKana;
				this.TownKana = "";	// 複数検索されたので町名は入力してもらう
				this.Success = true;
			}
			else
			{
				this.PrefectureName = "";
				this.CityName = "";
				this.TownName = "";
				this.Success = false;
			}
		}

		/// <summary>郵便番号検索結果</summary>
		public bool Success { get; private set; }
		/// <summary>都道府県名</summary>
		public string PrefectureName { get; private set; }
		/// <summary>市区町村名</summary>
		public string CityName { get; private set; }
		/// <summary>町域名設定</summary>
		public string TownName { get; private set; }
		/// <summary>都道府県カナ</summary>
		public string PrefectureKana { get; private set; }
		/// <summary>市区町村カナ</summary>
		public string CityKana { get; private set; }
		/// <summary>町域名カナ</summary>
		public string TownKana { get; private set; }
	}
}