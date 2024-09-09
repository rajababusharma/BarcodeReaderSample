using PdfSharpCore.Fonts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BarcodeReaderSample
{
    internal class FileFontResolver : IFontResolver
    {
        // public string DefaultFontName => throw new NotImplementedException();
         public string DefaultFontName => "verdana_regular";
        public byte[] GetFont(string faceName)
        {
            var assembly = typeof(FileFontResolver).Assembly;
            // var stream = assembly.GetManifestResourceStream($"BarcodeReaderSample.Resources.Fonts.{faceName}");
            var directory = $"BarcodeReaderSample.Resources.Fonts.{faceName}.ttf";
            var stream = assembly.GetManifestResourceStream(directory);
            using (var reader = new StreamReader(stream))
            {
                var bytes = default(byte[]);
                using (var ms = new MemoryStream())
                {
                    reader.BaseStream.CopyTo(ms);
                    bytes = ms.ToArray();
                   /* using (var fs = File.Open(faceName, FileMode.Open))
                    {
                        fs.CopyTo(ms);
                        ms.Position = 0;
                        return ms.ToArray();
                    }*/
                }
                return bytes;
            }
           
        }

        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
           
                return new FontResolverInfo(familyName);
                          
        }

    }
}