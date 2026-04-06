SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Printer_History](
	[Last_Update] [varchar](50) NOT NULL,
	[Barcode_No] [varchar](50) NULL,
	[IP_Printer] [varchar](50) NULL,
	[StatusText] [varchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
