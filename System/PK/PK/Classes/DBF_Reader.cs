using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PK.Classes
{
    class DBF_Reader : IDisposable
    {
        public System.Collections.ObjectModel.ReadOnlyCollection<Tuple<string, char, byte, byte, bool>>
            Fields
        {
            get { return _Fields.AsReadOnly(); }
        }

        public readonly byte Signature;
        public readonly DateTime ModificationDate;
        public readonly uint RowCount;
        public readonly ushort HeaderSize;
        public readonly ushort RowSize;
        public readonly bool DBASEIV_UnfinishedTransaction;
        public readonly bool DBASEIV_Crypto;
        public readonly bool IndexMDX;
        public readonly byte CodePageID;

        private readonly BinaryReader _Reader;
        private readonly List<Tuple<string, char, byte, byte, bool>> _Fields;

        private uint _CurrentRowIndex = 0;
        private object[] _CurrentRow;

        public DBF_Reader(string file)
        {
            _Reader = new BinaryReader(File.OpenRead(file)); //TODO Close

            Signature = _Reader.ReadByte();
            if (Signature != 3)
                throw new IOException("Unsupported DBF version.");

            ModificationDate = new DateTime(_Reader.ReadByte(), _Reader.ReadByte(), _Reader.ReadByte());
            RowCount = _Reader.ReadUInt32();
            HeaderSize = _Reader.ReadUInt16();
            RowSize = _Reader.ReadUInt16();
            _Reader.ReadUInt16();
            DBASEIV_UnfinishedTransaction = _Reader.ReadByte() != 0;
            DBASEIV_Crypto = _Reader.ReadByte() != 0;
            _Reader.ReadBytes(12);
            IndexMDX = _Reader.ReadByte() != 0;
            CodePageID = _Reader.ReadByte();
            _Reader.ReadUInt16();

            ushort fieldCount = (ushort)((HeaderSize - 33) / 32);

            _Fields = new List<Tuple<string, char, byte, byte, bool>>(fieldCount);
            for (ushort i = 0; i < fieldCount; ++i)
            {
                string name = Encoding.UTF8.GetString(_Reader.ReadBytes(11)).TrimEnd('\0');
                char type = Encoding.UTF8.GetChars(new byte[] { _Reader.ReadByte() })[0];
                _Reader.ReadUInt32();
                byte size = _Reader.ReadByte();
                byte digits = _Reader.ReadByte();
                _Reader.ReadBytes(13);
                bool mdx = _Reader.ReadByte() != 0;

                _Fields.Add(new Tuple<string, char, byte, byte, bool>(name, type, size, digits, mdx));
            }

            if (_Reader.ReadByte() != 13)
                throw new IOException("Unexepected terminator value.");
        }

        #region IDisposable Support
        private bool _Disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_Disposed)
            {
                if (disposing)
                {
                    _Reader.Close();
                }

                _Disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

        public bool ReadRow()
        {
            if (_CurrentRowIndex < RowCount)
            {
                Encoding dosEnc = Encoding.GetEncoding(866);

                if (_Reader.ReadByte() != 32)
                    throw new NotImplementedException();

                _CurrentRow = new object[Fields.Count];
                ushort index = 0;
                foreach (var field in Fields)
                {
                    _CurrentRow[index] = dosEnc.GetString(_Reader.ReadBytes(field.Item3)).Trim();
                    index++;
                }

                _CurrentRowIndex++;
                return true;
            }

            return false;
        }

        public object Value(string columnName)
        {
            return _CurrentRow[_Fields.FindIndex(f => f.Item1 == columnName)];
        }
    }
}
