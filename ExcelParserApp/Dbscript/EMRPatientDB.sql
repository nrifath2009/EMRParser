
USE [EMRPatientDB]
GO
/****** Object:  Table [dbo].[PatientSurveyDetail]    Script Date: 6/24/2019 2:08:44 PM ******/
CREATE TABLE [dbo].[PatientSurveyDetail](
	[PatientSurveyDetailId] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[PatientName] [nvarchar](150) NULL,
	[MRN] [nvarchar](50) NULL,
	[PatientAge] [int] NULL,
	[ProfessionalName] [nvarchar](150) NULL,
	[SurveyDate] [date] NULL,
	[SurveyQuestionText] [nvarchar](350) NULL,
	[SurveyQuestionAnswerText] [nvarchar](150) NULL,
	[CreatedOn] [datetime] NULL,
 CONSTRAINT [PK_PatientSurveyDetail] PRIMARY KEY CLUSTERED 
(
	[PatientSurveyDetailId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PatientSurveyDetail] ADD  CONSTRAINT [DF_PatientSurveyDetail_PatientSurveyDetailId]  DEFAULT (newid()) FOR [PatientSurveyDetailId]
GO

