﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibEveryFileExplorer.GameData;
using System.IO;
using LibEveryFileExplorer.Files;
using LibEveryFileExplorer.Collections;
using System.Windows.Forms;
using LibEveryFileExplorer;
using System.ComponentModel;
using LibEveryFileExplorer.ComponentModel;
using LibEveryFileExplorer.IO;
using LibEveryFileExplorer.IO.Serialization;

namespace MarioKart.MKDS.NKM
{
	public class IPAT : GameDataSection<IPAT.IPATEntry>
	{
		public IPAT() { Signature = "IPAT"; }
		public IPAT(EndianBinaryReaderEx er)
		{
			Signature = er.ReadString(Encoding.ASCII, 4);
			if (Signature != "IPAT") throw new SignatureNotCorrectException(Signature, "IPAT", er.BaseStream.Position - 4);
			NrEntries = er.ReadUInt32();
			for (int i = 0; i < NrEntries; i++) Entries.Add(new IPATEntry(er));
		}

		public void Write(EndianBinaryWriter er)
		{
			er.Write(Signature, Encoding.ASCII, false);
			NrEntries = (uint)Entries.Count;
			er.Write(NrEntries);
			for (int i = 0; i < NrEntries; i++) Entries[i].Write(er);
		}

		public override String[] GetColumnNames()
		{
			return new String[] {
					"ID",
					"Start",
					"Length",
					"Next",
					"Next",
					"Next",
					"Previous",
					"Previous",
					"Previous",
					"Order"
				};
		}

		public class IPATEntry : GameDataSectionEntry
		{
			public IPATEntry()
			{
				GoesTo = new byte[] { 0xFF, 0xFF, 0xFF };
				ComesFrom = new byte[] { 0xFF, 0xFF, 0xFF };
			}
			public IPATEntry(EndianBinaryReaderEx er)
			{
				er.ReadObject(this);
			}

			public override void Write(EndianBinaryWriter er)
			{
				er.Write(StartIndex);
				er.Write(Length);
				er.Write(GoesTo, 0, 3);
				er.Write(ComesFrom, 0, 3);
				er.Write(SectionOrder);
			}

			public override ListViewItem GetListViewItem()
			{
				ListViewItem m = new ListViewItem("");
				m.SubItems.Add(StartIndex.ToString());
				m.SubItems.Add(Length.ToString());

				m.SubItems.Add(GoesTo[0].ToString());
				m.SubItems.Add(GoesTo[1].ToString());
				m.SubItems.Add(GoesTo[2].ToString());

				m.SubItems.Add(ComesFrom[0].ToString());
				m.SubItems.Add(ComesFrom[1].ToString());
				m.SubItems.Add(ComesFrom[2].ToString());

				m.SubItems.Add(SectionOrder.ToString());
				return m;
			}
			[Category("Item Path"), DisplayName("Start Index")]
			public Int16 StartIndex { get; set; }
			[Category("Item Path")]
			public Int16 Length { get; set; }
			[Category("Item Path"), DisplayName("Goes To")]
			[TypeConverter(typeof(PrettyArrayConverter))]
			[BinaryFixedSize(3)]
			public Byte[] GoesTo { get; private set; }//3
			[Category("Item Path"), DisplayName("Comes From")]
			[TypeConverter(typeof(PrettyArrayConverter))]
			[BinaryFixedSize(3)]
			public Byte[] ComesFrom { get; private set; }//3
			[Category("Item Path"), DisplayName("Order")]
			public Int16 SectionOrder { get; set; }
		}
	}
}
