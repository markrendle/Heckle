SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Session](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EventCode] [nvarchar](50) NOT NULL,
	[Slot] [int] NOT NULL,
	[Track] [int] NOT NULL,
	[Title] [nvarchar](100) NOT NULL,
	[Presenter] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Session] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[Feedback](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SessionId] [int] NOT NULL,
	[Comment] [nvarchar](max) NOT NULL,
	[Mood] [nvarchar](50) NOT NULL,
	[Commenter] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Feedback] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Feedback]  WITH CHECK ADD  CONSTRAINT [FK_Feedback_Session] FOREIGN KEY([SessionId])
REFERENCES [dbo].[Session] ([Id])
GO

ALTER TABLE [dbo].[Feedback] CHECK CONSTRAINT [FK_Feedback_Session]
GO

ALTER TABLE [dbo].[Feedback] ADD  CONSTRAINT [DF_Feedback_Commenter]  DEFAULT ('Anonymous') FOR [Commenter]
GO

