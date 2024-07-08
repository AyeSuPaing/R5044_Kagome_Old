if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_CsMessageMail]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_CsMessageMail]
GO
/*
=========================================================================================================
  Module      : ���b�Z�[�W���[��(w2_CsMessageMail.sql)
 �������������������������������������������������������������������������������������������������������
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_CsMessageMail] (
	[dept_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[mail_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[mail_kbn] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[mail_from] [nvarchar] (512) NOT NULL DEFAULT (N''),
	[mail_to] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[mail_cc] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[mail_bcc] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[mail_subject] [nvarchar] (100) NOT NULL DEFAULT (N''),
	[mail_body] [nvarchar] (max) NOT NULL DEFAULT (N''),
	[send_operator_id] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[send_datetime] [datetime],
	[receive_datetime] [datetime],
	[message_id] [varchar] (900) NOT NULL DEFAULT (N''),
	[in_reply_to] [varchar] (900) NOT NULL DEFAULT (N''),
	[del_flg] [nvarchar] (1) NOT NULL DEFAULT (N'0'),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_CsMessageMail] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_CsMessageMail] PRIMARY KEY  CLUSTERED
	(
		[dept_id],
		[mail_id]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_CsMessageMail_1] ON [dbo].[w2_CsMessageMail]([message_id]) ON [PRIMARY]
GO

/*
-- w2_CsMessageMail
DECLARE @TABLE_NAME NVARCHAR(256)
DECLARE @COLUMN_NAME NVARCHAR(256)
DECLARE @TABLE_ID INTEGER 
DECLARE @COLUMN_ID INTEGER 
DECLARE @CONSTRAINT_NAME NVARCHAR(256)
SET @TABLE_NAME = 'w2_CsMessageMail'
SET @COLUMN_NAME = 'message_id'

--�폜�������e�[�u���̃V�X�e��id���擾����
SELECT @TABLE_ID = id FROM sys.sysobjects 
WHERE xtype = 'U' AND name = @TABLE_NAME

--�폜�������J�����̃V�X�e��id���擾����
SELECT @COLUMN_ID = column_id FROM sys.columns 
WHERE object_id = @TABLE_ID AND name = @COLUMN_NAME

--�폜���������񖼂��擾����
SELECT @CONSTRAINT_NAME = name FROM sys.sysobjects 
WHERE id = (SELECT constid FROM sys.sysconstraints WHERE id = @TABLE_ID AND colid = @COLUMN_ID)

--�m�F => �e�[�u��->����->����->������X�N���v�g���ł����Ă��邩�m�F�ł��܂��B
SELECT @CONSTRAINT_NAME

--������폜����
EXEC('ALTER TABLE '+ @TABLE_NAME + ' DROP CONSTRAINT ' + @CONSTRAINT_NAME)
--�C���f�b�N�X�폜
EXEC('DROP INDEX '+ @TABLE_NAME + '.[IX_w2_CsMessageMail_1]')

ALTER TABLE [w2_CsMessageMail] ALTER COLUMN [message_id] [varchar] (900) NOT NULL;
ALTER TABLE [w2_CsMessageMail] ADD DEFAULT (N'') FOR [message_id];

CREATE INDEX [IX_w2_CsMessageMail_1] ON [dbo].[w2_CsMessageMail]([message_id]) ON [PRIMARY];
GO
*/

/*
-- w2_CsMessageMail
DECLARE @TABLE_NAME NVARCHAR(256)
DECLARE @COLUMN_NAME NVARCHAR(256)
DECLARE @TABLE_ID INTEGER 
DECLARE @COLUMN_ID INTEGER 
DECLARE @CONSTRAINT_NAME NVARCHAR(256)
SET @TABLE_NAME = 'w2_CsMessageMail'
SET @COLUMN_NAME = 'in_reply_to'

--�폜�������e�[�u���̃V�X�e��id���擾����
SELECT @TABLE_ID = id FROM sys.sysobjects 
WHERE xtype = 'U' AND name = @TABLE_NAME

--�폜�������J�����̃V�X�e��id���擾����
SELECT @COLUMN_ID = column_id FROM sys.columns 
WHERE object_id = @TABLE_ID AND name = @COLUMN_NAME

--�폜���������񖼂��擾����
SELECT @CONSTRAINT_NAME = name FROM sys.sysobjects 
WHERE id = (SELECT constid FROM sys.sysconstraints WHERE id = @TABLE_ID AND colid = @COLUMN_ID)

--�m�F => �e�[�u��->����->����->������X�N���v�g���ł����Ă��邩�m�F�ł��܂��B
SELECT @CONSTRAINT_NAME

--������폜����
EXEC('ALTER TABLE '+ @TABLE_NAME + ' DROP CONSTRAINT ' + @CONSTRAINT_NAME)

ALTER TABLE [w2_CsMessageMail] ALTER COLUMN [in_reply_to] [varchar] (900) NOT NULL;
ALTER TABLE [w2_CsMessageMail] ADD DEFAULT (N'') FOR [in_reply_to];
*/