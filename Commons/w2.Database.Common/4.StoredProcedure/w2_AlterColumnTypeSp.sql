if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_AlterColumnTypeSp]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[w2_AlterColumnTypeSp]
GO
/*
=========================================================================================================
  Module      : ГJГЙГАВ╠М^Х╧НX(w2_AlterColumnType.sql)
 еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
CREATE PROCEDURE [dbo].w2_AlterColumnTypeSp
	@TABLE_NAME [nvarchar] (256),
	@COLUMN_NAME [nvarchar] (256),
	@NEW_COLUMN_TYPE_AND_NULL [nvarchar] (256)
AS

	DECLARE @TABLE_ID INTEGER
	DECLARE @COLUMN_ID INTEGER 
	DECLARE @CONSTRAINT_NAME NVARCHAR(256)
	
	SELECT @TABLE_ID = id FROM sys.sysobjects WHERE xtype = 'U' AND name = @TABLE_NAME
	SELECT @COLUMN_ID = column_id FROM sys.columns WHERE object_id = @TABLE_ID AND name = @COLUMN_NAME
	SELECT @CONSTRAINT_NAME = name FROM sys.sysobjects WHERE id = (SELECT constid FROM sys.sysconstraints WHERE id = @TABLE_ID AND colid = @COLUMN_ID)
	EXEC('ALTER TABLE '+ @TABLE_NAME + ' DROP CONSTRAINT [' + @CONSTRAINT_NAME + ']')
	EXEC('ALTER TABLE '+ @TABLE_NAME + ' ALTER COLUMN ' + @COLUMN_NAME + ' ' + @NEW_COLUMN_TYPE_AND_NULL)
	EXEC('ALTER TABLE '+ @TABLE_NAME + ' ADD CONSTRAINT [' + @CONSTRAINT_NAME + '] DEFAULT N'''' FOR ' + @COLUMN_NAME)
GO