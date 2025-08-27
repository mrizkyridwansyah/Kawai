SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [PO](
	[TanggalTOS] [datetime] NULL,
	[TanggalINT] [datetime] NULL,
	[Doku] [nvarchar](50) NULL,
	[Tanggal] [smalldatetime] NULL,
	[Revisi] [nvarchar](20) NULL,
	[Kode_Dept] [nvarchar](20) NULL,
	[Kode_Supplier] [nvarchar](20) NULL,
	[Subcon] [bit] NULL,
	[ContactPr] [nvarchar](100) NULL,
	[Syarat] [smallint] NULL,
	[Kode_Valas] [nvarchar](12) NULL,
	[Kurs] [float] NULL,
	[Diskon] [float] NULL,
	[bPPN] [bit] NULL,
	[PPN] [float] NULL,
	[PPH22] [float] NULL,
	[Keterangan] [nvarchar](255) NULL,
	[EntryDate] [nvarchar](25) NULL,
	[UserID] [nvarchar](25) NULL,
	[Hapus] [nvarchar](25) NULL,
	[NPWPSupplier] [nvarchar](50) NULL,
	[NamaSupplier] [nvarchar](255) NULL,
	[Alamat1Supplier] [nvarchar](255) NULL,
	[Alamat2Supplier] [nvarchar](255) NULL,
	[KotaSupplier] [nvarchar](255) NULL,
	[Status] [char](1) NULL,
	[DataReceivedCls] [char](1) NULL,
	[DataReceivedDate] [datetime] NULL,
	[PO_No] [nvarchar](50) NULL,
	[Last_Update] [datetime] NULL
) ON [PRIMARY]
GO
