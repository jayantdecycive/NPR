/*
   Tuesday, November 5, 201311:21:53 AM
   User: res_admin
   Server: restier2staging.cuwhyqxxofly.us-east-1.rds.amazonaws.com
   Database: npr
   Application: 
*/

/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.ResEvents ADD
	IsPaid bit NOT NULL CONSTRAINT DF_ResEvents_IsPaid DEFAULT 0,
	TicketAmount decimal(18, 2) NULL
GO
ALTER TABLE dbo.ResEvents SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.ResEvents', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.ResEvents', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.ResEvents', 'Object', 'CONTROL') as Contr_Per 