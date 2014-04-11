using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;

namespace Project {
    public class ExternalFolder : Folder {
        private static ImageSource OVERLAY;

        private bool inExternalFolder = false;

        public ExternalFolder() {
        }

        public ExternalFolder(ExternalFile file) {
            Name = file.Name;
            RealPath = file.RealPath;
        }

        static ExternalFolder() {
            BitmapImage ic = new BitmapImage();

            ic.BeginInit();
            ic.UriSource = new Uri(IconCache.Get("pack://application:,,,/Icons/Shortcut Overlay.ico"));

            ic.EndInit();

            OVERLAY = ic;
        }

        public string RealPath { get; set; }
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
        public override bool ChangeableContents {
            get {
                return false;
            }
        }


        public void Update() {
            this.Contents.Clear();

            foreach(string file in Directory.GetFiles(RealPath)) {
                string realPath = Path.Combine(RealPath, Path.GetFileName(file));
                this.Add(new ExternalFile {
                    Name = Path.GetFileName(file),
                    RealPath = realPath,
                });
            }
            foreach(string dir in Directory.GetDirectories(RealPath)) {
                string realPath = Path.Combine(RealPath, Path.GetFileName(dir));
                ExternalFolder child = new ExternalFolder() {
                    Name = Path.GetFileName(dir),
                    RealPath = realPath,
                };
                child.Update();
                this.Add(child);
            }
        }
    }
}
