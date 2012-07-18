//Faux, Copyright (C) 2012 Ray Wallace
//
//This program is free software; you can redistribute it and/or modify it under
//the terms of the GNU General Public License as published by the Free Software
//Foundation version 2 of the Licens.
//
//This program is distributed in the hope that it will be useful, but WITHOUT
//ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
//FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more
//details.
//
//You should have received a copy of the GNU General Public License along with
//this program; if not, write to the Free Software Foundation, Inc., 51 Franklin
//Street, Fifth Floor, Boston, MA  02110-1301, USA.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Project
{
    public class SDictionary<Key, Value> : Dictionary<Key, Value>, IXmlSerializable
    {

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            XmlSerializer keyDes = new XmlSerializer(typeof(Key));
            XmlSerializer valueDes = new XmlSerializer(typeof(Value));

            reader.ReadStartElement();
            Key key;
            Value value;

            while (reader.IsStartElement())
            {
                if (reader.Name == "Pair")
                {
                    reader.ReadStartElement();

                    reader.ReadStartElement("Key");
                    key = (Key)keyDes.Deserialize(reader);
                    reader.ReadEndElement();
                    reader.ReadStartElement("Value");
                    value = (Value)valueDes.Deserialize(reader);
                    reader.ReadEndElement();

                    reader.ReadEndElement();
                }
                else
                {
                    reader.Read();
                }
            }

            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            XmlSerializer keySer = new XmlSerializer(typeof(Key));
            XmlSerializer valueSer = new XmlSerializer(typeof(Value));

            foreach (var kvp in this)
            {
                writer.WriteStartElement("Pair");

                writer.WriteStartElement("Key");
                keySer.Serialize(writer, kvp.Key);
                writer.WriteEndElement();
                writer.WriteStartElement("Value");
                valueSer.Serialize(writer, kvp.Value);
                writer.WriteEndElement();

                writer.WriteEndElement();
            }
        }
    }
}
