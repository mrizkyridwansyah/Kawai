SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Company_Bank](
	[Seq_No] [numeric](18, 0) IDENTITY(1,1) NOT NULL,
	[Company_Code] [char](5) NOT NULL,
	[Currency_Code] [char](2) NOT NULL,
	[Bank_Name] [char](50) NOT NULL,
	[Address1] [char](50) NULL,
	[Address2] [char](50) NULL,
	[City] [char](25) NULL,
	[Postal_Code] [char](10) NULL,
	[Account_No] [char](35) NOT NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL,
 CONSTRAINT [PK_Company_Bank] PRIMARY KEY CLUSTERED 
(
	[Seq_No] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [Company_Bank] ADD  CONSTRAINT [DF_Company_Bank_Register_Date]  DEFAULT (getdate()) FOR [Register_Date]
GO
ALTER TABLE [Company_Bank]  WITH NOCHECK ADD  CONSTRAINT [FK_Company_Bank_Company_Profile] FOREIGN KEY([Company_Code])
REFERENCES [Company_Profile] ([Company_Code])
GO
ALTER TABLE [Company_Bank] CHECK CONSTRAINT [FK_Company_Bank_Company_Profile]
GO
