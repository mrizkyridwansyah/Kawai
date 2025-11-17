SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Bank_Master](
	[Bank_Code] [char](5) NOT NULL,
	[Trade_Code] [char](15) NOT NULL,
	[Currency_Code] [char](2) NOT NULL,
	[Bank_Name] [char](35) NOT NULL,
	[Bank_Address] [char](70) NULL,
	[Bank_Account] [char](30) NOT NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL,
 CONSTRAINT [PK_Supplier_Bank] PRIMARY KEY CLUSTERED 
(
	[Bank_Code] ASC,
	[Trade_Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [Bank_Master] ADD  CONSTRAINT [DF_Bank_Master_Register_Date]  DEFAULT (getdate()) FOR [Register_Date]
GO
ALTER TABLE [Bank_Master]  WITH NOCHECK ADD  CONSTRAINT [FK_Bank_Master_Trade_Master] FOREIGN KEY([Trade_Code])
REFERENCES [Trade_Master] ([Trade_Code])
GO
ALTER TABLE [Bank_Master] CHECK CONSTRAINT [FK_Bank_Master_Trade_Master]
GO
