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
	public class POIT : GameDataSection<POIT.POITEntry>
	{
		public POIT() { Signature = "POIT"; }
		public POIT(EndianBinaryReaderEx er)
		{
			Signature = er.ReadString(Encoding.ASCII, 4);
			if (Signature != "POIT") throw new SignatureNotCorrectException(Signature, "POIT", er.BaseStream.Position - 4);
			NrEntries = er.ReadUInt32();
			for (int i = 0; i < NrEntries; i++) Entries.Add(new POITEntry(er));
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
					"X", "Y", "Z",
					"Index",
					"?",
					"Duration",
					"?"
				};
		}

		public class POITEntry : GameDataSectionEntry
		{
			public POITEntry()
			{
				Position = new Vector3(0, 0, 0);
				Index = 0;
				Unknown1 = 0;
				Duration = 0;
				Unknown2 = 0;
			}
			public POITEntry(EndianBinaryReaderEx er)
			{
				er.ReadObject(this);
			}

			public override void Write(EndianBinaryWriter er)
			{
				er.WriteVecFx32(Position);
				er.Write(Index);
				er.Write(Unknown1);
				er.Write(Duration);
				er.Write(Unknown2);
			}

			public override ListViewItem GetListViewItem()
			{
				ListViewItem m = new ListViewItem("");
				m.SubItems.Add(Position.X.ToString("#####0.############"));
				m.SubItems.Add(Position.Y.ToString("#####0.############"));
				m.SubItems.Add(Position.Z.ToString("#####0.############"));

				m.SubItems.Add(Index.ToString());
				m.SubItems.Add(HexUtil.GetHexReverse(Unknown1));
				m.SubItems.Add(Duration.ToString());
				m.SubItems.Add(HexUtil.GetHexReverse(Unknown2));
				return m;
			}
			[Category("Transformation")]
			[BinaryFixedPoint(true, 19, 12)]
			public Vector3 Position { get; set; }
			[Category("Point")]
			public Byte Index { get; set; }
			[Category("Point")]
			public Byte Unknown1 { get; set; }
			[Category("Point")]
			public Int16 Duration { get; set; }
			[Category("Point")]
			[TypeConverter(typeof(HexTypeConverter)), HexReversedAttribute]
			public UInt32 Unknown2 { get; set; }
		}
	}
}
