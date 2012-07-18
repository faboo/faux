using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;

namespace Project
{
    public class Folder : Node, IXmlSerializable
    {
        public List<Node> Contents { get; set; }

        public Folder()
        {
            Type = Type.AllTypes.First(t => t.Name == "Folder");
            Contents = new List<Node>();
        }

        protected override System.Windows.Freezable CreateInstanceCore()
        {
            return new Folder();
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            reader.ReadStartElement();

            while (reader.IsStartElement())
            {
                if (reader.Name == "Name")
                {
                    Name = reader.ReadElementContentAsString();
                }
                else if (reader.Name == "Contents")
                {
                    reader.ReadStartElement();
                    ReadContents(reader);
                    reader.ReadEndElement();
                }
                else
                {
                    reader.Read();
                }
            }

            reader.Read();
        }

        private void ReadContents(XmlReader reader)
        {
            XmlSerializer des = null;
            XmlSerializer folderDes = new XmlSerializer(typeof(Folder));
            XmlSerializer fileDes = new XmlSerializer(typeof(File));

            while (reader.IsStartElement())
            {
                if (reader.Name == "Folder")
                    des = folderDes;
                else // (reader.Name == "File")
                    des = fileDes;

                Contents.Add((Node)des.Deserialize(reader));
            }
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
