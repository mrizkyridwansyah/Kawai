SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Company_Profile](
	[Company_Code] [char](5) NOT NULL,
	[Company_Name] [char](50) NULL,
	[Address1] [char](35) NULL,
	[Address2] [char](100) NULL,
	[Province] [char](20) NULL,
	[City] [char](20) NULL,
	[Postal_Code] [char](10) NULL,
	[Phone1] [char](15) NULL,
	[Phone2] [char](15) NULL,
	[Fax] [char](15) NULL,
	[SJ_Position] [char](25) NULL,
	[SJ_Person] [char](25) NULL,
	[DI_Position] [char](25) NULL,
	[DI_Person] [char](25) NULL,
	[Invoice_Position] [char](25) NULL,
	[Invoice_Person] [char](25) NULL,
	[PO_Position] [char](25) NULL,
	[PO_Person] [char](25) NULL,
	[Tax_Position] [char](25) NULL,
	[Tax_Person] [char](25) NULL,
	[Production_Position] [char](25) NULL,
	[Production_Person] [char](25) NULL,
	[QC_Position] [char](25) NULL,
	[QC_Person] [char](25) NULL,
	[PPC_Position] [char](25) NULL,
	[PPC_Person] [char](25) NULL,
	[Supply_Position] [char](25) NULL,
	[Supply_Person] [char](25) NULL,
	[Receipt_Position] [char](25) NULL,
	[Receipt_Person] [char](25) NULL,
	[President_Director] [char](25) NULL,
	[GLCode_Sales] [char](4) NULL,
	[FakturPajak_No] [char](25) NULL,
	[NPWP_No] [char](20) NULL,
	[NPWP_Name] [char](50) NULL,
	[NPWP_Address] [char](200) NULL,
	[NPWP_City] [char](30) NULL,
	[DO_SignCode] [char](15) NULL,
	[Invoice_SignCode] [char](15) NULL,
	[FakturPajak_SignCode] [char](15) NULL,
	[TglPengukuhan] [datetime] NULL,
	[ValuationPrice_BaseCurrency] [char](2) NULL,
	[ValuationPrice_ExchTerm] [char](1) NULL,
	[BC_Person1] [char](25) NULL,
	[BC_NIP1] [char](25) NULL,
	[BC_Person2] [char](25) NULL,
	[BC_NIP2] [char](25) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL,
	[Rate_Cls] [char](1) NULL,
	[No_KPPBC] [char](50) NULL,
	[No_Izin] [char](50) NULL,
	[NoDoc_BC40] [char](50) NULL,
 CONSTRAINT [PK_Company_Profile] PRIMARY KEY CLUSTERED 
(
	[Company_Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [Company_Profile] ADD  CONSTRAINT [DF_Company_Profile_Register_Date]  DEFAULT (getdate()) FOR [Register_Date]
GO
