/*
=========================================================================================================
  Module      : ���b�Z�[�W�}�l�[�W���[(MessageManager.cs)
 �������������������������������������������������������������������������������������������������������
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;

namespace w2.Common.Util
{
	///**************************************************************************************
	/// <summary>
	/// ��ʂɏo�͂����{���b�Z�[�W���Ǘ�����
	/// </summary>
	/// <remarks>
	/// GetMessages�ŃG���[���b�Z�[�W�擾�B
	/// GetMessages���Ăяo�����Ƃ�XML�t�@�C���̍X�V���t���`�F�b�N���A
	/// �ύX������΍ēǂݍ��݂��s���B
	/// </remarks>
	///**************************************************************************************
	public class MessageManager
	{
		//=========================================================================================
		// ���ʌn
		//=========================================================================================
		//------------------------------------------------------
		// �V�X�e���G���[
		//------------------------------------------------------
		/// <summary>�G���[���b�Z�[�W�萔�F�V�X�e���G���[</summary>
		public const string ERRMSG_SYSTEM_ERROR = "ERRMSG_SYSTEM_ERROR";
		/// <summary>�G���[���b�Z�[�W�萔�F404�G���[</summary>
		public const string ERRMSG_404_ERROR = "ERRMSG_404_ERROR";

		//------------------------------------------------------
		// �V�X�e�����̓G���[(�s���������͂Ȃ�) 
		//------------------------------------------------------
		/// <summary>�G���[���b�Z�[�W�萔�F�s���������̓G���[</summary>
		public const string ERRMSG_SYSTEM_VALIDATION_ERROR = "ERRMSG_SYSTEM_VALIDATION_ERROR";
		/// <summary>�O�������NURL�`���G���[/// </summary>
		public const string ERRMSG_MANAGER_URL_FORMAT_ERROR = "ERRMSG_MANAGER_URL_FORMAT_ERROR";
		//------------------------------------------------------
		// ���̓`�F�b�N
		//------------------------------------------------------
		/// <summary>�G���[���b�Z�[�W�萔�F�K�{�`�F�b�N�G���[</summary>
		public const string INPUTCHECK_NECESSARY = "INPUTCHECK_NECESSARY";
		/// <summary>�G���[���b�Z�[�W�萔�F�������`�F�b�N�G���[</summary>
		public const string INPUTCHECK_LENGTH = "INPUTCHECK_LENGTH";
		/// <summary>�G���[���b�Z�[�W�萔�F�ő啶�����`�F�b�N�G���[</summary>
		public const string INPUTCHECK_LENGTH_MAX = "INPUTCHECK_LENGTH_MAX";
		/// <summary>�G���[���b�Z�[�W�萔�F�ŏ��������`�F�b�N�G���[</summary>
		public const string INPUTCHECK_LENGTH_MIN = "INPUTCHECK_LENGTH_MIN";
		/// <summary>�G���[���b�Z�[�W�萔�F�o�C�g���`�F�b�N�G���[</summary>
		public const string INPUTCHECK_BYTE_LENGTH = "INPUTCHECK_BYTE_LENGTH";
		/// <summary>�G���[���b�Z�[�W�萔�F�ő�o�C�g���`�F�b�N�G���[</summary>
		public const string INPUTCHECK_BYTE_LENGTH_MAX = "INPUTCHECK_BYTE_LENGTH_MAX";
		/// <summary>�G���[���b�Z�[�W�萔�F�ŏ��o�C�g���`�F�b�N�G���[</summary>
		public const string INPUTCHECK_BYTE_LENGTH_MIN = "INPUTCHECK_BYTE_LENGTH_MIN";
		/// <summary>�G���[���b�Z�[�W�萔�F�ő吔�l�`�F�b�N�G���[</summary>
		public const string INPUTCHECK_NUMBER_MAX = "INPUTCHECK_NUMBER_MAX";
		/// <summary>�G���[���b�Z�[�W�萔�F�ŏ����l�`�F�b�N�G���[</summary>
		public const string INPUTCHECK_NUMBER_MIN = "INPUTCHECK_NUMBER_MIN";
		/// <summary>�G���[���b�Z�[�W�萔�F�S�p�`�F�b�N�G���[</summary>
		public const string INPUTCHECK_FULLWIDTH = "INPUTCHECK_FULLWIDTH";
		/// <summary>�G���[���b�Z�[�W�萔�F�S�p�Ђ炪�ȃ`�F�b�N�G���[</summary>
		public const string INPUTCHECK_FULLWIDTH_HIRAGANA = "INPUTCHECK_FULLWIDTH_HIRAGANA";
		/// <summary>�G���[���b�Z�[�W�萔�F�S�p�J�^�J�i�`�F�b�N�G���[</summary>
		public const string INPUTCHECK_FULLWIDTH_KATAKANA = "INPUTCHECK_FULLWIDTH_KATAKANA";
		/// <summary>�G���[���b�Z�[�W�萔�F���p�`�F�b�N�G���[</summary>
		public const string INPUTCHECK_HALFWIDTH = "INPUTCHECK_HALFWIDTH";
		/// <summary>�G���[���b�Z�[�W�萔�F���p�p���`�F�b�N�G���[</summary>
		public const string INPUTCHECK_HALFWIDTH_ALPHNUM = "INPUTCHECK_HALFWIDTH_ALPHNUM";
		/// <summary>�G���[���b�Z�[�W�萔�F���p�p���L���`�F�b�N�G���[</summary>
		public const string INPUTCHECK_HALFWIDTH_ALPHNUMSYMBOL = "INPUTCHECK_HALFWIDTH_ALPHNUMSYMBOL";
		/// <summary>�G���[���b�Z�[�W�萔�F���p���l�`�F�b�N�G���[</summary>
		public const string INPUTCHECK_HALFWIDTH_NUMBER = "INPUTCHECK_HALFWIDTH_NUMBER";
		/// <summary>�G���[���b�Z�[�W�萔�F���p�����`�F�b�N�G���[</summary>
		public const string INPUTCHECK_HALFWIDTH_DECIMAL = "INPUTCHECK_HALFWIDTH_DECIMAL";
		/// <summary>�G���[���b�Z�[�W�萔�F���p���t�`�F�b�N�G���[</summary>
		public const string INPUTCHECK_HALFWIDTH_DATE = "INPUTCHECK_HALFWIDTH_DATE";
		/// <summary>�G���[���b�Z�[�W�萔�F���p���l�`�F�b�N�G���[</summary>
		public const string INPUTCHECK_HALFWIDTH_NUMERIC = "INPUTCHECK_HALFWIDTH_NUMERIC";
		/// <summary>�G���[���b�Z�[�W�萔�F���t�`�F�b�N�G���[</summary>
		public const string INPUTCHECK_DATE = "INPUTCHECK_DATE";
		/// <summary>�G���[���b�Z�[�W�萔�F�������t�`�F�b�N�G���[</summary>
		public const string INPUTCHECK_DATE_FUTURE = "INPUTCHECK_DATE_FUTURE";
		/// <summary>�G���[���b�Z�[�W�萔�F�ߋ����t�`�F�b�N�G���[</summary>
		public const string INPUTCHECK_DATE_PAST = "INPUTCHECK_DATE_PAST";
		/// <summary>�G���[���b�Z�[�W�萔�F���[���A�h���X�`�F�b�N�G���[</summary>
		public const string INPUTCHECK_MAILADDRESS = "INPUTCHECK_MAILADDRESS";
		/// <summary>�G���[���b�Z�[�W�萔�F�s���{���`�F�b�N�G���[</summary>
		public const string INPUTCHECK_PREFECTURE = "INPUTCHECK_PREFECTURE";
		/// <summary>�G���[���b�Z�[�W�萔�F���i�ŗ��J�e�S���`�F�b�N�G���[</summary>
		public const string INPUTCHECK_TAX_RATE = "INPUTCHECK_TAX_RATE";
		/// <summary>�G���[���b�Z�[�W�萔�F�֎~�����`�F�b�N�G���[</summary>
		public const string INPUTCHECK_PROHIBITED_CHAR = "INPUTCHECK_PROHIBITED_CHAR";
		/// <summary>�G���[���b�Z�[�W�萔�F�����R�[�h�O�`�F�b�N�G���[</summary>
		public const string INPUTCHECK_OUTOFCHARCODE = "INPUTCHECK_OUTOFCHARCODE";
		/// <summary>�G���[���b�Z�[�W�萔�F���K�\���`�F�b�N�G���[</summary>
		public const string INPUTCHECK_REGEXP = "INPUTCHECK_REGEXP";
		/// <summary>�G���[���b�Z�[�W�萔�F���K�\��2�`�F�b�N�G���[</summary>
		public const string INPUTCHECK_REGEXP2 = "INPUTCHECK_REGEXP2";
		/// <summary>�G���[���b�Z�[�W�萔�F���K�\���i���O�j�`�F�b�N�G���[</summary>
		public const string INPUTCHECK_EXCEPT_REGEXP = "INPUTCHECK_EXCEPT_REGEXP";
		/// <summary>�G���[���b�Z�[�W�萔�F�m�F���̓`�F�b�N�G���[</summary>
		public const string INPUTCHECK_CONFIRM = "INPUTCHECK_CONFIRM";
		/// <summary>�G���[���b�Z�[�W�萔�F���l�`�F�b�N�G���[</summary>
		public const string INPUTCHECK_EQUIVALENCE = "INPUTCHECK_EQUIVALENCE";
		/// <summary>�G���[���b�Z�[�W�萔�F�ْl�`�F�b�N�G���[</summary>
		public const string INPUTCHECK_DIFFERENT_VALUE = "INPUTCHECK_DIFFERENT_VALUE";
		/// <summary>�G���[���b�Z�[�W�萔�F�d���`�F�b�N�G���[</summary>
		public const string INPUTCHECK_DUPLICATION = "INPUTCHECK_DUPLICATION";
		/// <summary>�G���[���b�Z�[�W�萔�F�d���`�F�b�N�G���[</summary>
		public const string INPUTCHECK_DUPLICATION_DATERANGE = "INPUTCHECK_DUPLICATION_DATERANGE";
		/// <summary>�G���[���b�Z�[�W�萔�F�J�n���t�͏I�����t��菬�����`�F�b�N�G���[</summary>
		public const string INPUTCHECK_DATERANGE = "INPUTCHECK_DATERANGE";
		/// <summary>�G���[���b�Z�[�W�萔�F�ʉ݃`�F�b�N�G���[</summary>
		public const string INPUTCHECK_CURRENCY = "INPUTCHECK_CURRENCY";
		/// <summary>�G���[���b�Z�[�W�萔�F���[�����M�֎~������`�F�b�N�G���[</summary>
		public const string INPUTCHECK_MAIL_TRANSMISSION_DISABLED_STRING = "INPUTCHECK_MAIL_TRANSMISSION_DISABLED_STRING";
		/// <summary>�G���[���b�Z�[�W�萔�F���ԃ`�F�b�N�G���[</summary>
		public const string INPUTCHECK_DROPDOWN_TIME = "INPUTCHECK_DROPDOWN_TIME";
		/// <summary>�G���[���b�Z�[�W�萔�F������z�`�F�b�N�G���[</summary>
		public const string INPUTCHECK_PRICE_MAX = "INPUTCHECK_PRICE_MAX";
		/// <summary>�I�v�V�������i�̃G���[���b�Z�[�W�萔�F���p���l�`�F�b�N�G���[</summary>
		public const string INPUTCHECK_HALFWIDTH_NUMBER_OPTION_PRICE = "INPUTCHECK_HALFWIDTH_NUMBER_OPTION_PRICE";
		/// <summary>�G���[���b�Z�[�W�萔�F�g�p�֎~�����G���[</summary>
		public const string INPUTCHECK_PROHIBITED_CHARACTERS = "INPUTCHECK_PROHIBITED_CHARACTERS";

		/// <summary>�G���[�t�@�C���ŏI�X�V��</summary>
		private static DateTime m_dtFileLastUpdate = new DateTime(0);

		/// <summary>�G���[���b�Z�[�W�i�[�f�B�N�V���i��</summary>
		private static Dictionary<string, string> m_dicMessages = new Dictionary<string, string>();

		/// <summary>ReaderWriterLockSlim�I�u�W�F�N�g</summary>
		private static System.Threading.ReaderWriterLockSlim m_lock = new System.Threading.ReaderWriterLockSlim();

		/// <summary>
		/// �G���[���b�Z�[�W�擾
		/// </summary>
		/// <param name="messageKey">���b�Z�[�W�L�[</param>
		/// <param name="replaces">�u���p�����[�^</param>
		/// <returns>�G���[���b�Z�[�W</returns>
		public static string GetMessages(string messageKey, params string[] replaces)
		{
			var message = MessageProvider.GetMessages(messageKey, replaces);
			if (string.IsNullOrEmpty(message)) message = string.Empty;
			return message;
		}

		/// <summary>
		/// ���b�Z�[�W�v���o�C�_
		/// </summary>
		public static IMessageProvider MessageProvider
		{
			get { return MessageManager.m_messageProvider; }
			set { MessageManager.m_messageProvider = value; }
		}
		/// <summary>���b�Z�[�W�v���o�C�_</summary>
		private static IMessageProvider m_messageProvider = new MessageProviderXml();
	}
}
