using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Windows;

namespace Project
{
    public abstract class Node : Freezable, IXmlSerializable
    {
        public string Name { get; set; }
        [XmlIgnore]
        public Type Type { get; protected set; }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public virtual void ReadXml(System.Xml.XmlReader reader)
        {
            reader.ReadStartElement();

            while (reader.IsStartElement())
            {
                if (reader.Name == "Name")
                {
                    Name = reader.ReadElementContentAsString();
                }
                else
                {
                    reader.Read();
                }
            }
            
            reader.ReadEndElement();
        }

        public virtual void WriteXml(System.Xml.XmlWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
