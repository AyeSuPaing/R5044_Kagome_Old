if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_CheckAccessLogStatusFnc]') and OBJECTPROPERTY(id, N'IsScalarFunction') = 1)
drop function [dbo].[w2_CheckAccessLogStatusFnc]
GO
/*
=========================================================================================================
  Module      : �A�N�Z�X���O�����X�e�[�^�X�`�F�b�N�֐�(w2_CheckAccessLogStatusFnc.sql)
 �������������������������������������������������������������������������������������������������������
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE FUNCTION w2_CheckAccessLogStatusFnc(
	@tgt_date [nvarchar] (10),			-- �Ώ�"2006/01/01"
	@file_num [int],					-- �捞�t�@�C���ԍ�(�����捞�̂�)
	@day_or_month [int],				-- 1:����  2:�������܂������̂�
	@req_status [int]					-- �v�������X�e�[�^�X(01�F�捞�����A11�F���H����)
	)
RETURNS int	-- 0:�捞�\�A -1�ȉ�:�捞�s��
BEGIN
	DECLARE	@st_target_date nvarchar(10)
	DECLARE	@st_day_status nvarchar(2)
	DECLARE	@st_import_files int
	DECLARE	@st_import_files_total int
	DECLARE	@st_month_status nvarchar(2)

	-- ���݂̃X�e�[�^�X�擾
	SELECT	TOP 1
			@st_target_date = target_date,
			@st_day_status = day_status,
			@st_month_status = month_status,
			@st_import_files = import_files,
			@st_import_files_total = import_files_total
	  FROM	w2_AccessLogStatus

	------------------------------------------
	-- �X�e�[�^�X���R�[�h�Ȃ��̏ꍇ-
	------------------------------------------
	IF @st_target_date IS NULL
	BEGIN
		-- ���H�v���ł����NG
		IF @req_status <> '01'
		BEGIN
			RETURN -1
		END
	END
	------------------------------------------
	-- �O�̓��ȑO�ł����NG
	------------------------------------------
	ELSE IF CONVERT(datetime, @tgt_date) < CONVERT(datetime, @st_target_date)
	BEGIN
		RETURN -10
	END
	------------------------------------------
	-- �������̏ꍇ
	------------------------------------------
	ELSE IF CONVERT(datetime, @tgt_date) = CONVERT(datetime, @st_target_date)
	BEGIN
		-- �捞�����v��
		IF @req_status = '01'
		BEGIN
			-- �捞���A���H�҂��A���H�������A���H�����͂m�f
			IF @st_day_status IN ('01','10','11','20')
			BEGIN
				RETURN -20
			END
			
			-- �ꕔ��荞�݂̓`�F�b�N�i������Ԃ͒ʉ߁j
			IF @st_day_status <> '00'
			BEGIN
				-- �A�������捞�łȂ��ꍇ�A�K�萔�ȏ�捞�̏ꍇ��NG
				IF ((@st_import_files +1 <> @file_num) OR (@file_num > @st_import_files_total))
				BEGIN
					RETURN -21
				END
			END
		END
		-- ���H�����v��
		ELSE IF @req_status = '11'
		BEGIN
			-- ���H�҂��ȊO�͂m�f
			IF NOT @st_day_status = '10'
			BEGIN
				RETURN -22
			END
		END
	END
	------------------------------------------
	-- ���̓��ȍ~�ꍇ
	------------------------------------------
	ELSE IF CONVERT(datetime, @tgt_date) > CONVERT(datetime, @st_target_date)
	BEGIN
		-- �捞�����v��
		IF @req_status = '01'
		BEGIN
			-- �O�� ���� �O�����H�����ȊO��NG
			IF NOT ((CONVERT(datetime, @tgt_date) = DATEADD(dd,1,CONVERT(datetime, @st_target_date)))  AND (@st_day_status = '20'))
			BEGIN
				RETURN -30
			END
		END
		ELSE
		-- �捞�����v���ȊO��NG
		BEGIN
			RETURN -31
		END
	END

	-- OK
	RETURN 0
END

