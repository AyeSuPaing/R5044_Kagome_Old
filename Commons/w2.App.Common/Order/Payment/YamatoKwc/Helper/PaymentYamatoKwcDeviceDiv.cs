/*
=========================================================================================================
  Module      : ヤマトKWC クロネコWEBコレクト 端末区分(PaymentYamatoKwcDeviceDiv.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace w2.App.Common.Order.Payment.YamatoKwc.Helper
{
	/// <summary>
	/// ヤマト クロネコWEBコレクト 端末区分
	/// </summary>
	public enum PaymentYamatoKwcDeviceDiv
	{
		/// <summary>スマートフォン</summary>
		SmartPhone = 1,
		/// <summary>パソコン</summary>
		Pc = 2,
		/// <summary>携帯電話</summary>
		Mobile = 3,

	}
}
