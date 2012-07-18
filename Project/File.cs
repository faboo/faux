using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project
{
    public class File : Node
    {


        public override void ReadXml(System.Xml.XmlReader reader)
        {
            reader.ReadStartElement();

            while (reader.IsStartElement())
            {
                if (reader.Name == "Name")
                {
                    Name = reader.ReadElementContentAsString();
                }
                else if (reader.Name == "RealPath")
                {
                    RealPath = reader.ReadElementContentAsString();
                }
                else
                {
                    reader.Read();
                }
            }

            reader.ReadEndElement();
        }

        protected override System.Windows.Freezable CreateInstanceCore()
        {
            return new File();
        }

		protected void OnReadPathChanged(DependencyPropertyChangedEventArgs args){
			for(var type in Type.AllTypes)
				if(type.Pattern.IsMatch(RealPath)){
					Type = type;
					break;
				}
		}

		private static void OnReadPathChanged(DependencyObject source, DependencyPropertyChangedEventArgs args){
			((File)source).OnReadPathChanged(args);
		}
    }
}
