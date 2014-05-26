using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Project {
    public class ExternalFile : File {
        private static ImageSource OVERLAY;

        private bool inExternalFolder = false;

        public ExternalFile() {
        }

        public ExternalFile(ExternalFile file) {
            Name = file.Name;
            RealPath = file.RealPath;
        }

        static ExternalFile() {
            BitmapImage ic = new BitmapImage();

            ic.BeginInit();
            ic.UriSource = new Uri(IconCache.Get("pack://application:,,,/Icons/Shortcut Overlay.ico"));

            ic.EndInit();

            OVERLAY = ic;
        }

        public override ImageSource Overlay {
            get { return OVERLAY; }
        }
        public override bool Moveable {
            get { return !inExternalFolder; }
        }
        public override Folder Parent {
            get { return base.Parent; }
            set {
                if(value is ExternalFolder)
                    inExternalFolder = true;
                base.Parent = value;
            }
        }
        public override bool ShouldSerialize {
            get {
                return !(Parent is ExternalFolder);
            }
        }
    }
}
