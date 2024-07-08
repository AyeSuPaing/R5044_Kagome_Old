/*
=========================================================================================================
  Module      : Telephone (Tel.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Linq;
using w2.Common.Util;
using w2.Domain.User;

namespace w2.App.Common.User
{
	/// <summary>
	/// Telephone
	/// </summary>
	public class Tel
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public Tel()
		{
			this.Tel1 = string.Empty;
			this.Tel2 = string.Empty;
			this.Tel3 = string.Empty;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="inputTel">Input telephone</param>
		/// <param name="isSplitLessThanTen">10未満を分割するか</param>
		public Tel(string inputTel, bool isSplitLessThanTen = false)
			: this()
		{
			inputTel = ReplaceAHyphenInTelephone(inputTel);

			if (inputTel.Length < 10)
			{
				if (isSplitLessThanTen) SplitTelLessThanTen(inputTel);
				return;
			}

			SplitterTel(inputTel);
		}

		/// <summary>
		/// Replace a hyphen in telephone
		/// </summary>
		/// <param name="inputTel">Input telephone</param>
		/// <returns>Telephone without hyphens(if phone number has contain hyphen)</returns>
		private string ReplaceAHyphenInTelephone(string inputTel)
		{
			if (inputTel.Contains("-")
				|| inputTel.Contains("ｰ"))
			{
				var telInputs = inputTel.Contains("-")
					? inputTel.Split('-')
					: inputTel.Split('ｰ');
				if (telInputs.Length <= 2)
				{
					var inputTelNoHyphen = StringUtility.ReplaceDelimiter(inputTel);
					return inputTelNoHyphen;
				}
			}
			return inputTel;
		}

		/// <summary>
		/// Splitter telephone
		/// </summary>
		/// <param name="inputTel">Input telephone</param>
		private void SplitterTel(string inputTel)
		{
			// Phone number with 2 hyphens
			if (inputTel.Contains("-")
					|| inputTel.Contains("ｰ"))
			{
				var inputTels = inputTel.Contains("-")
					? inputTel.Split('-')
					: inputTel.Split('ｰ');
				this.Tel1 = inputTels[0];
				this.Tel2 = inputTels[1];
				this.Tel3 = inputTels[2];
				return;
			}

			// Find the prefix code that is most like telephone input
			var prefixCode = new KeyValuePair<string, int>();
			prefixCode = this.DialPrefixCodesJp
				.FindAll(item => inputTel.StartsWith(item.Key))
				.OrderByDescending(item => item.Key)
				.FirstOrDefault();

			// The prefix code is not the same as the telephone input
			var prefixKey = prefixCode.Key;
			if (string.IsNullOrEmpty(prefixKey) == false)
			{
				this.Tel1 = prefixKey;
				this.Tel2 = inputTel.Substring(prefixKey.Length, prefixCode.Value);
				this.Tel3 = inputTel.Substring(prefixKey.Length + prefixCode.Value);
				return;
			}

			// The default format for 10 digit telephone numbers is 3-3-4
			if (inputTel.Length < 11)
			{
				this.Tel1 = inputTel.Substring(0, 3);
				this.Tel2 = inputTel.Substring(3, 3);
				this.Tel3 = inputTel.Substring(6);
				return;
			}

			// The default format for 11 digit telephone numbers is 3-4-4
			this.Tel1 = inputTel.Substring(0, 3);
			this.Tel2 = inputTel.Substring(3, 4);
			this.Tel3 = inputTel.Substring(7);
		}

		/// <summary>
		/// 10桁未満の分割
		/// </summary>
		/// <param name="inputTel">電話番号</param>
		private void SplitTelLessThanTen(string inputTel)
		{
			// 最大3桁-4桁-その他の形で分割
			this.Tel1 = (inputTel.Length >= 3) ? inputTel.Substring(0, 3) : inputTel;
			this.Tel2 = (inputTel.Length >= 7) ? inputTel.Substring(3, 4) : (inputTel.Length > 3) ? inputTel.Substring(3) : string.Empty;
			this.Tel3 = (inputTel.Length >= 8) ? inputTel.Substring(7) : string.Empty;
		}

		/// <summary>Tel 1</summary>
		public string Tel1 { get; set; }
		/// <summary>Tel 2</summary>
		public string Tel2 { get; set; }
		/// <summary>Tel 3</summary>
		public string Tel3 { get; set; }
		/// <summary>Telephone number</summary>
		public string TelNo
		{
			get
			{
				return UserService.CreatePhoneNo(this.Tel1, this.Tel2, this.Tel3);
			}
		}
		/// <summary>Dialing prefix code format in Japan</summary>
		private List<KeyValuePair<string, int>> DialPrefixCodesJp
		{
			get
			{
				var dialPrefixCodes = new List<KeyValuePair<string, int>>
				{
					new KeyValuePair<string, int>("050", 4),
					new KeyValuePair<string, int>("070", 4),
					new KeyValuePair<string, int>("080", 4),
					new KeyValuePair<string, int>("090", 4),
					new KeyValuePair<string, int>("020", 3),
					new KeyValuePair<string, int>("0120", 3),
					new KeyValuePair<string, int>("0800", 3),
					new KeyValuePair<string, int>("0570", 3),
					new KeyValuePair<string, int>("0990", 3),
					new KeyValuePair<string, int>("011", 3),
					new KeyValuePair<string, int>("0123", 2),
					new KeyValuePair<string, int>("0124", 2),
					new KeyValuePair<string, int>("0125", 2),
					new KeyValuePair<string, int>("0126", 2),
					new KeyValuePair<string, int>("01267", 1),
					new KeyValuePair<string, int>("0133", 2),
					new KeyValuePair<string, int>("0134", 2),
					new KeyValuePair<string, int>("0135", 2),
					new KeyValuePair<string, int>("0136", 2),
					new KeyValuePair<string, int>("01372", 1),
					new KeyValuePair<string, int>("01374", 1),
					new KeyValuePair<string, int>("01377", 1),
					new KeyValuePair<string, int>("0137", 2),
					new KeyValuePair<string, int>("0138", 2),
					new KeyValuePair<string, int>("01392", 1),
					new KeyValuePair<string, int>("0139", 2),
					new KeyValuePair<string, int>("01397", 1),
					new KeyValuePair<string, int>("01398", 1),
					new KeyValuePair<string, int>("0142", 2),
					new KeyValuePair<string, int>("0143", 2),
					new KeyValuePair<string, int>("0144", 2),
					new KeyValuePair<string, int>("0145", 2),
					new KeyValuePair<string, int>("01456", 1),
					new KeyValuePair<string, int>("01457", 1),
					new KeyValuePair<string, int>("0146", 2),
					new KeyValuePair<string, int>("01466", 1),
					new KeyValuePair<string, int>("0152", 2),
					new KeyValuePair<string, int>("0153", 2),
					new KeyValuePair<string, int>("0154", 2),
					new KeyValuePair<string, int>("01547", 1),
					new KeyValuePair<string, int>("015", 3),
					new KeyValuePair<string, int>("0155", 2),
					new KeyValuePair<string, int>("01558", 1),
					new KeyValuePair<string, int>("01564", 1),
					new KeyValuePair<string, int>("0156", 2),
					new KeyValuePair<string, int>("0157", 2),
					new KeyValuePair<string, int>("0158", 2),
					new KeyValuePair<string, int>("01586", 1),
					new KeyValuePair<string, int>("01587", 1),
					new KeyValuePair<string, int>("0162", 2),
					new KeyValuePair<string, int>("01632", 1),
					new KeyValuePair<string, int>("01634", 1),
					new KeyValuePair<string, int>("01635", 1),
					new KeyValuePair<string, int>("0163", 2),
					new KeyValuePair<string, int>("0164", 2),
					new KeyValuePair<string, int>("01648", 1),
					new KeyValuePair<string, int>("0165", 2),
					new KeyValuePair<string, int>("01654", 1),
					new KeyValuePair<string, int>("01655", 1),
					new KeyValuePair<string, int>("01656", 1),
					new KeyValuePair<string, int>("01658", 1),
					new KeyValuePair<string, int>("0166", 2),
					new KeyValuePair<string, int>("0167", 2),
					new KeyValuePair<string, int>("0172", 2),
					new KeyValuePair<string, int>("0173", 2),
					new KeyValuePair<string, int>("0174", 2),
					new KeyValuePair<string, int>("0175", 2),
					new KeyValuePair<string, int>("0176", 2),
					new KeyValuePair<string, int>("017", 3),
					new KeyValuePair<string, int>("0178", 2),
					new KeyValuePair<string, int>("0179", 2),
					new KeyValuePair<string, int>("0182", 2),
					new KeyValuePair<string, int>("0183", 2),
					new KeyValuePair<string, int>("0184", 2),
					new KeyValuePair<string, int>("0185", 2),
					new KeyValuePair<string, int>("0186", 2),
					new KeyValuePair<string, int>("0187", 2),
					new KeyValuePair<string, int>("018", 3),
					new KeyValuePair<string, int>("0191", 2),
					new KeyValuePair<string, int>("0192", 2),
					new KeyValuePair<string, int>("0193", 2),
					new KeyValuePair<string, int>("0194", 2),
					new KeyValuePair<string, int>("0195", 2),
					new KeyValuePair<string, int>("019", 3),
					new KeyValuePair<string, int>("0197", 2),
					new KeyValuePair<string, int>("0198", 2),
					new KeyValuePair<string, int>("022", 3),
					new KeyValuePair<string, int>("0220", 2),
					new KeyValuePair<string, int>("0223", 2),
					new KeyValuePair<string, int>("0224", 2),
					new KeyValuePair<string, int>("0225", 2),
					new KeyValuePair<string, int>("0226", 2),
					new KeyValuePair<string, int>("0228", 2),
					new KeyValuePair<string, int>("0229", 2),
					new KeyValuePair<string, int>("0233", 2),
					new KeyValuePair<string, int>("0234", 2),
					new KeyValuePair<string, int>("0235", 2),
					new KeyValuePair<string, int>("023", 3),
					new KeyValuePair<string, int>("0237", 2),
					new KeyValuePair<string, int>("0238", 2),
					new KeyValuePair<string, int>("0240", 2),
					new KeyValuePair<string, int>("0241", 2),
					new KeyValuePair<string, int>("0242", 2),
					new KeyValuePair<string, int>("0243", 2),
					new KeyValuePair<string, int>("0244", 2),
					new KeyValuePair<string, int>("0246", 2),
					new KeyValuePair<string, int>("0247", 2),
					new KeyValuePair<string, int>("0248", 2),
					new KeyValuePair<string, int>("024", 3),
					new KeyValuePair<string, int>("025", 3),
					new KeyValuePair<string, int>("0250", 2),
					new KeyValuePair<string, int>("0254", 2),
					new KeyValuePair<string, int>("0255", 2),
					new KeyValuePair<string, int>("0256", 2),
					new KeyValuePair<string, int>("0257", 2),
					new KeyValuePair<string, int>("0258", 2),
					new KeyValuePair<string, int>("0259", 2),
					new KeyValuePair<string, int>("0260", 2),
					new KeyValuePair<string, int>("0261", 2),
					new KeyValuePair<string, int>("026", 3),
					new KeyValuePair<string, int>("0263", 2),
					new KeyValuePair<string, int>("0264", 2),
					new KeyValuePair<string, int>("0265", 2),
					new KeyValuePair<string, int>("0266", 2),
					new KeyValuePair<string, int>("0267", 2),
					new KeyValuePair<string, int>("0268", 2),
					new KeyValuePair<string, int>("0269", 2),
					new KeyValuePair<string, int>("0270", 2),
					new KeyValuePair<string, int>("027", 3),
					new KeyValuePair<string, int>("0274", 2),
					new KeyValuePair<string, int>("0276", 2),
					new KeyValuePair<string, int>("0277", 2),
					new KeyValuePair<string, int>("0278", 2),
					new KeyValuePair<string, int>("0279", 2),
					new KeyValuePair<string, int>("0280", 2),
					new KeyValuePair<string, int>("0282", 2),
					new KeyValuePair<string, int>("0283", 2),
					new KeyValuePair<string, int>("0284", 2),
					new KeyValuePair<string, int>("0285", 2),
					new KeyValuePair<string, int>("028", 3),
					new KeyValuePair<string, int>("0287", 2),
					new KeyValuePair<string, int>("0288", 2),
					new KeyValuePair<string, int>("0289", 2),
					new KeyValuePair<string, int>("0291", 2),
					new KeyValuePair<string, int>("0293", 2),
					new KeyValuePair<string, int>("0294", 2),
					new KeyValuePair<string, int>("0295", 2),
					new KeyValuePair<string, int>("0296", 2),
					new KeyValuePair<string, int>("0297", 2),
					new KeyValuePair<string, int>("029", 3),
					new KeyValuePair<string, int>("0299", 2),
					new KeyValuePair<string, int>("03", 4),
					new KeyValuePair<string, int>("0422", 2),
					new KeyValuePair<string, int>("042", 3),
					new KeyValuePair<string, int>("0428", 2),
					new KeyValuePair<string, int>("043", 3),
					new KeyValuePair<string, int>("0436", 2),
					new KeyValuePair<string, int>("0438", 2),
					new KeyValuePair<string, int>("0439", 2),
					new KeyValuePair<string, int>("044", 3),
					new KeyValuePair<string, int>("045", 3),
					new KeyValuePair<string, int>("0460", 2),
					new KeyValuePair<string, int>("0463", 2),
					new KeyValuePair<string, int>("0465", 2),
					new KeyValuePair<string, int>("0466", 2),
					new KeyValuePair<string, int>("0467", 2),
					new KeyValuePair<string, int>("046", 3),
					new KeyValuePair<string, int>("0470", 2),
					new KeyValuePair<string, int>("04", 4),
					new KeyValuePair<string, int>("047", 3),
					new KeyValuePair<string, int>("0475", 2),
					new KeyValuePair<string, int>("0476", 2),
					new KeyValuePair<string, int>("0478", 2),
					new KeyValuePair<string, int>("0479", 2),
					new KeyValuePair<string, int>("0480", 2),
					new KeyValuePair<string, int>("048", 3),
					new KeyValuePair<string, int>("049", 3),
					new KeyValuePair<string, int>("0493", 2),
					new KeyValuePair<string, int>("0494", 2),
					new KeyValuePair<string, int>("0495", 2),
					new KeyValuePair<string, int>("04992", 1),
					new KeyValuePair<string, int>("04994", 1),
					new KeyValuePair<string, int>("04996", 1),
					new KeyValuePair<string, int>("04998", 1),
					new KeyValuePair<string, int>("052", 3),
					new KeyValuePair<string, int>("053", 3),
					new KeyValuePair<string, int>("0531", 2),
					new KeyValuePair<string, int>("0532", 2),
					new KeyValuePair<string, int>("0533", 2),
					new KeyValuePair<string, int>("0536", 2),
					new KeyValuePair<string, int>("0537", 2),
					new KeyValuePair<string, int>("0538", 2),
					new KeyValuePair<string, int>("0539", 2),
					new KeyValuePair<string, int>("054", 3),
					new KeyValuePair<string, int>("0544", 2),
					new KeyValuePair<string, int>("0545", 2),
					new KeyValuePair<string, int>("0547", 2),
					new KeyValuePair<string, int>("0548", 2),
					new KeyValuePair<string, int>("0550", 2),
					new KeyValuePair<string, int>("0551", 2),
					new KeyValuePair<string, int>("0553", 2),
					new KeyValuePair<string, int>("0554", 2),
					new KeyValuePair<string, int>("0555", 2),
					new KeyValuePair<string, int>("0556", 2),
					new KeyValuePair<string, int>("0557", 2),
					new KeyValuePair<string, int>("0558", 2),
					new KeyValuePair<string, int>("055", 3),
					new KeyValuePair<string, int>("0561", 2),
					new KeyValuePair<string, int>("0562", 2),
					new KeyValuePair<string, int>("0563", 2),
					new KeyValuePair<string, int>("0564", 2),
					new KeyValuePair<string, int>("0565", 2),
					new KeyValuePair<string, int>("0566", 2),
					new KeyValuePair<string, int>("0567", 2),
					new KeyValuePair<string, int>("0568", 2),
					new KeyValuePair<string, int>("0569", 2),
					new KeyValuePair<string, int>("0572", 2),
					new KeyValuePair<string, int>("0573", 2),
					new KeyValuePair<string, int>("0574", 2),
					new KeyValuePair<string, int>("0575", 2),
					new KeyValuePair<string, int>("0576", 2),
					new KeyValuePair<string, int>("05769", 1),
					new KeyValuePair<string, int>("0577", 2),
					new KeyValuePair<string, int>("0578", 2),
					new KeyValuePair<string, int>("058", 3),
					new KeyValuePair<string, int>("0581", 2),
					new KeyValuePair<string, int>("0584", 2),
					new KeyValuePair<string, int>("0585", 2),
					new KeyValuePair<string, int>("0586", 2),
					new KeyValuePair<string, int>("0587", 2),
					new KeyValuePair<string, int>("059", 3),
					new KeyValuePair<string, int>("0594", 2),
					new KeyValuePair<string, int>("0595", 2),
					new KeyValuePair<string, int>("0596", 2),
					new KeyValuePair<string, int>("0597", 2),
					new KeyValuePair<string, int>("05979", 1),
					new KeyValuePair<string, int>("0598", 2),
					new KeyValuePair<string, int>("0599", 2),
					new KeyValuePair<string, int>("06", 4),
					new KeyValuePair<string, int>("0721", 2),
					new KeyValuePair<string, int>("0725", 2),
					new KeyValuePair<string, int>("072", 3),
					new KeyValuePair<string, int>("073", 3),
					new KeyValuePair<string, int>("0735", 2),
					new KeyValuePair<string, int>("0736", 2),
					new KeyValuePair<string, int>("0737", 2),
					new KeyValuePair<string, int>("0738", 2),
					new KeyValuePair<string, int>("0739", 2),
					new KeyValuePair<string, int>("0740", 2),
					new KeyValuePair<string, int>("0742", 2),
					new KeyValuePair<string, int>("0743", 2),
					new KeyValuePair<string, int>("0744", 2),
					new KeyValuePair<string, int>("0745", 2),
					new KeyValuePair<string, int>("0746", 2),
					new KeyValuePair<string, int>("07468", 1),
					new KeyValuePair<string, int>("0747", 2),
					new KeyValuePair<string, int>("0748", 2),
					new KeyValuePair<string, int>("0749", 2),
					new KeyValuePair<string, int>("075", 3),
					new KeyValuePair<string, int>("0761", 2),
					new KeyValuePair<string, int>("0763", 2),
					new KeyValuePair<string, int>("076", 3),
					new KeyValuePair<string, int>("0765", 2),
					new KeyValuePair<string, int>("0766", 2),
					new KeyValuePair<string, int>("0767", 2),
					new KeyValuePair<string, int>("0768", 2),
					new KeyValuePair<string, int>("0770", 2),
					new KeyValuePair<string, int>("0771", 2),
					new KeyValuePair<string, int>("0772", 2),
					new KeyValuePair<string, int>("0773", 2),
					new KeyValuePair<string, int>("0774", 2),
					new KeyValuePair<string, int>("077", 3),
					new KeyValuePair<string, int>("0776", 2),
					new KeyValuePair<string, int>("0778", 2),
					new KeyValuePair<string, int>("0779", 2),
					new KeyValuePair<string, int>("078", 3),
					new KeyValuePair<string, int>("0790", 2),
					new KeyValuePair<string, int>("0791", 2),
					new KeyValuePair<string, int>("0794", 2),
					new KeyValuePair<string, int>("0795", 2),
					new KeyValuePair<string, int>("0796", 2),
					new KeyValuePair<string, int>("079", 3),
					new KeyValuePair<string, int>("0797", 2),
					new KeyValuePair<string, int>("0798", 2),
					new KeyValuePair<string, int>("0799", 2),
					new KeyValuePair<string, int>("0820", 2),
					new KeyValuePair<string, int>("0823", 2),
					new KeyValuePair<string, int>("0824", 2),
					new KeyValuePair<string, int>("082", 3),
					new KeyValuePair<string, int>("0826", 2),
					new KeyValuePair<string, int>("0827", 2),
					new KeyValuePair<string, int>("0829", 2),
					new KeyValuePair<string, int>("083", 3),
					new KeyValuePair<string, int>("0833", 2),
					new KeyValuePair<string, int>("0834", 2),
					new KeyValuePair<string, int>("0835", 2),
					new KeyValuePair<string, int>("0836", 2),
					new KeyValuePair<string, int>("0837", 2),
					new KeyValuePair<string, int>("0838", 2),
					new KeyValuePair<string, int>("08387", 1),
					new KeyValuePair<string, int>("08388", 1),
					new KeyValuePair<string, int>("08396", 1),
					new KeyValuePair<string, int>("0845", 2),
					new KeyValuePair<string, int>("0846", 2),
					new KeyValuePair<string, int>("0847", 2),
					new KeyValuePair<string, int>("08477", 1),
					new KeyValuePair<string, int>("0848", 2),
					new KeyValuePair<string, int>("084", 3),
					new KeyValuePair<string, int>("08512", 1),
					new KeyValuePair<string, int>("08514", 1),
					new KeyValuePair<string, int>("0852", 2),
					new KeyValuePair<string, int>("0853", 2),
					new KeyValuePair<string, int>("0854", 2),
					new KeyValuePair<string, int>("0855", 2),
					new KeyValuePair<string, int>("0856", 2),
					new KeyValuePair<string, int>("0857", 2),
					new KeyValuePair<string, int>("0858", 2),
					new KeyValuePair<string, int>("0859", 2),
					new KeyValuePair<string, int>("0863", 2),
					new KeyValuePair<string, int>("0865", 2),
					new KeyValuePair<string, int>("0866", 2),
					new KeyValuePair<string, int>("0867", 2),
					new KeyValuePair<string, int>("0868", 2),
					new KeyValuePair<string, int>("0869", 2),
					new KeyValuePair<string, int>("086", 3),
					new KeyValuePair<string, int>("0875", 2),
					new KeyValuePair<string, int>("0877", 2),
					new KeyValuePair<string, int>("087", 3),
					new KeyValuePair<string, int>("0879", 2),
					new KeyValuePair<string, int>("0880", 2),
					new KeyValuePair<string, int>("0883", 2),
					new KeyValuePair<string, int>("0884", 2),
					new KeyValuePair<string, int>("0885", 2),
					new KeyValuePair<string, int>("088", 3),
					new KeyValuePair<string, int>("0887", 2),
					new KeyValuePair<string, int>("0889", 2),
					new KeyValuePair<string, int>("0892", 2),
					new KeyValuePair<string, int>("0893", 2),
					new KeyValuePair<string, int>("0894", 2),
					new KeyValuePair<string, int>("0895", 2),
					new KeyValuePair<string, int>("0896", 2),
					new KeyValuePair<string, int>("0897", 2),
					new KeyValuePair<string, int>("0898", 2),
					new KeyValuePair<string, int>("089", 3),
					new KeyValuePair<string, int>("092", 3),
					new KeyValuePair<string, int>("0920", 2),
					new KeyValuePair<string, int>("093", 3),
					new KeyValuePair<string, int>("0930", 2),
					new KeyValuePair<string, int>("0940", 2),
					new KeyValuePair<string, int>("0942", 2),
					new KeyValuePair<string, int>("0943", 2),
					new KeyValuePair<string, int>("0944", 2),
					new KeyValuePair<string, int>("0946", 2),
					new KeyValuePair<string, int>("0947", 2),
					new KeyValuePair<string, int>("0948", 2),
					new KeyValuePair<string, int>("0949", 2),
					new KeyValuePair<string, int>("09496", 1),
					new KeyValuePair<string, int>("0950", 2),
					new KeyValuePair<string, int>("0952", 2),
					new KeyValuePair<string, int>("0954", 2),
					new KeyValuePair<string, int>("0955", 2),
					new KeyValuePair<string, int>("0956", 2),
					new KeyValuePair<string, int>("0957", 2),
					new KeyValuePair<string, int>("095", 3),
					new KeyValuePair<string, int>("0959", 2),
					new KeyValuePair<string, int>("096", 3),
					new KeyValuePair<string, int>("0964", 2),
					new KeyValuePair<string, int>("0965", 2),
					new KeyValuePair<string, int>("0966", 2),
					new KeyValuePair<string, int>("0967", 2),
					new KeyValuePair<string, int>("0968", 2),
					new KeyValuePair<string, int>("0969", 2),
					new KeyValuePair<string, int>("0972", 2),
					new KeyValuePair<string, int>("0973", 2),
					new KeyValuePair<string, int>("0974", 2),
					new KeyValuePair<string, int>("097", 3),
					new KeyValuePair<string, int>("0977", 2),
					new KeyValuePair<string, int>("0978", 2),
					new KeyValuePair<string, int>("0979", 2),
					new KeyValuePair<string, int>("098", 3),
					new KeyValuePair<string, int>("0980", 2),
					new KeyValuePair<string, int>("09802", 1),
					new KeyValuePair<string, int>("0982", 2),
					new KeyValuePair<string, int>("0983", 2),
					new KeyValuePair<string, int>("0984", 2),
					new KeyValuePair<string, int>("0985", 2),
					new KeyValuePair<string, int>("0986", 2),
					new KeyValuePair<string, int>("0987", 2),
					new KeyValuePair<string, int>("09912", 1),
					new KeyValuePair<string, int>("09913", 1),
					new KeyValuePair<string, int>("0993", 2),
					new KeyValuePair<string, int>("099", 3),
					new KeyValuePair<string, int>("0994", 2),
					new KeyValuePair<string, int>("0995", 2),
					new KeyValuePair<string, int>("0996", 2),
					new KeyValuePair<string, int>("09969", 1),
					new KeyValuePair<string, int>("0997", 2),
				};
				return dialPrefixCodes;
			}
		}
	}
}
