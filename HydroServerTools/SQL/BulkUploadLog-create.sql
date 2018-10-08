USE [hiscentral_logging]
GO

IF OBJECT_ID(N'dbo.BulkUploadLog', N'U') IS NOT NULL
BEGIN
	DROP TABLE [dbo].[BulkUploadLog]
END

IF OBJECT_ID(N'dbo.BulkUploadError', N'U') IS NOT NULL
BEGIN
	DROP TABLE [dbo].[BulkUploadError]
END

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[BulkUploadLog](
	[ClusteredId] [int] identity NOT NULL,
	[SessionId] [nvarchar](50) NOT NULL,
	[IPAddress] [nvarchar](50) NOT NULL,
	[Domain] [nvarchar](255) NOT NULL,
	[NetworkApiKey] [nvarchar](36) NULL,
	[GoogleEmailAddress] [nvarchar](255) NULL,
	[StartDateTime] [datetimeoffset](7) NOT NULL,
	[EndDateTime] [datetimeoffset](7) NOT NULL,
	[MethodName] [nvarchar](255) NOT NULL,
	[Parameters] [nvarchar](max) NULL,
	[Returns] [nvarchar](max) NULL,
	[Message] [nvarchar](255) NOT NULL,
	[LogLevel] [nvarchar](20) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

CREATE CLUSTERED INDEX IX_Log 
    ON dbo.BulkUploadLog (ClusteredId);

GO

CREATE TABLE [dbo].[BulkUploadError](
	[ClusteredId] [int] identity NOT NULL,
	[SessionId] [nvarchar](50) NOT NULL,
	[IPAddress] [nvarchar](50) NOT NULL,
	[Domain] [nvarchar](255) NOT NULL,
	[NetworkApiKey] [nvarchar](36) NULL,
	[OccurrenceDateTime] [datetimeoffset](7) NOT NULL,
	[MethodName] [nvarchar](255) NOT NULL,
	[Parameters] [nvarchar](max) NULL,
	[ExceptionType] [nvarchar](50) NOT NULL,
	[ExceptionMessage] [nvarchar](255) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

CREATE CLUSTERED INDEX IX_Error 
    ON dbo.BulkUploadError (ClusteredId);

GO

