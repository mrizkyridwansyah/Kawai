SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Bea_Cukai40_Doc](
	[No_Pengajuan] [varchar](50) NOT NULL,
	[SuratJalan_No] [varchar](25) NOT NULL,
	[Pengajuan_Date] [datetime] NULL,
	[Interface_Cls] [char](1) NULL,
	[Supplier_Code] [char](6) NULL,
	[Receipt_Date] [datetime] NULL,
 CONSTRAINT [PK__Bea_Cuka__330A364339FAE6D7] PRIMARY KEY CLUSTERED 
(
	[No_Pengajuan] ASC,
	[SuratJalan_No] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
