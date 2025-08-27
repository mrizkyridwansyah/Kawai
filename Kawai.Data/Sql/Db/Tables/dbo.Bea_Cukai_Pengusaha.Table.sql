SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Bea_Cukai_Pengusaha](
	[ID] [int] NOT NULL,
	[ALAMAT] [text] NULL,
	[CONTACT_PERSON] [varchar](255) NULL,
	[EMAIL] [varchar](255) NULL,
	[FAX] [varchar](20) NULL,
	[ID_PENGENAL] [varchar](255) NULL,
	[JENISTPB] [varchar](255) NULL,
	[KODE_ID] [varchar](255) NULL,
	[KODE_KANTOR] [varchar](255) NULL,
	[NAMA] [varchar](255) NULL,
	[NOMOR_PENGENAL] [varchar](255) NULL,
	[NOMOR_SKEP] [varchar](255) NULL,
	[NPWP] [varchar](255) NULL,
	[STATUS_IMPORTIR] [varchar](255) NULL,
	[TANGGAL_SKEP] [datetime] NULL,
	[TELEPON] [varchar](20) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
